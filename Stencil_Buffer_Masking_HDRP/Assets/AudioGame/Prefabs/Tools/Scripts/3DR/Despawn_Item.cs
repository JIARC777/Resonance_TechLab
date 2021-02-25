using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn_Item : MonoBehaviour
{
    [Header("Adjust Time Until Item Despawns")]
    public float timeUntilDespawn;

    //When this function is called, the gameobject this script is attached to will be destroyed after a certain amount of time
    public void Despawn()
    {
        //Starts the DespawnItem() coroutine
        StartCoroutine(DespawnItem());

        //This coroutine controls when the gameobject this script is attached to is destroyed
        IEnumerator DespawnItem()
        {
            //Waits for an amount of time in seconds based on the timeUntilDespawn variable
            yield return new WaitForSeconds(timeUntilDespawn);

            //Destroys the gameobject this script is attached to
            Destroy(gameObject);
        }
    }
}
