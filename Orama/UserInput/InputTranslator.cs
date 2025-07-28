namespace Orama.UserInput;

// Currently requires cleanup.
public static class InputTranslator
{
	/// <summary>
	/// Translates a Veldrid key to an Orama Engine Key.
	/// </summary>
	/// <param name="veldridKey"></param>
	/// <returns></returns>
	public static Key ToEngineKey(this Veldrid.Key veldridKey)
	{
		return veldridKey switch
		{
			Veldrid.Key.F1 => Key.F1,
			Veldrid.Key.F2 => Key.F2,
			Veldrid.Key.F3 => Key.F3,
			Veldrid.Key.F4 => Key.F4,
			Veldrid.Key.F5 => Key.F5,
			Veldrid.Key.F6 => Key.F6,
			Veldrid.Key.F7 => Key.F7,
			Veldrid.Key.F8 => Key.F8,
			Veldrid.Key.F9 => Key.F9,
			Veldrid.Key.F10 => Key.F10,
			Veldrid.Key.F11 => Key.F11,
			Veldrid.Key.F12 => Key.F12,
			Veldrid.Key.A => Key.A,
			Veldrid.Key.B => Key.B,
			Veldrid.Key.C => Key.C,
			Veldrid.Key.D => Key.D,
			Veldrid.Key.E => Key.E,
			Veldrid.Key.F => Key.F,
			Veldrid.Key.G => Key.G,
			Veldrid.Key.H => Key.H,
			Veldrid.Key.I => Key.I,
			Veldrid.Key.J => Key.J,
			Veldrid.Key.K => Key.K,
			Veldrid.Key.L => Key.L,
			Veldrid.Key.M => Key.M,
			Veldrid.Key.N => Key.N,
			Veldrid.Key.O => Key.O,
			Veldrid.Key.P => Key.P,
			Veldrid.Key.Q => Key.Q,
			Veldrid.Key.R => Key.R,
			Veldrid.Key.S => Key.S,
			Veldrid.Key.T => Key.T,
			Veldrid.Key.U => Key.U,
			Veldrid.Key.V => Key.V,
			Veldrid.Key.W => Key.W,
			Veldrid.Key.X => Key.X,
			Veldrid.Key.Y => Key.Y,
			Veldrid.Key.Z => Key.Z,
			Veldrid.Key.Number1 => Key.One,
			Veldrid.Key.Number2 => Key.Two,
			Veldrid.Key.Number3 => Key.Three,
			Veldrid.Key.Number4 => Key.Four,
			Veldrid.Key.Number5 => Key.Five,
			Veldrid.Key.Number6 => Key.Six,
			Veldrid.Key.Number7 => Key.Seven,
			Veldrid.Key.Number8 => Key.Eight,
			Veldrid.Key.Number9 => Key.Nine,
			Veldrid.Key.Number0 => Key.Zero,
			Veldrid.Key.Keypad0 => Key.NumpadZero,
			Veldrid.Key.Keypad1 => Key.NumpadOne,
			Veldrid.Key.Keypad2 => Key.NumpadTwo,
			Veldrid.Key.Keypad3 => Key.NumpadThree,
			Veldrid.Key.Keypad4 => Key.NumpadFour,
			Veldrid.Key.Keypad5 => Key.NumpadFive,
			Veldrid.Key.Keypad6 => Key.NumpadSix,
			Veldrid.Key.Keypad7 => Key.NumpadSeven,
			Veldrid.Key.Keypad8 => Key.NumpadEight,
			Veldrid.Key.Keypad9 => Key.NumpadNine,
			Veldrid.Key.Comma => Key.Comma,
			Veldrid.Key.Period => Key.Period,
			Veldrid.Key.Slash => Key.Slash,
			Veldrid.Key.BackSlash => Key.Backslash,
			Veldrid.Key.Semicolon => Key.Semicolon,
			Veldrid.Key.Quote => Key.Quote,
			Veldrid.Key.BracketLeft => Key.BracketL,
			Veldrid.Key.BracketRight => Key.BracketR,
			Veldrid.Key.Minus => Key.Minus,
			Veldrid.Key.Grave => Key.Grave,
			Veldrid.Key.Plus => Key.Plus,
			Veldrid.Key.Up => Key.Up,
			Veldrid.Key.Down => Key.Down,
			Veldrid.Key.Left => Key.Left,
			Veldrid.Key.Right => Key.Right,
			Veldrid.Key.ShiftLeft => Key.ShiftL,
			Veldrid.Key.ShiftRight => Key.ShiftR,
			Veldrid.Key.ControlLeft => Key.CtrlL,
			Veldrid.Key.ControlRight => Key.CtrlR,
			Veldrid.Key.AltLeft => Key.AltL,
			Veldrid.Key.AltRight => Key.AltR,
			Veldrid.Key.Enter => Key.Enter,
			Veldrid.Key.Space => Key.Space,
			Veldrid.Key.Escape => Key.Escape,
			_ => Key.Unknown
		};
	}

	public static MouseButton ToEngineMouseButton(this Veldrid.MouseButton veldridMouseButton)
	{
		return veldridMouseButton switch
		{
			Veldrid.MouseButton.Left => MouseButton.Left,
			Veldrid.MouseButton.Right => MouseButton.Right,
			Veldrid.MouseButton.Middle => MouseButton.Middle,
			Veldrid.MouseButton.Button4 => MouseButton.Button4,
			Veldrid.MouseButton.Button5 => MouseButton.Button5,
			Veldrid.MouseButton.Button6 => MouseButton.Button6,
			Veldrid.MouseButton.Button7 => MouseButton.Button7,
			Veldrid.MouseButton.Button8 => MouseButton.Button8,
		};
	}
}