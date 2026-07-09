using Orama.Common.Utility;
using SlangShaderSharp;

namespace Orama.Rendering;

/// <summary>
/// Bakes Slang shaders to SPIRV.
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
            CompilerOptionEntries = options,
        };

        globalSession.CreateSession(sesDesc, out var ls);
        localSession = ls;
    }

    /// <summary> Compiles Slang source to SPIRV. </summary>
    public static (byte[] Vert, byte[] Frag) SlangToSpirV(string source)
    {
        IModule? module = localSession.LoadModuleFromSourceString("shader", "shader.slang", source, out _);
        if (module == null)
            throw new Exception("TBD");
            
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

        // TODO: Support only one entry point
        if (vertexEntry == null || fragmentEntry == null)
            throw new Exception("TBD");

        IComponentType[] components =
        {
            module,
            vertexEntry,
        };

        localSession.CreateCompositeComponentType(components, out IComponentType vertexProgram, out _);

        vertexProgram.Link(out IComponentType linkedVertex, out _);

        linkedVertex.GetEntryPointCode(0, 0, out ISlangBlob vertexBlob, out _);

        components =
        [
            module,
            fragmentEntry,
        ];

        localSession.CreateCompositeComponentType(components, out IComponentType fragmentProgram, out _);

        fragmentProgram.Link(out IComponentType linkedFragment, out _);

        linkedFragment.GetEntryPointCode(0, 0, out ISlangBlob fragmentBlob, out _);

        unsafe
        {
            byte[] vert = new byte[vertexBlob.GetBufferSize()];
            fixed (byte* dst = vert)
                Buffer.MemoryCopy(vertexBlob.GetBufferPointer(), dst, vert.Length, vert.Length);

            byte[] frag = new byte[fragmentBlob.GetBufferSize()];
            fixed (byte* dst = frag)
                Buffer.MemoryCopy(fragmentBlob.GetBufferPointer(), dst, frag.Length, frag.Length);

            return (vert, frag);
        }

    }
}
