
using Jitter2;
using Orama.Core.Common;

namespace Orama.Core.Modules.Physics;

/// <summary>
/// Module responsible for handling the world physics.
/// </summary>
public class PhysicsModule : BaseModule
{
    /// <summary> Gets the physics simulation world. </summary>
    public World World { get; private set; } = new World();

    public override void Initialize()
    {
        World.SubstepCount = 4; // Default substep count
    }
    public override void Update()
    {
        World.Step((float)Time.PreciseDelta, true);
    }
}
