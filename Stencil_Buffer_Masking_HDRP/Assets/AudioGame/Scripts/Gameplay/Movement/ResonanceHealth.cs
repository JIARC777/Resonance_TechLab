using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResonanceHealth : MonoBehaviour
{
    public static ResonanceHealth instance;

    Animation youDiedAnim;

    public Vector3 startingLocation;
    public int maxPlayerHealth = 2;
    [Header("Set from script")]
    public static int currentPlayerHealth;

    //Establish a good singleton
    private void Awake()
    {
        youDiedAnim = GetComponent<Animation>();
        startingLocation = this.transform.position;
        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            instance = this;
            // Might need this later
            //DontDestroyOnLoad(gameObject);
        }
    }

    //Set health to max at level start
    private void Start()
    {
        currentPlayerHealth = maxPlayerHealth;
    }

    public void DamagePlayer()
    {
        currentPlayerHealth--;

        if (currentPlayerHealth <= 0)
            Die();
    }

    private void Die()
    {
        RespawnPlayer();
        //TODO: Figure out resetting level with persistent player
        Debug.Log("The player has died");
    }

    public IEnumerator RespawnPlayer()
	{
        youDiedAnim.Play();
        yield return new WaitForSeconds(1);
        transform.position = startingLocation;

    }
    
}
