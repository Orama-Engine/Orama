using Orama.Core.Common;
using Orama.Modules;
using Orama.Physics.Engines.Jitter2;

namespace Orama.Physics;

/// <summary>
/// Module responsible for handling the physics simulation.
/// </summary>
public class PhysicsModule : BaseModule
{
    public IPhysicsWorld World { get; set; } = null!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        Application.OnUpdate += Update;

        World = new Jitter2World();
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        base.Dispose();

        Application.OnUpdate -= Update;
    }

    /// <inheritdoc/>
    public void Update() => World.Step(Time.FixedDelta);
}
