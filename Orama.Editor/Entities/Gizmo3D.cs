using Orama.Core.Common;
using Orama.Core.Common.Entities;
using Orama.Core.Common.Utility;
using Orama.Core.Modules.Rendering.Components;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Rendering;
using Silk.NET.OpenGL;

namespace Orama.Editor.Entities;

/// <summary>
/// A Gizmo that exists in 3D space.
/// </summary>
internal class Gizmo3D : Entity
{
    /// <summary> The type of the gizmo. </summary>
    public GizmoType Type { get; set; }

    [ImplicitComponent]
    private MeshRenderer meshRenderer = null!;

    const string GIZMO_VERTEX = @"
#version 450 core

layout(location = 0) in vec3 pos;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 uv;

layout(std140, binding = 0) uniform ShaderParams
{
    mat4 u_MVP;
};

void main()
{
    gl_Position = u_MVP * vec4(pos, 1.0);
}
";

    const string GIZMO_FRAGMENT = @"
#version 450 core

layout(location = 0) out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0, 0.0, 0.0, 1.0);
}
";

    public override void Start()
    {
        base.Start();

        string gizmoMeshName = Type switch
        {
            GizmoType.Translate => "Assets/TranslateGizmo.fbx",
            _ => throw new NotImplementedException()
        };

        Material gizmoMaterial = new(new Core.Modules.Rendering.Resources.Shader(GIZMO_VERTEX, GIZMO_FRAGMENT));

        meshRenderer.Mesh = Application.ResourceProvider.GetResource<Mesh>(gizmoMeshName);
        meshRenderer.Mesh?.Material = gizmoMaterial;
    }
}

internal enum GizmoType
{
    Translate
}
