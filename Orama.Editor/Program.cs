using Orama.Core.Common;
using Orama.Core.Common.Entities;
using Orama.Core.Common.Utility;
using Orama.Core.Modules;
using Orama.Core.Modules.Assemblies;
using Orama.Core.Modules.GUI;
using Orama.Core.Modules.GUI.Widgets;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Physics;
using Orama.Core.Modules.Rendering;
using Orama.Core.Modules.Scenes;
using Orama.Editor.Widgets;
using Orama.Math;
using Orama.Rendering;
using Orama.Rendering.Resources;
using Orama.ShaderLang;
using Orama.ShaderLang.Targets;

namespace Orama.Editor;

internal class Program
{
    const string SHADER_LANG_SOURCE = @"
#vertex VertexEntryPoint
#fragment FragmentEntryPoint

Name = ""Default/White""
Pass = ""Opaque""

Properties
{
    float3 pos;
    float3 normal;
    float2 uv;
    float4x4 u_MVP;
}

Source
{

}
";
    static void Main(string[] args)
    {
        // REGISTER MODULES
        ModuleManager.RegisterModule<AssemblyModule>();
        ModuleManager.RegisterModule<PhysicsModule>();
        ModuleManager.RegisterModule<SceneModule>();
        ModuleManager.RegisterModule<RenderingModule>();
        ModuleManager.RegisterModule<GUIModule>();
        ModuleManager.RegisterModule<InputModule>();
        ModuleManager.RegisterModule<EditorModule>();

        Application.OnStart += () =>
        {
            EngineOutput.Log("Hello World!");

            ShaderLangFormat format = ShaderLangFormat.FromSource(SHADER_LANG_SOURCE);

            foreach (var meta in format.MetaData)
                EngineOutput.Log($"{meta.Key}: {meta.Value}");

            EngineOutput.Log(format.Name ?? "Name Not Found!");
            EngineOutput.Log(format.Pass ?? "Pass Not Found!");

            (string, string) compiled = HLSLTarget.Compile(format);
            EngineOutput.Log(compiled.Item1);
            EngineOutput.Log(compiled.Item2);
            GraphicsShader shader = ShaderBaker.HLSLToShader(compiled.Item1, compiled.Item2);
            (string, string) glsl = ShaderUnbaker.SpirVToGLSL(shader.VertexBytes, shader.FragmentBytes);

            EngineOutput.Log(glsl.Item1);
            EngineOutput.Log(glsl.Item2);



            FlyController flyController = new();
            flyController.Name = "Camera";
            flyController.Transform.Position = new Vector3(0, 0, 0);
            flyController.Start();

            Label FPS = new("FPS: N/A");
            FPS.Rect = new Rect(5, 5, 0, 0);
            Application.OnRender += () => FPS.Text = $"FPS: {Application.Window.FramesPerSecond}";

            HierarchyWindow hierarchy = new HierarchyWindow();
            hierarchy.Rect = new Rect(200, 200, 300, 200);

            InspectorWindow inspector = new InspectorWindow();
            inspector.Rect = new Rect(500, 200, 300, 200);
            hierarchy.EntitySelected += () => inspector.Target = hierarchy.SelectedEntity;

            ModuleManager.GetModule<GUIModule>()?.Widgets.Add(FPS);
            ModuleManager.GetModule<GUIModule>()?.Widgets.Add(hierarchy);
            ModuleManager.GetModule<GUIModule>()?.Widgets.Add(inspector);
        };

        Application.OnExit += () =>
        {

        };

        Application.OnUpdate += () =>
        {

        };

        Application.OnRender += () =>
        {

        };

        Application.Initialize();
    }
}

