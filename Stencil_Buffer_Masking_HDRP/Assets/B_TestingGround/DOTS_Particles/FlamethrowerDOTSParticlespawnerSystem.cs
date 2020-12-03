using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Extensions;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;


public class FlamethrowerDOTSParticlespawnerSystem : ComponentSystem
{
	[SerializeField] private int particleSpawnAmount = 20;




	private Random random;

	protected override void OnUpdate()
	{
		SpawnParticles();
	}

	protected override void OnCreate()
	{
		random = new Random(126);
	}

	public void SpawnParticles()
	{
		Entities.ForEach((ref FlamethrowerSpawner ftSpawner, ref Translation tx, ref Rotation parentRot) =>
		{
			for (int i = 0; i < particleSpawnAmount; i++)
			{
				Entity spawnedParticle = EntityManager.Instantiate(ftSpawner.flamethrowerParticleEntity);
				
				
				//Set the particle's position at the parent's
				EntityManager.SetComponentData(spawnedParticle,
					new Translation
					{
						Value = tx.Value
					});

				var particleFanAmount = ftSpawner.particleFanAmount;

				var initialParticleVel = math.forward(parentRot.Value);

				var randDirection = random.NextFloat3Direction();
				var fannedParticleVel = new float3(random.NextFloat(initialParticleVel.x - particleFanAmount.x, initialParticleVel.x + particleFanAmount.x),
													 random.NextFloat(initialParticleVel.y - particleFanAmount.y, initialParticleVel.y + particleFanAmount.y),
													 random.NextFloat(initialParticleVel.z - particleFanAmount.z, initialParticleVel.z + particleFanAmount.z));

				var finalParticleVel = fannedParticleVel * ftSpawner.launchSpeed;
				//Set the particles start vel
				EntityManager.SetComponentData(spawnedParticle,
					new PhysicsVelocity
					{
						Linear = finalParticleVel,
						Angular = float3.zero
					});
			}
		});
	}
}



