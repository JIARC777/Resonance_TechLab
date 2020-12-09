using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

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
		var collideJob = new CollideJob
			{
				cutParticleTag = GetComponentDataFromEntity<CutterParticleTag>(),
				dstManager = dstManager.CreateCommandBuffer(),
				jobHandle = inputDeps,
			};

		// dstManager.AddJobHandleForProducer(collideJob);
		return collideJob.Schedule(StepPhysicsWorld.Simulation, ref BuildPhysicsWorld.PhysicsWorld, inputDeps);
	}

	[BurstCompile]
	private struct CollideJob : ICollisionEventsJob
	{
		public ComponentDataFromEntity<CutterParticleTag> cutParticleTag;
		public EntityCommandBuffer dstManager;
		public JobHandle jobHandle;




		public void Execute(CollisionEvent collisionEvent)
		{
			Entity particle;
			Entity otherCollider;

			if (cutParticleTag.HasComponent(collisionEvent.EntityA))
			{
				//Particle is A
				particle = collisionEvent.EntityA;
				otherCollider = collisionEvent.EntityB;
			}
			else if (cutParticleTag.HasComponent(collisionEvent.EntityB))
			{
				//Particle is B
				particle = collisionEvent.EntityB;
				otherCollider = collisionEvent.EntityA;
			}
			else
			{
				return;
			}

			#region Adjust Particle

			//Add the needed transform

			// Connect to the parent
			 dstManager.AddComponent(particle, new Parent
			 {
			 	Value = otherCollider
			 });
			 dstManager.AddComponent(particle, new LocalToWorld
			 {
			 });
			 dstManager.AddComponent(particle, new LocalToParent
			 {
			 });

			
			//Remove physics body
			dstManager.SetComponent<PhysicsVelocity>(particle, new PhysicsVelocity{});

			dstManager.SetComponent<PhysicsDamping>(particle, new PhysicsDamping
			{
				Linear =  Single.PositiveInfinity,
				Angular = Single.PositiveInfinity
			});
			
			// dstManager.AddComponent(particle, new Frozen());
			
			#endregion Adjust Particle
			
			
			
			#region Adjust Collider

			// dstManager.AddComponent(otherCollider, new LocalToWorld { });
			
			#endregion Adjust Collider


			jobHandle.Complete();
		}
		
	}
}