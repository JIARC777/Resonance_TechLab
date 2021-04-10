using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Fire_3DR : MonoBehaviour
{
    #region 3DR Properties
    [Header("3DR Properties")]
    public static Fire_3DR threeDRInstance; //Instance of this script, used within the spool and projectile reloading scripts
    private SteamVR_Input_Sources holdingHand; //SteamVR input reference
    public float firingForce; //The force that will be applied to the projectile
    public float printTime; //How long it takes for a projectile to be printed
    public bool canGenerateProjectile; //Can the 3DR generate another projectile
    public bool isHeld; //Is the 3DR being held in VR    
    public bool isFiring = false; //Is the player firing the 3DR
    #endregion 3DR Properties

    #region Spool Properties
    [Header("Spool Properties")]    
    public float spoolRemaining; //How many "charges" of the spool remain
    public float spoolLost; //How many charges of the spool are lost each time a projectile is made
    public float maxSpool; //The maximum amount of charges a spool provides to the 3DR
    public float spoolScaleReduce; //How much the spool model is reduced by when a projectile is made
    private Vector3 spoolOrigSize = new Vector3(1f, 0.1f, 1f); //The original size of the spool
    #endregion Spool Properties

    #region Projectile Properties
    [Header("Projectile Properties")]    
    public int projectileChance = 10; //Set the chance of getting a rare projectile at 1 / ammoChance    
    public Transform[] commonProjectileModels; //An array of all the common projectile the 3DR can print    
    public Transform[] rareProjectileModels; //An array of all the rare projectile the 3DR can print   
    #endregion Projectile Properties

    #region 3DR Transforms
    [Header("3DR Transforms")]
    public Transform firingLocation; //Transform where the projectiles are placed to be fired    
    public Transform currentProjectile; //Current projectile loaded in the 3DR    
    public Transform internalSpool; //The object representing the spool on the 3DR
    public Transform spoolOBJ;
    //public Transform projectileSpawn; //Place where the printed projectiles are placed
    #endregion 3DR Transforms

    #region Line Properties
    [Header("Line Properties")]    
    public LineRenderer aimingLine; //The LineRenderer used to create the aiming line    
    public Transform aimingObject; //The object the aimingLine LineRenderer is on
    public int lineLength; //How many points the line follows
    public float lineWidthMultiplier; //How wide the line looks
    #endregion Line Properties

    public ParticleSystem cutterEmitter;

    #region SteamVR Input Functions
    public void StartFiring(SteamVR_Behaviour_Boolean behavBool, SteamVR_Input_Sources source, bool boolStat)
    {
        var bTriggerPulledIsSameHand = behavBool.booleanAction.activeDevice == holdingHand;
        if (bTriggerPulledIsSameHand)
            isFiring = true;
    }

    public void StopFiring() => isFiring = false;

    public void OnPickUp()
    {
        holdingHand = GetComponentInParent<Hand>().handType;
        Debug.Log("Picked Up");
        isHeld = true;
    }

    public void OnDrop() => isHeld = false;
    #endregion  SteamVR Input Functions

    // Start is called before the first frame update
    void Start()
    {
        
        threeDRInstance = this; //Assign threeDRInstance to the transform it's attached to
        
        spoolRemaining = maxSpool; //Sets the initial amount of spool charges to the max spool charge count

        //Assigns the firingLocation, internalSpool, spoolOBJ, and ammoSpawn transforms to their corresponding transforms within the 3DR prefab
        firingLocation = transform.GetChild(2);
        
        spoolOBJ = transform.GetChild(0);
        spoolOBJ.GetComponent<BoxCollider>().enabled = false;
        
        internalSpool = spoolOBJ.GetChild(1);
        
        aimingObject = transform.GetChild(4);
                
        aimingLine = aimingObject.GetComponent<LineRenderer>(); //Creates a gameObject with the 3DR's LineRenderer attached to it, then assigns the LineRenderer component to the aimingLine LineRenderer variable
                
        aimingLine.enabled = false; //Disables the aimingLine LineRenderer

        canGenerateProjectile = true;
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (!isHeld) //If the 3DR isn't being held, turn off the aim lines
        {
            aimingLine.enabled = false;
        }
                
        if (isHeld && currentProjectile) //Turn on the aim lines only when held and when the 3DR is loaded
        {
            //Generate the aiming line path
            CreateLinePositions();

            aimingLine.enabled = true;
            aimingLine.transform.localPosition = firingLocation.localPosition;
            aimingLine.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
        
        if (isHeld) //If the 3DR is being held
        {
            
            if (currentProjectile && isFiring) //If the 3DR has a projectile loaded and the corresponding trigger is pressed, fire the 3DR
            {
                Fire();
            }
            
            if (spoolRemaining > 0 && canGenerateProjectile && !currentProjectile) //If the amount of spool charges is greater than 0 and the 3DR isn't creating a projectile already, generate another projectile
            {
                StartCoroutine(Generate());
            }
        }        
    }


    //This function places a projectile into the firingLocation, and assigns the ammo added to the currentAmmo transform
    public void Reload(Transform ammo)
    {

        currentProjectile = Instantiate(ammo, firingLocation); //Creates a projectile based off what is loaded into the 3DR and parent it to the firingLocation Transform

        //Sets the parent, position, and rotation of currentAmmo to the firing location
        currentProjectile.position = firingLocation.position;
        currentProjectile.rotation = firingLocation.rotation;

        currentProjectile.gameObject.GetComponent<Rigidbody>().isKinematic = true; //Makes the currentAmmo gameobject kinematic     

    }


    //This coroutine creates a projectile, choosing the ammo model at random, while also preventing the player from generating more projectiles while the first one is being made
    IEnumerator Generate()
    {
        //Prevents any more projectiles from being created during this process then reduces the remaining spool charges by the spoolLost variable
        canGenerateProjectile = false;

        internalSpool.localScale = new Vector3(internalSpool.localScale.x * spoolScaleReduce, internalSpool.localScale.y * spoolScaleReduce, internalSpool.localScale.z);
            
        yield return new WaitForSeconds(printTime); //Waits for printTime seconds


        //Creates a null Transform that will be used to set a new projectile, then create an integer spanning between 0 and the ammoChance variable
        Transform newAmmo = null;
        int randomAmmoChoice = Random.Range(0, projectileChance);
        
        if(randomAmmoChoice == projectileChance / 2) //If the randomAmmoChoice variable ends up equalling to half the ammoChance variable, choose a rare ammo type, otherwise choose a common ammo type
        {
            int rareArraySize = rareProjectileModels.Length;
            int randomRareAmmo = Random.Range(0, rareArraySize);

            newAmmo = rareProjectileModels[randomRareAmmo];
        }
        else
        {
            int commonArraySize = commonProjectileModels.Length;
            int randomCommonAmmo = Random.Range(0, commonArraySize);

            newAmmo = commonProjectileModels[randomCommonAmmo];
        }
            
        spoolRemaining -= spoolLost;
            
        //Creates the projectile based on the newAmmo transform, and assigns the ammoSpawn transform as its parent, then allows the player to generate another projectile, if they have a spool charge remaining
        Reload(newAmmo);
        canGenerateProjectile = true;
        
        if (spoolRemaining == 0) //If the spool has no remaining charges left, reset its size back to its original size, then disable it
        {
            StartCoroutine(EjectSpool());
            //internalSpool.gameObject.SetActive(false);
        }
    }  
    

    //When a spool is placed within the Spool_Loading gameobject trigger, this function is called to replenish the spool charges
    public void NewSpool()
    {
        
        spoolRemaining = maxSpool; //Assigns the maxSpool value to the spoolRemaining value, replenishing its charges
                
        if (spoolOBJ.gameObject.activeSelf == false) //If the internalSpool gameobject is disabled, re-enable it
        {
            spoolOBJ.gameObject.SetActive(true);
        }
        
        internalSpool.localScale = spoolOrigSize; //The internalSpool gameobject's scale is returned to its original size
    }


    //This function fires a projectile from the 3DR
    private void Fire()
    {
        //Emit vision particles
        cutterEmitter.Play();

        //Unparents the projectile from the 3DR's firingLocation then disables the projectile's kinematic property
        currentProjectile.SetParent(null);
        currentProjectile.gameObject.GetComponent<Rigidbody>().isKinematic = false;

        //Applies an impulse force to the projectile in the forward direction of the firingLocations position using the firingForce variable
        currentProjectile.gameObject.GetComponent<Rigidbody>().AddForce(firingLocation.forward * firingForce, ForceMode.Impulse);

        //Calls the Despawn() function withing the Despawn_Item script atttached to the currentAmmo gameobject, then Sets the currentAmmo in the 3DR and allows the 3DR to generate projectiles again
        currentProjectile.gameObject.GetComponent<Despawn_Item>().Despawn();
        currentProjectile = null;
        canGenerateProjectile = true;
    }


    //This function uses the parameters under the Line header and uses them to create a LineRenderer
    public LineRenderer LineCreator()
    {
        
        LineRenderer line = aimingLine; //Finds the LineRenderer component on the firingLocation gameObject

        //Sets the LineRenderer's width multiplier, how many points it has, and whether it uses the world space
        line.useWorldSpace = false;
        line.widthMultiplier = lineWidthMultiplier;
        line.positionCount = lineLength;
        
        return line; //Returns the created LineRenderer
    }

    //This function creates a path based on the forced applied to the projectile, and the total points within the line created
    public void CreateLinePositions()
    {
        
        LineRenderer aimingLine = LineCreator(); //Use the LineCreator function to create a line
                
        for (int i = 0; i < lineLength; i++) //Loops through lineLength amount of times
        {
            
            float timeTravelled = (float)i / lineLength; //Gets the time at this point in the line

            //Calculates the vertical and horizontal velocities using PHYSICS EQUATIONS
            float horizVel = firingForce * Mathf.Cos(transform.rotation.eulerAngles.x * Mathf.Deg2Rad);
            float vertVel = firingForce * Mathf.Sin(transform.rotation.eulerAngles.x * Mathf.Deg2Rad);

            //Calculates the vertical and horizontal distances at the current time using PHYSICS EQUATIONS
            float vertDist = -(timeTravelled * ((49 * timeTravelled) + (10 * vertVel)) / 10);
            float horizDist = horizVel * timeTravelled;
            
            aimingLine.SetPosition(i, new Vector3(0, vertDist, horizDist)); //Sets the position of the current point in the line using the variables set previously
        }
    }

    IEnumerator EjectSpool()
    {

        Debug.Log("EJECT");
        GetComponent<HingeDoor>().isLoaded = false;
        Vector3 spoolOrigPos = spoolOBJ.localPosition;
        Quaternion origRotation = spoolOBJ.localRotation;

        GetComponent<HingeDoor>().UnlockDoor();
        
        spoolOBJ.GetComponent<Rigidbody>().isKinematic = false;
        
        spoolOBJ.GetComponent<BoxCollider>().enabled = true;
        
        spoolOBJ.GetComponent<Rigidbody>().AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
        
        yield return new WaitForSeconds(printTime);

        spoolOBJ.GetComponent<BoxCollider>().enabled = false;
        
        internalSpool.localScale = spoolOrigSize;

        spoolOBJ.GetComponent<Rigidbody>().isKinematic = true;
        
        spoolOBJ.localPosition = spoolOrigPos;
        spoolOBJ.localRotation = origRotation;
        
        spoolOBJ.gameObject.SetActive(false);

    }
}

