
namespace Orama.Rendering.Backends;

/// <summary>
/// Backend-agnostic way of sending commands to the GPU.
/// </summary>
/// <remarks>
/// Some methods may require being ran once per frame before rendering whilst others can be run once.
/// </remarks>
public interface ICommandBuffer
{
    /// <summary> Clears the screen. </summary>
    void Clear(float r, float g, float b, float a);

    /// <summary> Enables a <see cref="RenderFeature"/>. </summary>
    void EnableFeature(RenderFeature feature);

    /// <summary> Disables a <see cref="RenderFeature"/>. </summary>
    void DisableFeature(RenderFeature feature);
}

/// <summary>
/// A Toggleable feature of the GPU.
/// </summary>
public enum RenderFeature
{
    CullFaces
}