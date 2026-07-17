// This file is part of the Orama Game Engine.
// Licensed under the MIT license. (https://github.com/Orama-Engine/Orama/blob/main/LICENSE)

using Orama.Common;
using Orama.Common.Utility;
using Orama.Input;
using Orama.Input.Devices;
using Orama.Math;
using Orama.Rendering.Components;
using Orama.Scenes.Entities;

namespace Orama.Rendering.Entities;

[Entity("fly_controller")]
public class FlyController : Entity
{
	[ImplicitComponent] public Camera Camera { get; set; } = null!;

	private readonly float mouseSensitivity = 0.0025f;
	private readonly float moveSpeed = 8.0f;

	private float pitch;
	private float yaw;

	bool cursorLocked = false;

	public override void Update()
	{
		base.Update();

		var Input = ModuleManager.GetModule<InputModule>();
		if (Input == null)
			return;

		if (Input.PrimaryKeyboard?.IsKeyPressed(Keyboard.Key.Escape) ?? false)
		{
			cursorLocked = !cursorLocked;
			Input.PrimaryMouse?.CursorLocked = cursorLocked;
		}

		if (Input.PrimaryGamepad?.IsButtonPressed(Gamepad.Button.ActionLeft) ?? false)
			OramaConsole.Log("Action left pressed.");

		// Movement
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.W) ?? false)
			Transform.Position += Transform.Forward * moveSpeed * Time.Delta;
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.S) ?? false)
			Transform.Position -= Transform.Forward * moveSpeed * Time.Delta;
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.A) ?? false)
			Transform.Position -= Transform.Right * moveSpeed * Time.Delta;
		if (Input.PrimaryKeyboard?.IsKeyDown(Keyboard.Key.D) ?? false)
			Transform.Position += Transform.Right * moveSpeed * Time.Delta;

		if (!cursorLocked)
			return;

		// Mouse look
		Vector2 delta = Input.PrimaryMouse?.MouseDelta ?? Vector2.Zero;

		yaw += delta.X * mouseSensitivity;
		pitch += delta.Y * mouseSensitivity;

		pitch = Math.Math.Clamp(pitch, -1.55f, 1.55f);

		var yawRot = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);

		var right = Vector3.Transform(Vector3.Right, yawRot);
		var pitchRot = Quaternion.CreateFromAxisAngle(right, pitch);

		Transform.Rotation = pitchRot * yawRot;
	}
}
