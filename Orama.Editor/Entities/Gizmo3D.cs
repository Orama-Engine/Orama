using Orama.Core.Common;
using Orama.Core.Common.Entities;
using Orama.Core.Common.Utility;
using Orama.Core.Modules.Rendering.Components;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;
using Orama.Rendering;

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

    public override void Start()
    {
        base.Start();

        string gizmoMeshName = Type switch
        {
            GizmoType.Translate => "Assets/TranslateGizmo.fbx",
            _ => throw new NotImplementedException()
        };

        Material gizmoMaterial = new Material(Application.ResourceProvider.GetResource<Shader>("Assets/Gizmo.shader") ?? throw new InvalidOperationException());
        gizmoMaterial.SetParameter("Color", Color.Red);

        meshRenderer.Mesh = Application.ResourceProvider.GetResource<Mesh>(gizmoMeshName);
        meshRenderer.Mesh?.Material = gizmoMaterial;
    }
}

internal enum GizmoType
{
    Translate
}
