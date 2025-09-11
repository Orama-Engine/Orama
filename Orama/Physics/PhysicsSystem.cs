using BulletSharp;
using BulletSharp.Math;

namespace Orama.Physics;

/// <summary>
/// Singleton class that manages the Bullet physics world.
/// </summary>
public class PhysicsSystem
{
	private static PhysicsSystem _instance;
	
	/// <summary>
	/// Gets the singleton instance of the physics system.
	/// </summary>
	public static PhysicsSystem Instance => _instance ??= CreateInstance();
	
	/// <summary>
	/// The Bullet physics world.
	/// </summary>
	public DiscreteDynamicsWorld World { get; private set; }
	
	private CollisionConfiguration _collisionConfig;
	private CollisionDispatcher _dispatcher;
	private BroadphaseInterface _broadphase;
	private SequentialImpulseConstraintSolver _solver;
	
	/// <summary>
	/// Creates and initialises the physics system instance.
	/// </summary>
	/// <returns></returns>
	private static PhysicsSystem CreateInstance()
	{
		try
		{
			var ps = new PhysicsSystem();

			ps._collisionConfig = new DefaultCollisionConfiguration();
			ps._dispatcher = new CollisionDispatcher(ps._collisionConfig);
			ps._broadphase = new DbvtBroadphase();
			ps._solver = new SequentialImpulseConstraintSolver();

			ps.World = new DiscreteDynamicsWorld(ps._dispatcher, ps._broadphase, ps._solver, ps._collisionConfig)
			{
				Gravity = new Vector3(0, -9.81f, 0)
			};

			return ps;
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
	public void Step() => World.StepSimulation(1f / 60f, maxSubSteps: 10, fixedTimeStep: 1f / 60f);
}