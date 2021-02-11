using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
// [UpdateAfter(typeof(EndFramePhysicsSystem))] //Not bad, removes an -out-of-update instance
[UpdateAfter(typeof(StepPhysicsWorld))]
[UpdateBefore(typeof(EndFramePhysicsSystem))]
//[DisableAutoCreation]
public class CutterParticleCollisionSystem : JobComponentSystem
{
	private BuildPhysicsWorld BuildPhysicsWorld;
	private StepPhysicsWorld StepPhysicsWorld;
	private EndSimulationEntityCommandBufferSystem dstManager;

	protected override void OnCreate()
	{
		BuildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		StepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
		dstManager = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
	}

	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		dstManager.AddJobHandleForProducer(inputDeps);

		var collideJob = new CollideJob
			{
				cutParticleTag = GetComponentDataFromEntity<CutterParticleTag>(),
				dstManager = dstManager.CreateCommandBuffer(),
				//jobHandle = inputDeps,
			}.Schedule(StepPhysicsWorld.Simulation, ref BuildPhysicsWorld.PhysicsWorld, inputDeps);

		collideJob.Complete();
		return collideJob;
	}

	// [BurstCompile]
	[RequireComponentTag(typeof(CutterParticleTag))]
	private struct CollideJob : ITriggerEventsJob
	{
		[ReadOnly] public ComponentDataFromEntity<CutterParticleTag> cutParticleTag;
		public EntityCommandBuffer dstManager;
		//public JobHandle jobHandle;




		public void Execute(CollisionEvent collisionEvent)
		{
			Entity particle;
			Entity otherCollider;

			
			///
			/// At this point in time, and due to [] filtering above, ONLY particle announces colls
			// if (cutParticleTag.HasComponent(collisionEvent.EntityA))
			{
				//Particle is A
				particle = collisionEvent.EntityA;
				otherCollider = collisionEvent.EntityB;
				// Debug.Log("Particle A");
			}
			// else if (cutParticleTag.HasComponent(collisionEvent.EntityB))
			// {
			// 	//Particle is B
			// 	particle = collisionEvent.EntityB;
			// 	otherCollider = collisionEvent.EntityA;
			// 	Debug.Log("Particle B");
			// }
			// else
			// {
			// 	//Debug.Log("No Collision");
			// 	//jobHandle.Complete();
			// 	return;
			// }
			// //Debug.Log("Collided");



			#region Adjust Particle

			//Add the needed transform

			// Connect to the parent
			// dstManager.AddComponent(particle, new Parent
			//  {
			//  	Value = otherCollider
			//  });
			//  dstManager.AddComponent(particle, new LocalToWorld
			//  {
			//  });
			//  dstManager.AddComponent(particle, new LocalToParent
			//  {
			//  });

			
			//Remove physics body
			dstManager.SetComponent<PhysicsVelocity>(particle, new PhysicsVelocity{});




			// dstManager.SetComponent<PhysicsDamping>(particle, new PhysicsDamping
			// {
			// 	Linear =  Single.PositiveInfinity,
			// 	Angular = Single.PositiveInfinity
			// });


			#endregion Adjust Particle



			#region Adjust Collider


			#endregion Adjust Collider


			//jobHandle.Complete();
		}

		public void Execute(TriggerEvent triggerEvent)
		{
			throw new NotImplementedException();
		}
	}
}