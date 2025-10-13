
using Orama.Math;

namespace Orama.Core.Common.Components;

public class Camera : Component
{
    /// <summary> The field of view. </summary>
    public float FOV { get; set; } = MathF.PI / 3f;

    /// <summary> The aspect ratio. </summary>
    public float AspectRatio { get; set; } = 16f / 9f;

    /// <summary> The near plane. </summary>
    public float ZNear { get; set; } = 0.1f;

    /// <summary> The far plane. </summary>
    public float ZFar { get; set; } = 1000f;

    /// <summary> The main camera. </summary>
    public static Camera? Main { get; private set; }

    /// <summary> Gets the view <see cref="Matrix4x4"/> for this Camera. </summary>
    public Matrix4x4 ViewMatrix
    {
        get
        {
            return Matrix4x4.LookAt(
                Entity.Transform.Position,
                Entity.Transform.Position + Entity.Transform.Forward,
                Vector3.Up
            );
        }
    }

    /// <summary> Gets the projection <see cref="Matrix4x4"/> for this Camera. </summary>
    public Matrix4x4 ProjectionMatrix
    {
        get
        {
            float f = 1f / MathF.Tan(FOV / 2f);
            float zRange = ZNear - ZFar;

            return new Matrix4x4(
                f / AspectRatio, 0, 0, 0,
                0, f, 0, 0,
                0, 0, (ZFar + ZNear) / zRange, -1,
                0, 0, (2 * ZFar * ZNear) / zRange, 0
            );
        }
    }

    /// <summary> Gets the orthographic projection <see cref="Matrix4x4"/> for this Camera. </summary>
    public Matrix4x4 OrthographicMatrix
    {
        get
        {
            float right = AspectRatio * 1f;
            float left = -right;
            float top = 1f;
            float bottom = -top;

            float zRange = ZFar - ZNear;

            return new Matrix4x4(
                2f / (right - left), 0, 0, 0,
                0, 2f / (top - bottom), 0, 0,
                0, 0, -2f / zRange, 0,
                -(right + left) / (right - left),
                -(top + bottom) / (top - bottom),
                -(ZFar + ZNear) / zRange,
                1
            );
        }
    }

    public Camera()
    {
        Main = this;
    }
}
