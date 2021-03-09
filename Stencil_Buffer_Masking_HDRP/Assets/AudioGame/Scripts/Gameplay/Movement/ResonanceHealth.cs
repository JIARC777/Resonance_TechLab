using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResonanceHealth : MonoBehaviour
{
    public static ResonanceHealth instance;

    public int maxPlayerHealth = 2;
    [Header("Set from script")]
    public static int currentPlayerHealth;

    //Establish a good singleton
    private void Awake()
    {
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

    public static void DamagePlayer()
    {
        currentPlayerHealth--;

        if (currentPlayerHealth <= 0)
            Die();
    }

    private static void Die()
    {
        //TODO: Figure out resetting level with persistent player
        Debug.Log("The player has died");
    }
    
}
