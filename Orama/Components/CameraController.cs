using Orama.UserInput;
using System.Numerics;

namespace Orama.Components;

public class CameraController : Camera
{
	public float Speed = 0.25f;
	public float RotationSpeed = 0.005f;

	private float yaw = 0f;
	private float pitch = 0f;

	public override void Update()
	{
		HandleMovement();
		HandleRotation();

		if (Input.KeyPressed(Key.Q))
			Console.WriteLine("Camera position: " + Transform.Position);
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
		// Update pitch
		if (Input.IsKeyDown(Key.Up))
			pitch += RotationSpeed;

		if (Input.IsKeyDown(Key.Down))
			pitch -= RotationSpeed;

		pitch = Math.Clamp(pitch, -MathF.PI / 2 + 0.01f, MathF.PI / 2 - 0.01f);

		// Update yaw
		if (Input.IsKeyDown(Key.Left))
			yaw += RotationSpeed;

		if (Input.IsKeyDown(Key.Right))
			yaw -= RotationSpeed;

		// Apply rotation
		var yawQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw);
		var pitchQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitX, pitch);

		Transform.Rotation = yawQuat * pitchQuat;
	}

}
