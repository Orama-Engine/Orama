using Orama.Core.Common.Components;
using Orama.Core.Common.Utility;
using Orama.Core.Modules;
using Orama.Core.Modules.Input;
using Orama.Math;

namespace Orama.Core.Common.Entities;

internal class FlyController : Entity
{
    [ImplicitComponent] public Camera Camera { get; set; } = null!;

    private float mouseSensitivity = 0.25f;

    private float pitch;
    private float yaw;

    public override void Update()
    {
        base.Update();

        var Input = ModuleManager.GetOrRegisterModule<InputModule>();

        // Movement
        if (Input.IsKeyDown(Key.W)) Transform.Position += Transform.Forward * 0.1f;
        if (Input.IsKeyDown(Key.S)) Transform.Position -= Transform.Forward * 0.1f;
        if (Input.IsKeyDown(Key.A)) Transform.Position -= Transform.Right * 0.1f;
        if (Input.IsKeyDown(Key.D)) Transform.Position += Transform.Right * 0.1f;
        if (Input.IsKeyDown(Key.Q)) Transform.Position -= Transform.Up * 0.1f;
        if (Input.IsKeyDown(Key.E)) Transform.Position += Transform.Up * 0.1f;

        // Mouse look
        Vector2 delta = Input.MouseDelta;

        // Update cumulative yaw/pitch
        yaw += -delta.X * mouseSensitivity * Time.Delta;
        pitch += -delta.Y * mouseSensitivity * Time.Delta;

        // Build rotation from cumulative angles
        Quaternion yawRot = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);
        Quaternion pitchRot = Quaternion.CreateFromAxisAngle(Vector3.Right, pitch);
        Transform.Rotation = yawRot * pitchRot; // order: yaw then pitch

        if (Input.IsKeyPressed(Key.Space))
            EngineOutput.Log($"Position: {Transform.Position}, Rotation: {Transform.Rotation}");
    }
}
