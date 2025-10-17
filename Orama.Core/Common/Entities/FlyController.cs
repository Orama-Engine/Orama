using Orama.Core.Common.Components;
using Orama.Core.Common.Utility;
using Orama.Core.Modules;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Rendering.Resources;
using Orama.Math;
using Orama.Rendering.Resources;

namespace Orama.Core.Common.Entities;

internal class FlyController : Entity
{
    [ImplicitComponent] public Camera Camera { get; set; } = null!;

    private float mouseSensitivity = 0.25f;

    private float pitch;
    private float yaw;

    private Texture? screenshot;

    bool cursorLocked = false;

    public override void Update()
    {
        base.Update();

        var Input = ModuleManager.GetModule<InputModule>();
        if (Input == null)
            return;

        if (Input.IsKeyPressed(Key.Escape))
        {
            cursorLocked = !cursorLocked;
            Input.CursorLocked = cursorLocked;
        }

        // Movement
        if (Input.IsKeyDown(Key.W)) Transform.Position += Transform.Forward * 0.1f;
        if (Input.IsKeyDown(Key.S)) Transform.Position -= Transform.Forward * 0.1f;
        if (Input.IsKeyDown(Key.A)) Transform.Position -= Transform.Right * 0.1f;
        if (Input.IsKeyDown(Key.D)) Transform.Position += Transform.Right * 0.1f;
        if (Input.IsKeyDown(Key.Q)) Transform.Position -= Transform.Up * 0.1f;
        if (Input.IsKeyDown(Key.E)) Transform.Position += Transform.Up * 0.1f;

        if (!cursorLocked)
            return;

        // Mouse look
        Vector2 delta = Input.MouseDelta;

        // Update cumulative yaw/pitch
        yaw += -delta.X * mouseSensitivity * Time.Delta;
        pitch += -delta.Y * mouseSensitivity * Time.Delta;

        // Build rotation from cumulative angles
        Quaternion yawRot = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);
        Quaternion pitchRot = Quaternion.CreateFromAxisAngle(Vector3.Right, pitch);
        Transform.Rotation = yawRot * pitchRot;

        if (screenshot?.GetData() != null || screenshot?.GetData() != Array.Empty<byte>())
        {
            screenshot?.ToPng("screenshot.png");
            Camera.Target = null;
            screenshot = null;
        }

        // Take a screenshot
        if (Input.IsKeyPressed(Key.Space))
        {
            screenshot = new Texture(Application.Window.Size.X, Application.Window.Size.Y, TextureDataType.RGBA8);
            Camera.Target = screenshot;
        }
    }
}
