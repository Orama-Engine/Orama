
namespace Orama.Core.Modules.Physics;

/// <summary>
/// Represents a simulated physics world.
/// </summary>
public interface IPhysicsWorld
{
    /// <summary> Creates a new physics body and adds it to the physics world. </summary>
    /// <returns> The created physics body. </returns>
    IPhysicsBody CreateBody();

    /// <summary> Destroys a physics body and removes it from the physics world. </summary>
    /// <param name="body"> The physics body to remove. </param>
    void DestroyBody(IPhysicsBody body);

    /// <summary> Steps the physics simulation forward by a given time delta. </summary>
    /// <param name="delta"> The time in seconds to advance the simulation by. </param>
    void Step(float delta);
}
