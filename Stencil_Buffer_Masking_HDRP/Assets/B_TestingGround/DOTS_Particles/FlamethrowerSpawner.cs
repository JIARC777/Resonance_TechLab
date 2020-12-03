using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;



/// <summary>
/// Add this to the 'muzzle' of the flamethrower, spawns particles
/// </summary>
[GenerateAuthoringComponent]
public struct FlamethrowerSpawner : IComponentData
{
	[SerializeField] public Entity flamethrowerParticleEntity;
	[SerializeField] public float launchSpeed;
	[SerializeField] public float3 particleFanAmount;
}