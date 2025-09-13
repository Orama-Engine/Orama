using BulletSharp;
using BulletSharp.Math;

namespace Orama.Modules.Physics;

/// <summary>
/// Access into the worlds Physics.
/// </summary>
public class PhysicsModule : Module
{
	/// <summary>
	/// The Bullet physics world.
	/// </summary>
	public DiscreteDynamicsWorld World { get; private set; }
	
	private CollisionConfiguration _collisionConfig;
	private CollisionDispatcher _dispatcher;
	private BroadphaseInterface _broadphase;
	private SequentialImpulseConstraintSolver _solver;

	public override void Start()
	{
		_collisionConfig = new DefaultCollisionConfiguration();
		_dispatcher = new CollisionDispatcher(_collisionConfig);
		_broadphase = new DbvtBroadphase();
		_solver = new SequentialImpulseConstraintSolver();

		World = new DiscreteDynamicsWorld(_dispatcher, _broadphase, _solver, _collisionConfig)
		{
			Gravity = new BulletSharp.Math.Vector3(0, -9.81f, 0)
		};
	}

	public override void Update()
	{
		if (World == null) return;
		World.StepSimulation(1f / 60f, maxSubSteps: 10, fixedTimeStep: 1f / 60f);
	}
}