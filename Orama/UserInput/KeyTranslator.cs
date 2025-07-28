namespace Orama.UserInput;

public static class KeyTranslator
{
	public static Key ToEngineKey(this Veldrid.Key veldridKey)
	{
		return veldridKey switch
		{
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
			Veldrid.Key.Up => Key.Up,
			Veldrid.Key.Down => Key.Down,
			Veldrid.Key.Left => Key.Left,
			Veldrid.Key.Right => Key.Right,
			Veldrid.Key.Space => Key.Space,
			Veldrid.Key.Escape => Key.Escape,
			_ => Key.Unknown
		};
	}
}