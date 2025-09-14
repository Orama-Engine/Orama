using Orama.Echo;
using Orama.Modules.Input;
using System.Numerics;
using Orama.Modules;

namespace Orama.Components;

public class CameraController : Camera
{
	[Serialize] public float Speed { get; set; } = 0.005f;
	[Serialize] public float RotationSpeed { get; set; } = 0.001f;
	
	private float yaw = 0f;
	private float pitch = 0f;

	public override void Update()
	{
		// Adjust movement speed using scroll wheel
		if (ModuleManager.GetModule<InputModule>().ScrollDelta != 0f)
		{
			Speed += ModuleManager.GetModule<InputModule>().ScrollDelta * 0.005f;

			// Clamp to prevent negative speed
			Speed = MathF.Max(Speed, 0.01f);
		}
		
		HandleMovement();
		HandleRotation();
	}

	private void HandleMovement()
	{
		if (ModuleManager.GetModule<InputModule>().IsKeyDown(Key.A))
			Transform.Position += -Transform.Right * Speed;

		if (ModuleManager.GetModule<InputModule>().IsKeyDown(Key.D))
			Transform.Position += Transform.Right * Speed;

		if (ModuleManager.GetModule<InputModule>().IsKeyDown(Key.W))
			Transform.Position += Transform.Forward * Speed;

		if (ModuleManager.GetModule<InputModule>().IsKeyDown(Key.S))
			Transform.Position += -Transform.Forward * Speed;
	}

	private void HandleRotation()
	{
		if (ModuleManager.GetModule<InputModule>().IsMouseButtonDown(MouseButton.Right))
		{
			ModuleManager.GetModule<InputModule>().CursorVisible = false;
			ModuleManager.GetModule<InputModule>().CursorLocked = true;

			// Use mouse delta to update yaw and pitch
			yaw += -ModuleManager.GetModule<InputModule>().MouseDelta.X * RotationSpeed;
			pitch += -ModuleManager.GetModule<InputModule>().MouseDelta.Y * RotationSpeed;

			// Clamp pitch to avoid flipping
			pitch = Math.Clamp(pitch, -MathF.PI / 2 + 0.01f, MathF.PI / 2 - 0.01f);

			// Create quaternions from yaw and pitch
			var yawQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw);
			var pitchQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitX, pitch);

			// Apply combined rotation
			Transform.Rotation = yawQuat * pitchQuat;
		} else
		{
			ModuleManager.GetModule<InputModule>().CursorVisible = true;
			ModuleManager.GetModule<InputModule>().CursorLocked = false;
		}
	}
}
