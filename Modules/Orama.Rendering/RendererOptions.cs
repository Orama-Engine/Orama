
namespace Orama.Rendering;

public enum CullingMode
{
    None,
    Front,
    Back
}

public struct RendererOptions
{
    /// <summary> The culling mode to use. </summary>
    public CullingMode Culling { get; set; }
}
