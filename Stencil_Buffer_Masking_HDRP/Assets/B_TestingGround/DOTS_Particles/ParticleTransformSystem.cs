using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

//[DisableAutoCreation]
public class ParticleTransformSystem : ComponentSystem
{
	public float minParticleScale = 0.001f;

	private EntityCommandBufferSystem ecbSys;

	protected override void OnCreate()
	{
		ecbSys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
    {
        // Assign values to local variables captured in your job here, so that it has
        // everything it needs to do its work when it runs later.
        // For example,
	    var commandBuffer = ecbSys.CreateCommandBuffer().AsParallelWriter();
        var minPScale = minParticleScale;

        
        float deltaTime = Time.DeltaTime;

        // This declares a new kind of job, which is a unit of work to do.
        // The job is declared as an Entities.ForEach with the target components as parameters,
        // meaning it will process all entities in the world that have both
        // Translation and Rotation components. Change it to process the component
        // types you want.



        Entities
	        //.WithNativeDisableParallelForRestriction(commandBuffer)
	        //.WithAll<CutterParticleTag, Child, LocalToParent>()

	        .ForEach((Entity entity, ref Translation tx, ref Scale scale) =>
	        {

		        // Implement the work to perform for each entity here.
		        // You should only access data that is local or that is a
		        // field on this job. Note that the 'rotation' parameter is
		        // marked as 'in', which means it cannot be modified,
		        // but allows this job to run in parallel with other jobs
		        // that want to read Rotation component data.
		        // For example,
		        //     translation.Value += math.mul(rotation.Value, new float3(0, 0, 1)) * deltaTime;

		        //DOTS: tx.Value += math.forward(rot.Value) * deltaTime;

		        // Shrink over time

		        scale.Value *= .95f;

		        // Debug.Log("Scaling");
		        if (scale.Value <= minPScale)
		        {
			        PostUpdateCommands.DestroyEntity(entity);
			        //Debug.Log("Deleted ent");
		        }


	        }); //.WithBurst().ScheduleParallel();


        // commandBuffer.Dispose();
    }
}






