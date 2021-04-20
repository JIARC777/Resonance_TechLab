using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResonancePlayerHealthRespawn : MonoBehaviour
{
    public static ResonancePlayerHealthRespawn instance;

    public Animator playerDiedAnimator;
    public Animator damageAnimator;

    public Vector3 respawnLocation;
    public TextMeshProUGUI healthNumberText;

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
        healthNumberText.text = currentPlayerHealth.ToString();
        playerDiedAnimator.Play("Rest");
    }

    public void DamagePlayer()
    {
        currentPlayerHealth--;
        healthNumberText.text = currentPlayerHealth.ToString();
        if (currentPlayerHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(damageAnimation());
    }

    public IEnumerator damageAnimation()
    {
        damageAnimator.SetBool("Damage", true);
        yield return new WaitForSeconds(.5f);
        damageAnimator.SetBool("Damage", false);

    }

    private void Die()
    {
        StartCoroutine(RespawnPlayer());
        //TODO: Figure out resetting level with persistent player
        Debug.Log("The player has died");
    }

    public IEnumerator RespawnPlayer()
    {
        playerDiedAnimator.SetBool("Respawn", true);
        //playerDiedAnimator.Play("Entry");
        yield return new WaitForSeconds(1);
        transform.position = respawnLocation;
        currentPlayerHealth = maxPlayerHealth;
        healthNumberText.text = currentPlayerHealth.ToString();
        // If this ends the animation we should add an extra delay;
        yield return new WaitForSeconds(1);

        playerDiedAnimator.SetBool("Respawn", false);
    }
}