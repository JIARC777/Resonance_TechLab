using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate_Spools : MonoBehaviour
{
    [Header("Spool Generation Controller")]
    //Boolean that depends whether the user can spawn another spool
    public bool canSpawn;
    //The delay of when the user can spawn more spools
    public float generationDelay;

    //The spool object to be spawned
    public Transform spool;
    
    // Update is called once per frame
    void Update()
    {
        //Boolean that determines when the user hits the spacebar
        bool spawnSpool = Input.GetKey(KeyCode.Space);

        //If the spacebar is pressed, call the SpawnSpool() function
        if (spawnSpool && canSpawn)
            SpawnSpool();
    }

    //This function spawns spools using a coroutine
    public void SpawnSpool()
    {
        //Calls the Spawn() Coroutine
        StartCoroutine(Spawn());

        //This coroutine spawns a single spool
        IEnumerator Spawn()
        {
            //Prevents the player from spawning another spool
            canSpawn = false;

            //Creates another spool using the variable of the same name
            Instantiate(spool, transform);

            //Wait for a certain time in seconds based on the generationDelay variable
            yield return new WaitForSeconds(generationDelay);

            //Allows the player to generate another spool
            canSpawn = true;
        }

        
    }
}
