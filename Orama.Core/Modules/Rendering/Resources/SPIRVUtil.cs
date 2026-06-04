using Silk.NET.SPIRV.Reflect;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Orama.Core.Modules.Rendering.Resources;

// Hacky, should move this
internal static class SPIRVUtil
{
    public static Dictionary<Silk.NET.SPIRV.Op, ShaderParameter.ParamType> ParamTypeMap = new Dictionary<Silk.NET.SPIRV.Op, ShaderParameter.ParamType>()
    {
        { Silk.NET.SPIRV.Op.TypeFloat, ShaderParameter.ParamType.Float },
        { Silk.NET.SPIRV.Op.TypeMatrix, ShaderParameter.ParamType.Matrix },
    };

    /// <summary> Extracts parameter definitions from the specified SPIR-V bytecode. </summary>
    public static List<ShaderParameter> ExtractParameters(byte[] bytecode)
    {
        List<ShaderParameter> parameters = new List<ShaderParameter>();

        unsafe
        {
            Reflect api = Reflect.GetApi();

            ReflectShaderModule module = default;

            fixed (byte* code = bytecode)
            {
                Result result = api.CreateShaderModule((nuint)bytecode.Length, code, ref module);

                if (result != Result.Success)
                    throw new Exception($"Shader compilation failed: {result}");

                try
                {
                    uint count = 0;
                    api.EnumerateDescriptorBindings(ref module, ref count, (DescriptorBinding**)null);

                    DescriptorBinding*[] bindings = new DescriptorBinding*[count];

                    fixed (DescriptorBinding** ppBindings = bindings)
                    {
                        api.EnumerateDescriptorBindings(ref module, ref count, ppBindings);

                        for (int i = 0; i < count; i++)
                        {
                            DescriptorBinding* binding = bindings[i];
                            if (binding->DescriptorType == DescriptorType.UniformBuffer || binding->DescriptorType == DescriptorType.StorageBuffer)
                            {
                                var block = binding->Block;

                                for (int m = 0; m < block.MemberCount; m++)
                                {
                                    var member = block.Members[m];
                                    string? name = Marshal.PtrToStringUTF8((nint)member.Name);

                                    TypeDescription* type = member.TypeDescription;

                                    if (!ParamTypeMap.TryGetValue(type->Op, out ShaderParameter.ParamType paramType))
                                        continue;

                                    parameters.Add(new ShaderParameter(name ?? "Unknown", paramType, binding->Binding));

                                    Console.WriteLine($"{name} ({type->Op})");
                                }
                            }

                        }
                    }
                }
                finally
                {
                    api.DestroyShaderModule(ref module);
                }
            }
        }

        return parameters;
    }
}
