using Orama.Core.Common.Components;
using Orama.Core.Modules;
using Orama.Core.Modules.Input;

namespace Orama.Core.Common.Entities;

internal class FPSController : BaseEntity
{
    public Camera Camera { get; set; } = null!;

    public override void Start()
    {
        Camera = AddComponent<Camera>();
    }

    public override void Update()
    {
        InputModule inputModule = ModuleManager.GetOrRegisterModule<InputModule>();

        if (inputModule.IsKeyDown(Key.W))
            Transform.Position += Transform.Forward * 0.1f;

        if (inputModule.IsKeyDown(Key.S))
            Transform.Position -= Transform.Forward * 0.1f;

        if (inputModule.IsKeyDown(Key.A))
            Transform.Position -= Transform.Right * 0.1f;

        if (inputModule.IsKeyDown(Key.D))
            Transform.Position += Transform.Right * 0.1f;
    }
}
