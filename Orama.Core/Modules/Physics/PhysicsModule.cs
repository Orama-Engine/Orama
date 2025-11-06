
using Jitter2;
using Orama.Core.Common;
using Orama.Core.Common.Utility;

namespace Orama.Core.Modules.Physics;

/// <summary>
/// Module responsible for handling the world physics.
/// </summary>
public class PhysicsModule : BaseModule
{
    /// <summary> Gets the physics simulation world. </summary>
    public World? World { get; private set; }

    public override void Initialize()
    {
        World = new World();
        World.SubstepCount = 4; // Default substep count
    }

    public override void Update()
    {
        World?.Step((float)Time.PreciseDelta, true);
    }
}
