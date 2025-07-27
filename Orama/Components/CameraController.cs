using Orama.InputManagement;

namespace Orama.Components;

public class CameraController : Camera
{
	public override void Update()
	{
		if (Input.IsKeyDown(Key.A))
			Console.WriteLine("A pressed");
	}
}
