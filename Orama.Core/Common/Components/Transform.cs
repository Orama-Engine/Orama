
using Orama.Math;

namespace Orama.Core.Common.Components;

/// <summary>
/// A 3D Position, Rotation and Scale.
/// </summary>
public class Transform : Component
{
    /// <summary> The position of the transform. </summary>
    public Vector3 Position { get; set; }

    /// <summary> The rotation of the transform. </summary>
    public Quaternion Rotation { get; set; } = Quaternion.FromEulerAngles(new Vector3(0f, 0f, 0f));

    /// <summary> The scale of the transform. </summary>
    public Vector3 Scale { get; set; } = Vector3.One;

    /// <summary> The combined <see cref="Matrix4x4"/> of the transform. </summary>
    public Matrix4x4 Matrix
    {
        get
        {
            Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(Scale);
            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(Rotation);
            Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(Position);

            return scaleMatrix * rotationMatrix * translationMatrix;
        }
    }

    /// <summary> The forward direction of the transform. </summary>
    public Vector3 Forward => Vector3.Transform(-Vector3.Forward, Rotation);

    /// <summary> The right direction of the transform. </summary>
    public Vector3 Right => Vector3.Transform(Vector3.Right, Rotation);

    /// <summary> The up direction of the transform. </summary>
    public Vector3 Up => Vector3.Transform(Vector3.Up, Rotation);

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
