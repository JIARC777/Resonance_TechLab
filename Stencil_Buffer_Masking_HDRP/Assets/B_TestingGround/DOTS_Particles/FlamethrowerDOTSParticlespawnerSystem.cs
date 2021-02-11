using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class FlamethrowerDOTSParticlespawnerSystem : SystemBase
{
	public Entity particle;

	protected override void OnUpdate()
	{
		SpawnParticles();
	}

	protected override void OnCreate()
	{
	}

	[BurstCompile]
	public void SpawnParticles()
	{
		var randomArray = World.GetExistingSystem<RandomSystem>().RandomArray;

		var dstManager = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer().AsParallelWriter();


		Entities
			.WithNativeDisableParallelForRestriction(randomArray)
			.ForEach((int nativeThreadIndex, in FlamethrowerSpawner ftSpawner, in Translation tx, in Rotation parentRot) =>
			{
				//Random
				var random = randomArray[nativeThreadIndex];


				Entity spawnedParticle = dstManager.Instantiate(0, ftSpawner.flamethrowerParticleEntity);

				//Add a Scale component to the particles (not on the 
				dstManager.AddComponent(0, spawnedParticle,
					new Scale
					{
						Value = 0.1f
					});
				//Set the particle's position at the parent, flamethrower 'nozzle'
				dstManager.SetComponent(0, spawnedParticle,
					new Translation
					{
						Value = tx.Value
					});
				
				// Calculate the velocity for each particle
				var particleFanAmount = ftSpawner.particleFanAmount;

				var initialParticleVel = math.forward(parentRot.Value);
				

				
				
				//Spawn particles, as many as the flamethrower says to
				for (int i = 0; i < ftSpawner.spawnAmount; i++)
				{

					var fannedParticleVel = new float3(
						random.NextFloat(initialParticleVel.x - particleFanAmount.x, initialParticleVel.x + particleFanAmount.x),
						random.NextFloat(initialParticleVel.y - particleFanAmount.y, initialParticleVel.y + particleFanAmount.y),
						random.NextFloat(initialParticleVel.z - particleFanAmount.z, initialParticleVel.z + particleFanAmount.z));

					var finalParticleVel = fannedParticleVel * ftSpawner.launchSpeed;

					//Set the particles start vel
					dstManager.SetComponent(0, spawnedParticle,
						new PhysicsVelocity
						{
							Linear = finalParticleVel,
							Angular = float3.zero
						});

					dstManager.Instantiate(0, spawnedParticle);
				}

				//Verify Random
				randomArray[nativeThreadIndex] = random;
			})
			.ScheduleParallel();
		
		this.CompleteDependency();
	}
}