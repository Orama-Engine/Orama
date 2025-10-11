
using Orama.Math;

namespace Orama.Core.Common.Components;

/// <summary>
/// A 3D Position, Rotation and Scale.
/// </summary>
public class Transform : BaseComponent
{
    /// <summary> The position of the transform. </summary>
    public Vector3 Position { get; set; }

    /// <summary> The rotation of the transform. </summary>
    public Quaternion Rotation { get; set; }

    /// <summary> The scale of the transform. </summary>
    public Vector3 Scale { get; set; } = Vector3.One;

    /// <summary> Creates a new instance of <see cref="Transform"/>. </summary>
    public Transform() { }

    /// <summary> Creates a new instance of <see cref="Transform"/> from the given position, rotation and scale. </summary>
    public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}
