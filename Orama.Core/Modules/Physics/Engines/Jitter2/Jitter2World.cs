using Jitter2;

namespace Orama.Core.Modules.Physics.Engines.Jitter2;

/// <summary>
/// Represents a Jitter2 physics world.
/// </summary>
public class Jitter2World : IPhysicsWorld
{
    private readonly World world;

    /// <summary> Creates a new Jitter2 physics world. </summary>
    public Jitter2World()
    {
        world = new World();
        world.SubstepCount = 4;
    }

    /// <inheritdoc/>
    public IPhysicsBody CreateBody()
    {
        var body = world.CreateRigidBody();
        return new Jitter2Body(body);
    }

    /// <inheritdoc/>
    public void DestroyBody(IPhysicsBody body)
    {
        if (body is Jitter2Body b && b.body != null)
            world.Remove(b.body);
    }

    /// <inheritdoc/>
    public void Step(float delta) => world.Step(delta, true);
}
