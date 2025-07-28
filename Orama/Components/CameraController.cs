using Orama.UserInput;

namespace Orama.Components;

public class CameraController : Camera
{
	public float Speed = 0.25f;

	public override void Update()
	{
		if (Input.IsKeyDown(Key.A))
			Transform.Position += -Transform.Right * Speed;

		if (Input.IsKeyDown(Key.D))
			Transform.Position += Transform.Right * Speed;

		if (Input.IsKeyDown(Key.W))
			Transform.Position += Transform.Forward * Speed;

		if (Input.IsKeyDown(Key.S))
			Transform.Position += -Transform.Forward * Speed;

		if (Input.IsKeyPressed(Key.Q))
			Console.WriteLine("Camera position: " + Transform.Position);
	}

}
