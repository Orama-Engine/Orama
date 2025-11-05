
using Orama.Core.Common;
using Orama.Core.Common.Entities;
using Orama.Core.Common.Utility;
using Orama.Core.Modules.Rendering.Components;
using Orama.Core.Modules.Rendering.Resources;

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

        meshRenderer.Mesh = Application.ResourceProvider.GetResource<Mesh>(gizmoMeshName);
    }
}

internal enum GizmoType
{
    Translate
}
