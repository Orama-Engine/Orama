using Assimp;
using Orama.Math;

namespace Orama.Core.Common.Resources.DefaultProvider;

[ResourceLoader]
internal class MeshLoader : ResourceLoader<Modules.Rendering.Resources.Mesh>
{
    /// <inheritdoc/>
    public override Modules.Rendering.Resources.Mesh? LoadResource(byte[] data)
    {
        using var ms = new MemoryStream(data);
        using var importer = new AssimpContext();

        // TODO: figure out how to tell assimp the file extension
        var scene = importer.ImportFileFromStream(ms, PostProcessPreset.TargetRealTimeMaximumQuality, ".fbx");

        if (scene == null || scene.MeshCount == 0)
            return null;

        var mesh = scene.Meshes[0];

        var vertices = mesh.Vertices
            .Select(v => new Vector3(v.X, v.Y, v.Z))
            .ToArray();

        var normals = mesh.Normals
            .Select(n => new Vector3(n.X, n.Y, n.Z))
            .ToArray();

        var indices = mesh.GetIndices();

        Modules.Rendering.Resources.Mesh output = new();
        output.Vertices = vertices;
        output.Normals = normals;
        output.Indices = indices.Select(i => unchecked((uint)i)).ToArray();

        return output;
    }
}
