using Orama.Core.Common.Components;
using Orama.Core.Common.Utility;
using Orama.Core.Modules;
using Orama.Core.Modules.Input;
using Orama.Core.Modules.Physics;
using Orama.Math;

namespace Orama.Core.Common.Entities;

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

        if (Input.IsKeyPressed(Key.Escape))
        {
            cursorLocked = !cursorLocked;
            Input.CursorLocked = cursorLocked;
        }

        // Movement
        if (Input.IsKeyDown(Key.W)) Transform.Position += Transform.Forward * moveSpeed * Time.Delta;
        if (Input.IsKeyDown(Key.S)) Transform.Position -= Transform.Forward * moveSpeed * Time.Delta;
        if (Input.IsKeyDown(Key.A)) Transform.Position -= Transform.Right * moveSpeed * Time.Delta;
        if (Input.IsKeyDown(Key.D)) Transform.Position += Transform.Right * moveSpeed * Time.Delta;

        if (Input.IsKeyPressed(Key.E))
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
