using Orama.Echo;
using Orama.UserInput;
using System.Numerics;

namespace Orama.Components;

public class CameraController : Camera
{
	[SerializeProperty] public float Speed { get; set; } = 0.005f;
	[SerializeProperty] public float RotationSpeed { get; set; } = 0.001f;

	private float yaw = 0f;
	private float pitch = 0f;

	public override void Update()
	{
		// Adjust movement speed using scroll wheel
		if (Input.ScrollDelta != 0f)
		{
			Speed += Input.ScrollDelta * 0.05f;

			// Clamp to prevent negative speed
			Speed = MathF.Max(Speed, 0.01f);
		}
		
		HandleMovement();
		HandleRotation();

		if (Input.KeyPressed(Key.Q))
		{
			Console.WriteLine("Camera position: " + Transform.Position);
			Console.WriteLine("Camera rotation: " + Transform.Rotation);
			Console.WriteLine("Mouse Position: " + Input.MousePosition);
		}
	}

	private void HandleMovement()
	{
		if (Input.IsKeyDown(Key.A))
			Transform.Position += -Transform.Right * Speed;

		if (Input.IsKeyDown(Key.D))
			Transform.Position += Transform.Right * Speed;

		if (Input.IsKeyDown(Key.W))
			Transform.Position += Transform.Forward * Speed;

		if (Input.IsKeyDown(Key.S))
			Transform.Position += -Transform.Forward * Speed;
	}

	private void HandleRotation()
	{
		if (Input.IsMouseButtonDown(MouseButton.Right))
		{
			Input.CursorVisible = false;
			Input.CursorLocked = true;

			// Use mouse delta to update yaw and pitch
			yaw += -Input.MouseDelta.X * RotationSpeed;
			pitch += -Input.MouseDelta.Y * RotationSpeed;

			// Clamp pitch to avoid flipping
			pitch = Math.Clamp(pitch, -MathF.PI / 2 + 0.01f, MathF.PI / 2 - 0.01f);

			// Create quaternions from yaw and pitch
			var yawQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw);
			var pitchQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -pitch);

			// Apply combined rotation
			Transform.Rotation = yawQuat * pitchQuat;
		} else
		{
			Input.CursorVisible = true;
			Input.CursorLocked = false;
		}
	}
}
