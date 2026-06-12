using Orama.Core.Common;
using Orama.Core.Modules.Physics.Engines.Jitter2;

namespace Orama.Core.Modules.Physics;

/// <summary>
/// Module responsible for handling the physics simulation.
/// </summary>
public class PhysicsModule : BaseModule
{
    /// <summary>
    /// The physics world.
    /// </summary>
    public IPhysicsWorld World { get; }

    /// <summary> Creates a new physics module with the given physics world. </summary>
    /// <param name="world"> The physics world to use. </param>
    public PhysicsModule(IPhysicsWorld world)
    {
        World = world;
    }

    /// <summary> Creates a new physics module with the default physics world. </summary>
    public PhysicsModule()
    {
        World = new Jitter2World();
    }

    /// <inheritdoc/>
    public override void Update() => World.Step(Time.FixedDelta);
}
