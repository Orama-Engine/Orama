using Orama.Core.Common;
using Orama.Core.Common.Utility;
using Orama.Math;
using Silk.NET.Input;
using System.Diagnostics;

namespace Orama.Core.Modules.Input;

/// <summary>
/// Module responsible for handling user input.
/// </summary>
public class InputModule : BaseModule
{
    /// <summary> Invoked when a mouse button is pressed. </summary>
    public Action<MouseButton, Vector2>? OnMouseClick { get; set; }

    /// <summary> The current mouse position. </summary>
    public Vector2 MousePosition => input.Mice[0].Position;

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
        { Key.Backspace, Silk.NET.Input.Key.Backspace }
    };

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
                OnMouseClick?.Invoke(localButton, mouse.Position);
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

        var currentMousePosition = MousePosition;
        MouseDelta = currentMousePosition - previousMousePosition;
        previousMousePosition = currentMousePosition;

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
