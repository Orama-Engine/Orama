using Orama.Core.Common;
using Orama.Math;
using Silk.NET.Input;

namespace Orama.Core.Modules.Input;

/// <summary>
/// Module responsible for handling user input.
/// </summary>
public class InputModule : BaseModule
{
    /// <summary> The current mouse position. </summary>
    public Vector2 MousePosition => input.Mice[0].Position;

    /// <summary> The change in mouse position since the last update. </summary>
    public Vector2 MouseDelta { get; private set; }
    private Vector2 previousMousePosition;

    private IInputContext input = null!;

    #region Silk Mappings
    private static readonly Dictionary<Key, Silk.NET.Input.Key> KeyMap = new()
    {
        { Key.A, Silk.NET.Input.Key.A },
        { Key.B, Silk.NET.Input.Key.B },
        { Key.C, Silk.NET.Input.Key.C },
        { Key.D, Silk.NET.Input.Key.D },
        { Key.E, Silk.NET.Input.Key.E },
        { Key.F, Silk.NET.Input.Key.F },
        { Key.G, Silk.NET.Input.Key.G },
        { Key.H, Silk.NET.Input.Key.H },
        { Key.I, Silk.NET.Input.Key.I },
        { Key.J, Silk.NET.Input.Key.J },
        { Key.K, Silk.NET.Input.Key.K },
        { Key.L, Silk.NET.Input.Key.L },
        { Key.M, Silk.NET.Input.Key.M },
        { Key.N, Silk.NET.Input.Key.N },
        { Key.O, Silk.NET.Input.Key.O },
        { Key.P, Silk.NET.Input.Key.P },
        { Key.Q, Silk.NET.Input.Key.Q },
        { Key.R, Silk.NET.Input.Key.R },
        { Key.S, Silk.NET.Input.Key.S },
        { Key.T, Silk.NET.Input.Key.T },
        { Key.U, Silk.NET.Input.Key.U },
        { Key.V, Silk.NET.Input.Key.V },
        { Key.W, Silk.NET.Input.Key.W },
        { Key.X, Silk.NET.Input.Key.X },
        { Key.Y, Silk.NET.Input.Key.Y },
        { Key.Z, Silk.NET.Input.Key.Z },
        { Key.Space, Silk.NET.Input.Key.Space },
        { Key.Enter, Silk.NET.Input.Key.Enter },
        { Key.Backspace, Silk.NET.Input.Key.Backspace }
    };
    #endregion


    public override void Initialize()
    {
        input = Application.Window.InternalWindow.CreateInput();
    }

    public override void Update()
    {
        var currentMousePosition = MousePosition;
        MouseDelta = currentMousePosition - previousMousePosition;
        previousMousePosition = currentMousePosition;
    }

    /// <summary> Checks if the specified mouse button is currently pressed. </summary>
    /// <param name="button"> The mouse button to check. </param>
    public bool IsMouseButtonDown(int button) => input.Mice[0].IsButtonPressed((MouseButton)button);

    /// <summary> Checks if the specified key is currently pressed. </summary>
    /// <param name="key"> The key to check. </param>
    public bool IsKeyDown(Key key) => input.Keyboards.Any(kb => kb.IsKeyPressed(KeyMap[key]));
}
