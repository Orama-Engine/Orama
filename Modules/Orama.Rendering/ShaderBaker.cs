using NeoVeldrid;
using Orama.Common.Utility;
using Orama.Rendering.Resources;
using SlangShaderSharp;

namespace Orama.Rendering;

/// <summary>
/// Handles baking Slang module source code into a <see cref="SlangCompilationResult"/>.
/// </summary>
public static class ShaderBaker
{
    private static IGlobalSession globalSession;
    private static ISession localSession;

    static ShaderBaker()
    {
        Slang.CreateGlobalSession(Slang.ApiVersion, out var gs);
        globalSession = gs;

        CompilerOptionEntry[] options =
        [
            new CompilerOptionEntry
                {
                    Name = CompilerOptionName.MatrixLayoutColumn,
                    Value = new CompilerOptionValue
                    {
                        Kind = CompilerOptionValueKind.Int,
                        IntValue0 = 1
                    }
                }
        ];


        SessionDesc sesDesc = new()
        {
            Targets = [new TargetDesc { Format = SlangCompileTarget.Spirv }],
            SearchPaths = ["Assets"], // Hacky
            CompilerOptionEntries = options,
        };

        globalSession.CreateSession(sesDesc, out var ls);
        localSession = ls;
    }

    /// <summary> Compiles Slang source to SPIRV. </summary>
    /// <param name="source"> The string slang source code. </param>
    /// <param name="moduleName"> The name of the Slang module. This is used for importing this compilation into other modules. </param>
    public static SlangCompilationResult SlangToSpirV(string source, string moduleName)
    {
        IModule? module = localSession.LoadModuleFromSourceString(moduleName, $"{moduleName}.slang", source, out ISlangBlob? diagnostics);
        if (module == null)
            throw new Exception($"Failed to compile shader: {diagnostics?.AsString}");

        IEntryPoint? vertexEntry = null;
        IEntryPoint? fragmentEntry = null;

        for (int i = 0; i < module.GetDefinedEntryPointCount(); i++)
        {
            module.GetDefinedEntryPoint(i, out var entryPoint);
            ShaderReflection reflection = entryPoint.GetLayout(0, out _);
            SlangStage stage = reflection.GetEntryPointByIndex(0).Stage;

            switch (stage)
            {
                case SlangStage.Vertex:
                    vertexEntry = entryPoint;
                    break;

                case SlangStage.Fragment:
                    fragmentEntry = entryPoint;
                    break;
            }
        }

        List<AttributeReflection> attributes = new();

        ShaderReflection layout = module.GetLayout(0, out _);
        TypeReflection? attributesType = layout.FindTypeByName("ShaderAttributes");
        if (attributesType != null)
        {
            var aCount = attributesType.Value.AttributeCount;

            for (uint i = 0; i < aCount; i++)
            {
                var attribute = attributesType.Value.GetAttribute(i);
                attributes.Add(attribute);
            }
        }

        List<VariableReflection> parameters = new();
        TypeReflection? parametersType = layout.FindTypeByName("ShaderParameters");
        if (parametersType != null)
        {
            var pCount = parametersType.Value.FieldCount;

            for (uint i = 0; i < pCount; i++)
            {
                var parameter = parametersType.Value.GetFieldByIndex(i);
                parameters.Add(parameter);
            }
        }

        byte[] vert = Array.Empty<byte>();
        byte[] frag = Array.Empty<byte>();

        List<ShaderResource> resources = new();

        if (vertexEntry == null && fragmentEntry == null)
            return new SlangCompilationResult() { ShaderAttributes = attributes, ShaderParameters = parameters, Resources = resources };

        unsafe
        {
            if (vertexEntry != null)
            {
                IComponentType[] components = { module, vertexEntry };
                localSession.CreateCompositeComponentType(components, out IComponentType vertexProgram, out _);
                vertexProgram.Link(out IComponentType linkedVertex, out _);
                linkedVertex.GetEntryPointCode(0, 0, out ISlangBlob vertexBlob, out _);


                var linkedLayout = linkedVertex.GetLayout(0, out _);

                for (uint i = 0; i < linkedLayout.ParameterCount; i++)
                {
                    VariableLayoutReflection variable = linkedLayout.GetParameterByIndex(i);

                    if (variable.Type.Kind == SlangTypeKind.ParameterBlock || variable.Type.Kind == SlangTypeKind.ConstantBuffer)
                    {
                        resources.Add(new ShaderResource(
                            variable.Name,
                            ResourceKind.UniformBuffer,
                            variable.BindingIndex
                        ));
                    }
                }

                vert = new byte[vertexBlob.GetBufferSize()];
                fixed (byte* dst = vert)
                    Buffer.MemoryCopy(vertexBlob.GetBufferPointer(), dst, vert.Length, vert.Length);
            }

            if (fragmentEntry != null)
            {
                IComponentType[] components = { module, fragmentEntry };
                localSession.CreateCompositeComponentType(components, out IComponentType fragmentProgram, out _);
                fragmentProgram.Link(out IComponentType linkedFragment, out _);
                linkedFragment.GetEntryPointCode(0, 0, out ISlangBlob fragmentBlob, out _);

                frag = new byte[fragmentBlob.GetBufferSize()];
                fixed (byte* dst = frag)
                    Buffer.MemoryCopy(fragmentBlob.GetBufferPointer(), dst, frag.Length, frag.Length);
            }
        }

        return new SlangCompilationResult() { FragmentBytes = frag, VertexBytes = vert, ShaderAttributes = attributes, ShaderParameters = parameters, Resources = resources };
    }
}

/// <summary>
/// Result of a Slang compilation.
/// </summary>
public readonly ref struct SlangCompilationResult
{
    /// <summary> SPIRV Vertex bytes. </summary>
    public byte[] VertexBytes { get; init; }

    /// <summary> SPIRV Fragment bytes. </summary>
    public byte[] FragmentBytes { get; init; }

    /// <summary> All attributes inside of a <c>SHADER_ATTRIBUTES()</c> block. </summary>
    /// <example>
    /// The following is Slang source that defines a <c>ShaderPass</c> Shader Attribute:
    /// <code>
    /// SHADER_ATTRIBUTES(
    ///     [ShaderPass("Opaque")]
    /// )
    /// </code>
    /// </example>
    public List<AttributeReflection> ShaderAttributes { get; init; }

    /// <summary> All parameters inside of a <c>SHADER_PARAMETERS()</c> block. </summary>
    public List<VariableReflection> ShaderParameters { get; init; }

    /// <summary> All resources this shader requests. </summary>
    public List<ShaderResource> Resources { get; init; }
}