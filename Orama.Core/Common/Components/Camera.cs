
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;
using Orama.Rendering;

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

    /// <summary> The target texture to render to. </summary>
    public Texture? Target { get; set; }

    /// <summary> The main camera. </summary>
    public static Camera? Main { get; private set; }


    // TODO: We should probably have a consistent format for camera matrices and modify them right before upload but for now we change them depending on backend

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

            Matrix4x4 proj;

            if (Renderer.Veldrid.GraphicsDevice.IsDepthRangeZeroToOne)
            {
                // D3D11 / Metal ortho depth [0, 1]
                proj = new Matrix4x4(
                    f / AspectRatio, 0, 0, 0,
                    0, f, 0, 0,
                    0, 0, ZFar / (ZFar - ZNear), 1,
                    0, 0, -ZNear * ZFar / (ZFar - ZNear), 0
                );
            }
            else
            {
                // Vulkan / OpenGL ortho depth [-1, 1]
                float zRange = ZFar - ZNear;
                proj = new Matrix4x4(
                    f / AspectRatio, 0, 0, 0,
                    0, f, 0, 0,
                    0, 0, ZFar / zRange, 1,
                    0, 0, -(ZNear * ZFar) / zRange, 0
                );
            }

            if (Renderer.Veldrid.GraphicsDevice.IsClipSpaceYInverted)
            {
                proj.M21 = -proj.M21;
                proj.M22 = -proj.M22;
                proj.M23 = -proj.M23;
                proj.M24 = -proj.M24;
            }

            return proj;
        }
    }

    /// <summary> Gets the orthographic projection <see cref="Matrix4x4"/> for this Camera. </summary>
    public Matrix4x4 OrthographicMatrix
    {
        get
        {
            float right = AspectRatio;
            float left = -AspectRatio;
            float top = 1f;
            float bottom = -1f;

            float rl = right - left;
            float tb = top - bottom;

            Matrix4x4 proj;

            if (Renderer.Veldrid.GraphicsDevice.IsDepthRangeZeroToOne)
            {
                // D3D11 / Metal ortho depth [0, 1]
                float zRange = ZFar - ZNear;
                proj = new Matrix4x4(
                    2f / rl, 0, 0, 0,
                    0, 2f / tb, 0, 0,
                    0, 0, 1f / zRange, 0,
                    -(right + left) / rl, -(top + bottom) / tb, -ZNear / zRange, 1
                );
            }
            else
            {
                // Vulkan / OpenGL ortho depth [-1, 1]
                float zRange = ZFar - ZNear;
                proj = new Matrix4x4(
                    2f / rl, 0, 0, 0,
                    0, 2f / tb, 0, 0,
                    0, 0, -2f / zRange, 0,
                    -(right + left) / rl, -(top + bottom) / tb, -(ZFar + ZNear) / zRange, 1
                );
            }

            if (Renderer.Veldrid.GraphicsDevice.IsClipSpaceYInverted)
            {
                proj.M21 = -proj.M21;
                proj.M22 = -proj.M22;
                proj.M23 = -proj.M23;
                proj.M24 = -proj.M24;
            }

            return proj;
        }
    }

    /// <summary> Initializes a new instance of <see cref="Camera"/>. </summary>
    public Camera()
    {
        Main = this;
    }
}
