using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Rendering.HybridV2;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class FlamethrowerDOTSParticlespawnerSystem : SystemBase
{
    [SerializeField]  Entity particle;
    bool _tripped = false;

    GameObject cutterCube;

    protected override void OnUpdate()
    {
        if (Time.ElapsedTime >= 7 && _tripped == false)
        {
            _tripped = true;
            SpawnParticles();
        }
    }

    protected override void OnCreate()
    {
        cutterCube = Resources.Load("cutterCube") as GameObject;
        Entity prefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(cutterCube, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, new BlobAssetStore()));


        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //entityManager.AddComponent<Disabled>(prefab);

        // EntityArchetype entityArchetype = entityManager.CreateArchetype(
        //     typeof(RenderMesh),
        //     typeof(LocalToWorld),
        //     typeof(RenderBounds)
        // );

        // create 100 entities
        NativeArray<Entity> entityArray = new NativeArray<Entity>(5000, Allocator.Temp);
        entityManager.Instantiate(prefab, entityArray);

        // for (int i = 0; i < entityArray.Length; i++)
        // {
        //     Entity entity = entityArray[i];
        //
        //     entityManager.SetSharedComponentData(entity, new RenderMesh
        //     {
        //         mesh = _mesh,
        //         material = _mat,
        //     });
        // }
        
        entityManager.SetEnabled(entityArray, false);
        entityArray.Dispose();
        particle = prefab;
    }

    [BurstCompile]
    public void SpawnParticles()
    {
        var randomArray = World.GetExistingSystem<RandomSystem>().RandomArray;

        var dstManager = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer()
            .AsParallelWriter();

        Entity spawnedParticle = particle;//dstManager.Instantiate(0, ftSpawner.flamethrowerParticleEntity);

        Entities
            .WithNativeDisableParallelForRestriction(randomArray)
            .ForEach(
                (int nativeThreadIndex, in FlamethrowerSpawner ftSpawner, in Translation tx, in Rotation parentRot) =>
                {
                    //Random
                    var random = randomArray[nativeThreadIndex];



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
                            random.NextFloat(initialParticleVel.x - particleFanAmount.x,
                                initialParticleVel.x + particleFanAmount.x),
                            random.NextFloat(initialParticleVel.y - particleFanAmount.y,
                                initialParticleVel.y + particleFanAmount.y),
                            random.NextFloat(initialParticleVel.z - particleFanAmount.z,
                                initialParticleVel.z + particleFanAmount.z));

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