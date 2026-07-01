using Orama.Rendering.Components;
using Orama.Input;
using Orama.Math;
using Orama.Physics;
using Orama.Scenes.Entities;
using Orama.Common;
using Orama.Common.Utility;

namespace Orama.Rendering.Entities;

[Entity("fly_controller")]
public class FlyController : Entity
{
    [ImplicitComponent] public Camera Camera { get; set; } = null!;

    private float mouseSensitivity = 0.0025f;
    private float moveSpeed = 8.0f;

    private float pitch;
    private float yaw;

    bool cursorLocked = false;

    public override void Update()
    {
        base.Update();

        var Input = ModuleManager.GetModule<InputModule>();
        if (Input == null)
            return;

        if (Input.IsKeyPressed(KeyboardKey.Escape))
        {
            cursorLocked = !cursorLocked;
            Input.CursorLocked = cursorLocked;
        }

        // Movement
        if (Input.IsKeyDown(KeyboardKey.W)) Transform.Position += Transform.Forward * moveSpeed * Time.Delta;
        if (Input.IsKeyDown(KeyboardKey.S)) Transform.Position -= Transform.Forward * moveSpeed * Time.Delta;
        if (Input.IsKeyDown(KeyboardKey.A)) Transform.Position -= Transform.Right * moveSpeed * Time.Delta;
        if (Input.IsKeyDown(KeyboardKey.D)) Transform.Position += Transform.Right * moveSpeed * Time.Delta;

        if (Input.IsKeyPressed(KeyboardKey.E))
            if (ModuleManager.GetModule<PhysicsModule>()?.World.TryRaycast(Transform.Position, Transform.Forward, 1f, out RaycastResult result) == true)
                EngineConsole.Log(result.Body.Owner?.Name ?? "null");

        if (!cursorLocked)
            return;

        // Mouse look
        Vector2 delta = Input.MouseDelta;

        yaw += delta.X * mouseSensitivity;
        pitch += -delta.Y * mouseSensitivity;

        pitch = Math.Math.Clamp(pitch, -1.55f, 1.55f);

        Quaternion yawRot = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);

        Vector3 right = Vector3.Transform(Vector3.Right, yawRot);
        Quaternion pitchRot = Quaternion.CreateFromAxisAngle(right, pitch);

        Transform.Rotation = yawRot * pitchRot;
    }
}
