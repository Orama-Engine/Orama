using Orama.Core.Common.Components;
using Orama.Core.Common.Utility;
using Orama.Core.Modules;
using Orama.Core.Modules.Input;
using Orama.Math;

namespace Orama.Core.Common.Entities;

internal class FlyController : Entity
{
    [ImplicitComponent] public Camera Camera { get; set; } = null!;

    private float mouseSensitivity = 0.1f;

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

        // Yaw (rotation around up axis)
        Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -delta.X * mouseSensitivity);

        // Pitch (rotation around right axis)
        Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitX, -delta.Y * mouseSensitivity);

        // Clamp pitch to avoid flipping
        Vector3 euler = Transform.Rotation.ToEulerAngles();
        euler.X = MathFEx.Clamp(euler.X, -MathF.PI / 2f, MathF.PI / 2f);
        Transform.Rotation = Quaternion.FromEulerAngles(euler);

        EngineOutput.Log($"Position: {Transform.Position}, Rotation: {Transform.Rotation}");
    }
}
