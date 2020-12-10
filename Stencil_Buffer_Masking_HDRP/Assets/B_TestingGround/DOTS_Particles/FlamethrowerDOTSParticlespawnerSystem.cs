using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Extensions;
using UnityEngine;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Random = Unity.Mathematics.Random;

[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class FlamethrowerDOTSParticlespawnerSystem : ComponentSystem
{
	public Entity particle;

	private Random random;
	
	BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
	protected override void OnUpdate()
	{
		SpawnParticles();
	}

	protected override void OnCreate()
	{
		random = new Random(126);
		m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
	}

	[BurstCompile]
	public void SpawnParticles()
	{
		var ecBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

		/*Entities
			.ForEach((ref Translation tx, ref Rotation parentRot, in FlamethrowerSpawner ftSpawner) =>
		{
			// Entity spawnedParticle = EntityManager.Instantiate(ftSpawner.flamethrowerParticleEntity);
			Debug.Log("caw");
			
			//Add a Scale component to the particles (not on the 
			EntityManager.AddComponentData(spawnedParticle,
				new Scale
				{
					Value = 0.1f
				});*/
			
			//Set the particle's position at the parent, flamethrower 'nozzle'
			ecBuffer.SetComponent(entityInQueryIndex, spawnedParticle,
				new Translation
				{
					Value = tx.Value
				});#1#
			
			
			
			// Calculate the velocity for each particle
			//var particleFanAmount = ftSpawner.particleFanAmount;

			// var initialParticleVel = math.forward(parentRot.Value);

			var randDirection = random.NextFloat3Direction();
				
			// var fannedParticleVel = new float3(random.NextFloat(initialParticleVel.x - particleFanAmount.x, initialParticleVel.x + particleFanAmount.x),
				// random.NextFloat(initialParticleVel.y - particleFanAmount.y, initialParticleVel.y + particleFanAmount.y),
				// random.NextFloat(initialParticleVel.z - particleFanAmount.z, initialParticleVel.z + particleFanAmount.z));

			// var finalParticleVel = fannedParticleVel * ftSpawner.launchSpeed;

			//Set the particles start vel
			/*EntityManager.SetComponentData(spawnedParticle,
				new PhysicsVelocity
				{
					// Linear = finalParticleVel,
					Angular = float3.zero
				});

			
			//Spawn particles, as many as the flamethrower says to
			for (int i = 0; i < ftSpawner.spawnAmount; i++)
			{
				EntityManager.Instantiate(spawnedParticle);

			}#1#
		}).ScheduleParallel();*/
		
		Entities.ForEach(
			(Entity entity, ref Translation tx) =>
			{
				tx.Value += 1;
			})
			.ScheduleParallel();
			
	}
}



