using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// Adds the 'CutterParticleTag' to the 'CutterCube' prefab
/// </summary>
//[DisallowMultipleComponent]
public class CutterParticleTagAuthor : MonoBehaviour, IConvertGameObjectToEntity
{
	// Add fields to your component here. Remember that:
	//
	// * The purpose of this class is to store data for authoring purposes - it is not for use while the game is
	//   running.
	// 
	// * Traditional Unity serialization rules apply: fields must be public or marked with [SerializeField], and
	//   must be one of the supported types.
	//
	// For example,
	public float scale;


	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		// Call methods on 'dstManager' to create runtime components on 'entity' here. Remember that:
		//
		// * You can add more than one component to the entity. It's also OK to not add any at all.
		//
		// * If you want to create more than one entity from the data in this class, use the 'conversionSystem'
		//   to do it, instead of adding entities through 'dstManager' directly.
		//
		// For example,

		dstManager.AddComponentData(entity, new Translation {});

		
		dstManager.AddComponentData(entity, new PhysicsVelocity {});
		
		dstManager.AddComponentData(entity, new Scale {});
		

		// dstManager.AddComponentData(entity, new CompositeScale
		// {
		// 	Value = new float4x4(rotation: quaternion.identity, translation:new float3(0,0,0))
		// });
		// dstManager.SetComponentData(entity, new LocalToWorld
		// {
		// 	Value = new float4x4(rotation: quaternion.identity, translation:new float3(0,0,0))
		// });


		dstManager.AddComponentData(entity, new RenderBounds {});

		
		var ah = Resources.Load<GameObject>("AssetHolder").GetComponent<AssetHolder>();
		dstManager.SetSharedComponentData(entity, new RenderMesh
		{
			mesh = ah.myMesh,
			material = ah.myMaterial
		});
		
		dstManager.RemoveComponent<LinkedEntityGroup>(entity);

		
		//dstManager.AddComponentData(entity, new CutterParticleTag { });
		
		// dstManager.AddComponent<Scale>(entity);
		//dstManager.AddComponentData(entity, new Scale {	Value = 0.1f });
		//// dstManager.AddComponent<LocalToParent>(entity);
		
	}
}

