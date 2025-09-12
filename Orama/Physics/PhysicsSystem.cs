using BulletSharp;
using BulletSharp.Math;

namespace Orama.Physics;

/// <summary>
/// Singleton class that manages the Bullet physics world.
/// </summary>
public static class PhysicsSystem
{
	/// <summary>
	/// The Bullet physics world.
	/// </summary>
	public static DiscreteDynamicsWorld World { get; private set; }
	
	private static CollisionConfiguration _collisionConfig;
	private static CollisionDispatcher _dispatcher;
	private static BroadphaseInterface _broadphase;
	private static SequentialImpulseConstraintSolver _solver;
	
	/// <summary>
	/// Creates and initialises the physics system instance.
	/// </summary>
	public static void Initialize()
	{
		try
		{
			_collisionConfig = new DefaultCollisionConfiguration();
			_dispatcher = new CollisionDispatcher(_collisionConfig);
			_broadphase = new DbvtBroadphase();
			_solver = new SequentialImpulseConstraintSolver();

			World = new DiscreteDynamicsWorld(_dispatcher, _broadphase, _solver, _collisionConfig)
			{
				Gravity = new Vector3(0, -9.81f, 0)
			};
		}
		catch (Exception e)
		{
			Console.WriteLine("PhysicsSystem creation failed: " + e);
			throw;
		}
	}

	/// <summary>
	/// Ticks the physics simulation forward by a fixed time step.
	/// </summary>
	public static void Step() => World.StepSimulation(1f / 60f, maxSubSteps: 10, fixedTimeStep: 1f / 60f);
}