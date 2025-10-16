using Orama.Core.Common.Components;
using Orama.Core.Modules;
using Orama.Core.Modules.Input;

namespace Orama.Core.Common.Entities;

internal class FlyController : Entity
{
    [ImplicitComponent] public Camera Camera { get; set; } = null!;

    public override void Update()
    {
        base.Update();

        var Input = ModuleManager.GetOrRegisterModule<InputModule>();

        if (Input.IsKeyDown(Key.W)) Transform.Position += Transform.Forward * 0.1f;
        if (Input.IsKeyDown(Key.S)) Transform.Position -= Transform.Forward * 0.1f;
        if (Input.IsKeyDown(Key.A)) Transform.Position -= Transform.Right * 0.1f;
        if (Input.IsKeyDown(Key.D)) Transform.Position += Transform.Right * 0.1f;
        if (Input.IsKeyDown(Key.Q)) Transform.Position -= Transform.Up * 0.1f;
        if (Input.IsKeyDown(Key.E)) Transform.Position += Transform.Up * 0.1f;
    }
}
