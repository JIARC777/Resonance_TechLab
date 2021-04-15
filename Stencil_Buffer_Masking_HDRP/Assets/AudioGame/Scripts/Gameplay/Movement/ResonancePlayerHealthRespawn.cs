using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResonancePlayerHealthRespawn : MonoBehaviour
{
    public static ResonancePlayerHealthRespawn instance;

    public Animator playerDiedAnimator;
    public Animator damageAnimator;

    public Vector3 respawnLocation;
    
    #region Health
    public int maxPlayerHealth = 2;
    [HideInInspector] public int currentPlayerHealth;
    #endregion
    //Establish a good singleton
    private void Awake()
    {
        respawnLocation = this.transform.position;
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
        playerDiedAnimator.Play("Rest");
    }

    public void DamagePlayer()
    {
        currentPlayerHealth--;

        if (currentPlayerHealth <= 0)
		{
            Die();
            return;
        }
        StartCoroutine(damageAnimation());
            
    }

    public IEnumerator damageAnimation()
	{
        damageAnimator.Play("Entry");
        yield return new WaitForSeconds(.5f);
        playerDiedAnimator.Play("Rest");
	}
    private void Die()
    {
        RespawnPlayer();
        //TODO: Figure out resetting level with persistent player
        Debug.Log("The player has died");
    }

    public IEnumerator RespawnPlayer()
	{
        playerDiedAnimator.Play("Entry");
        yield return new WaitForSeconds(1);
        transform.position = respawnLocation;
        // If this ends the animation we should add an extra delay;
        playerDiedAnimator.Play("Rest");


    }
    
}
