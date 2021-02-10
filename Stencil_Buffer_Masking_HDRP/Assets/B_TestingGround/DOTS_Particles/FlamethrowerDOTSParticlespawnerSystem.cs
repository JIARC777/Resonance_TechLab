using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.Physics.Extensions;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class FlamethrowerDOTSParticlespawnerSystem : SystemBase
{
    public Entity particle;

    private int _spawnLimit = 10;
    int _iter = 0;

    protected override void OnUpdate()
    {
        if (_iter < _spawnLimit)
        {
            _iter++;
            SpawnParticles();
        } else if (Time.ElapsedTime == 25)
        {
            _iter = 0;
        }
        else
        {
            Debug.Log(Time.ElapsedTime);
        }
    }

    protected override void OnCreate()
    {
    }

    [BurstCompile]
    public void SpawnParticles()
    {
        var randomArray = World.GetExistingSystem<RandomSystem>().RandomArray;
        
        var dstManager = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer();


        Entities
            .WithNativeDisableParallelForRestriction(randomArray)
            .ForEach((int nativeThreadIndex, ref FlamethrowerSpawner ftSpawner, ref Translation tx, ref Rotation parentRot) =>
            {
                //Random
                var random = randomArray[nativeThreadIndex];
                
                //Spawn particles, as many as the flamethrower says to
                for (int i = 0; i < ftSpawner.spawnAmount; i++)
                {
                    Entity spawnedParticle = dstManager.Instantiate(ftSpawner.flamethrowerParticleEntity);

                    //Add a Scale component to the particles (not on the 
                    dstManager.AddComponent(spawnedParticle,
                        new Scale
                        {
                            Value = 0.1f
                        });

                    //Set the particle's position at the parent, flamethrower 'nozzle'
                    dstManager.SetComponent(spawnedParticle,
                        new Translation
                        {
                            Value = tx.Value
                        });


                    // Calculate the velocity for each particle
                    var particleFanAmount = ftSpawner.particleFanAmount;

                    var initialParticleVel = math.forward(parentRot.Value);

                    var randDirection = random.NextFloat3Direction();

                    var fannedParticleVel = new float3(
                        random.NextFloat(initialParticleVel.x - particleFanAmount.x,
                            initialParticleVel.x + particleFanAmount.x),
                        random.NextFloat(initialParticleVel.y - particleFanAmount.y,
                            initialParticleVel.y + particleFanAmount.y),
                        random.NextFloat(initialParticleVel.z - particleFanAmount.z,
                            initialParticleVel.z + particleFanAmount.z));

                    var finalParticleVel = fannedParticleVel * ftSpawner.launchSpeed;

                    //Set the particles start vel
                    dstManager.SetComponent(spawnedParticle,
                        new PhysicsVelocity
                        {
                            Linear = finalParticleVel,
                            Angular = float3.zero
                        });



                    //dstManager.Instantiate(spawnedParticle);
                }
                
                
                
                //Verify Random
                randomArray[nativeThreadIndex] = random;
            })
            
            .Run();
    }
}