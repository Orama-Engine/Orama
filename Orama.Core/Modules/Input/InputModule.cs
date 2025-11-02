using Orama.Core.Common;
using Orama.Math;
using Silk.NET.Input;

namespace Orama.Core.Modules.Input;

/// <summary>
/// Module responsible for handling user input.
/// </summary>
public class InputModule : BaseModule
{
    /// <summary> Invoked when a mouse button is pressed. </summary>
    public event Action<MouseButton, Vector2>? MouseClicked;

    /// <summary> Invoked when a mouse button is released. </summary>
    public event Action<MouseButton, Vector2>? MouseReleased;

    /// <summary> Invoked when the mouse moves. </summary>
    public event Action<Vector2>? MouseMoved;
    
    /// <summary> Invoked when a key is pressed. </summary>
    public event Action<Key>? KeyPressed;

    /// <summary> The current mouse position. </summary>
    public Vector2 MousePosition { get { return input.Mice[0].Position; } set { input.Mice[0].Position = value; } }

    /// <summary> Whether the cursor is locked to the window. </summary>
    public bool CursorLocked { get { return input.Mice[0].Cursor.CursorMode == CursorMode.Raw && input.Mice[0].Cursor.IsConfined; } set { input.Mice[0].Cursor.CursorMode = value ? CursorMode.Raw : CursorMode.Normal; input.Mice[0].Cursor.IsConfined = value; } }

    /// <summary> The change in mouse position since the last update. </summary>
    public Vector2 MouseDelta { get; private set; }
    private Vector2 previousMousePosition;

    private IInputContext input = null!;

    private Dictionary<Silk.NET.Input.Key, bool> previousKeys = new();

    #region Silk Mappings
    private static readonly Dictionary<Key, Silk.NET.Input.Key> keyMap = new()
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
        { Key.Backspace, Silk.NET.Input.Key.Backspace },
        { Key.Escape, Silk.NET.Input.Key.Escape }
    };

    private static readonly Dictionary<Silk.NET.Input.Key, Key> keyMapInverse = keyMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    private static readonly Dictionary<MouseButton, Silk.NET.Input.MouseButton> mouseButtonMap = new()
    {
        { MouseButton.Left, Silk.NET.Input.MouseButton.Left },
        { MouseButton.Right, Silk.NET.Input.MouseButton.Right },
        { MouseButton.Middle, Silk.NET.Input.MouseButton.Middle }
    };

    private static readonly Dictionary<Silk.NET.Input.MouseButton, MouseButton> mouseButtonMapInverse = mouseButtonMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
    #endregion


    public override void Initialize()
    {
        input = Application.Window.InternalWindow.CreateInput();

        // Bind Events
        input.Mice[0].MouseDown += (mouse, button) =>
        {
            if (mouseButtonMapInverse.TryGetValue(button, out var localButton))
                MouseClicked?.Invoke(localButton, mouse.Position);
        };

        input.Mice[0].MouseUp += (mouse, button) =>
        {
            if (mouseButtonMapInverse.TryGetValue(button, out var localButton))
                MouseReleased?.Invoke(localButton, mouse.Position);
        };

        input.Mice[0].MouseMove += (mouse, position) =>
        {
            MouseMoved?.Invoke(position);
        };

        input.Keyboards[0].KeyDown += (kb, key, i) =>
        {
            if (keyMapInverse.TryGetValue(key, out var localKey))
                KeyPressed?.Invoke(localKey);
        };

        foreach (var key in keyMap.Values)
            previousKeys[key] = false;
    }

    public override void Update()
    {
        // Update previous key states for all keyboards
        foreach (var kb in input.Keyboards)
            foreach (var key in keyMap.Values)
                previousKeys[key] = kb.IsKeyPressed(key);

        MouseDelta = MousePosition - previousMousePosition;
        previousMousePosition = MousePosition;

    }

    /// <summary> Checks if the specified mouse button is currently pressed. </summary>
    /// <param name="button"> The mouse button to check. </param>
    public bool IsMouseButtonDown(MouseButton button) => input.Mice[0].IsButtonPressed(mouseButtonMap[button]);

    /// <summary> Checks if the specified key is currently pressed. </summary>
    /// <param name="key"> The key to check. </param>
    public bool IsKeyDown(Key key) => input.Keyboards.Any(kb => kb.IsKeyPressed(keyMap[key]));

    /// <summary> Checks if the specified key was pressed this frame. </summary>
    /// <param name="key"> The key to check. </param>
    public bool IsKeyPressed(Key key)
    {
        Silk.NET.Input.Key mappedKey = keyMap[key];

        foreach (var kb in input.Keyboards)
        {
            bool isDown = kb.IsKeyPressed(mappedKey);
            bool wasDown = previousKeys[mappedKey];

            if (isDown && !wasDown)
                return true;
        }

        return false;
    }
}
