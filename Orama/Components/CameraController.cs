using Orama.Echo;
using Orama.Modules.Input;
using System.Numerics;

namespace Orama.Components;

public class CameraController : Camera
{
	[Serialize] public float Speed { get; set; } = 0.005f;
	[Serialize] public float RotationSpeed { get; set; } = 0.001f;

	private InputModule inputModule => Application.ModuleManager.GetModule<InputModule>()
	                                       ?? throw new InvalidOperationException("InputModule must exist and be initialized.");

	private float yaw = 0f;
	private float pitch = 0f;

	public override void Update()
	{
		// Adjust movement speed using scroll wheel
		if (inputModule.ScrollDelta != 0f)
		{
			Speed += inputModule.ScrollDelta * 0.005f;

			// Clamp to prevent negative speed
			Speed = MathF.Max(Speed, 0.01f);
		}
		
		HandleMovement();
		HandleRotation();
	}

	private void HandleMovement()
	{
		if (inputModule.IsKeyDown(Key.A))
			Transform.Position += -Transform.Right * Speed;

		if (inputModule.IsKeyDown(Key.D))
			Transform.Position += Transform.Right * Speed;

		if (inputModule.IsKeyDown(Key.W))
			Transform.Position += Transform.Forward * Speed;

		if (inputModule.IsKeyDown(Key.S))
			Transform.Position += -Transform.Forward * Speed;
	}

	private void HandleRotation()
	{
		if (inputModule.IsMouseButtonDown(MouseButton.Right))
		{
			inputModule.CursorVisible = false;
			inputModule.CursorLocked = true;

			// Use mouse delta to update yaw and pitch
			yaw += -inputModule.MouseDelta.X * RotationSpeed;
			pitch += -inputModule.MouseDelta.Y * RotationSpeed;

			// Clamp pitch to avoid flipping
			pitch = Math.Clamp(pitch, -MathF.PI / 2 + 0.01f, MathF.PI / 2 - 0.01f);

			// Create quaternions from yaw and pitch
			var yawQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw);
			var pitchQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitX, pitch);

			// Apply combined rotation
			Transform.Rotation = yawQuat * pitchQuat;
		} else
		{
			inputModule.CursorVisible = true;
			inputModule.CursorLocked = false;
		}
	}
}
