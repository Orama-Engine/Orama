using Orama.Core.Common.Components;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;

namespace Orama.Core.Modules.Rendering.Components;

public class MeshRenderer : Component, IClientRenderable
{
    /// <summary> The <see cref="Resources.Mesh"/> to render. </summary>
    public Mesh? Mesh { get; set; }

    public Matrix4x4 Transform => Entity.Transform.Matrix;

    public Vector3[] Vertices => Mesh?.Vertices ?? Array.Empty<Vector3>();

    public Vector3[] Normals => Mesh?.Normals ?? Array.Empty<Vector3>();

    public Vector2[] UVs => Mesh?.UVs ?? Array.Empty<Vector2>();

    public uint[] Indices => Mesh?.Indices ?? Array.Empty<uint>();

    public Material Material => Mesh?.Material ?? Material.Default;

    public override void Update()
    {
        if (Mesh == null)
            return;

        ModuleManager.GetModule<RenderingModule>()?.QueueObject(this);
    }
}
