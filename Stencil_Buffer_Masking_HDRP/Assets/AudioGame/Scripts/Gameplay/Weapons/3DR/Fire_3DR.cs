using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire_3DR : MonoBehaviour
{

    [Header("3DR Properties")]
    //The force that will be applied to the projectile
    public float firingForce;
    //Can the 3DR generate another projectile
    public bool canGenerateAmmo;
    //Is the 3DR being held in VR
    public bool isHeld;
    //How long it takes for a projectile to be printed
    public float printTime;
    //Is the player firing the 3DR
    public bool isFiring = false;
    //Is the player reloading
    public bool isReloading = false;

    [Header("Spool Properties")]
    //How many "charges" of the spool remain
    public float spoolRemaining;
    //How many charges of the spool are lost each time a projectile is made
    public float spoolLost;
    //The maximum amount of charges a spool provides to the 3DR
    public float maxSpool;
    //How much the spool model is reduced by when a projectile is made(debug)
    public float spoolScaleReduce;

    [Header("Ammo Models")]
    //Set the chance of getting a rare projectile at 1 / ammoChance
    public int ammoChance = 10;
    //An array of all the common projectile the 3DR can print
    public Transform[] commonAmmoModels;
    //An array of all the rare projectile the 3DR can print
    public Transform[] rareAmmoModels;

    //The original size of the spool(debug)
    private Vector3 spoolOrigSize = new Vector3(1f, 0.1f, 1f);

    [Header("3DR Transforms")]
    //Transform where the projectiles are placed to be fired
    public Transform firingLocation;
    //Current projectile loaded in the 3DR
    public Transform currentAmmo;
    //The object representing the spool on the 3DR(debug, can be removed in place for a model)
    public Transform internalSpool;
    //Place where the printed projectiles are placed
    public Transform ammoSpawn;

    [Header("Line Properties")]
    //The LineRenderer used to create the aiming line
    public LineRenderer aimingLine;
    //How many points the line follows
    public int lineLength;
    //The object the aimingLine LineRenderer is on
    public Transform aimingObject;    
    //How wide the line looks
    public float lineWidthMultiplier;

    //Instance of this script, used within the spool and projectile reloading scripts
    public static Fire_3DR threeDRInstance;

    // Start is called before the first frame update
    void Start()
    {
        //Assign threeDRInstance to the transform it's attached to
        threeDRInstance = this;

        //Sets the initial amount of spool charges to the max spool charge count
        spoolRemaining = maxSpool;

        //Assigns the firingLocation, internalSpool, and ammoSpawn transforms to their corresponding transforms within the 3DR prefab
        firingLocation = transform.GetChild(3);
        internalSpool = transform.GetChild(4);//Debug
        ammoSpawn = transform.GetChild(5);

        //Creates a gameObject with the 3DR's LineRenderer attached to it, then assigns the LineRenderer component to the aimingLine LineRenderer variable
        aimingLine = Instantiate(aimingObject, null).GetComponent<LineRenderer>();

        //Disables the aimingLine LineRenderer
        aimingLine.enabled = false;
    }

    public void startFiring()
    {
        isFiring = true;
    }
    public void stopFiring()
    {
        isFiring = false;
    }
    public void startReloading()
    {
        isReloading = true;
    }

    public void stopReloading()
    {
        isReloading = false;
    }

    public void OnPickUp()
    {
        isHeld = true;
    }

    public void OnDrop()
    {
        isHeld = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //If the 3DR isn't being held, turn off the aim lines
        if (!isHeld)
        {
            aimingLine.gameObject.GetComponent<LineRenderer>().enabled = false;
        }

        //If the 3DR is being held
        if (isHeld)
        {
            
            //If the 3DR is being held, turn on the aim lines
            aimingLine.enabled = true;

            //Sets the aimingLine gameObject's position and yRotation to this object's respective position and rotation
            aimingLine.gameObject.transform.localPosition = firingLocation.position;
            aimingLine.gameObject.transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);

            //Generate the aiming line path
            CreateLinePositions();

            //If the 3DR has a projectile loaded and the LMB is pressed
            if (currentAmmo && isFiring)
            {
                //Call the Fire() function
                Fire();
            }
            //If the amount of spool charges is greater than 0, the "R" key is pressed, and the 3DR isn't creating a projectile already
            if (spoolRemaining > 0 && isReloading && canGenerateAmmo)
            {
                //Call the GenerateAmmo() function
                GenerateAmmo();
            }
        }        
    }

    //This function places a projectile into the firingLocation, and assigns the ammo added to the currentAmmo transform
    public void Reload(Transform ammo)
    {
        //Assigns ammo to the currentAmmo transform
        currentAmmo = ammo;

        //Sets the parent, position, and rotation of currentAmmo to the firing location
        currentAmmo.SetParent(firingLocation);
        currentAmmo.position = firingLocation.position;
        currentAmmo.rotation = firingLocation.rotation;

        //Makes the currentAmmo gameobject kinematic
        currentAmmo.gameObject.GetComponent<Rigidbody>().isKinematic = true;             

    }

    //This function generates a projectile to be used as ammunition for the 3DR
    public void GenerateAmmo()
    {
        //Starts the Generate() coroutine
        StartCoroutine(Generate());

        //This coroutine creates a projectile, choosing the ammo model at random, while also preventing the player from generating more projectiles while the first one is being made
        IEnumerator Generate()
        {
            //Prevents any more projectiles from being created during this process then reduces the remaining spool charges by the spoolLost variable
            canGenerateAmmo = false;
            spoolRemaining -= spoolLost;

            //If the spool has no remaining charges left, reset its size back to its original size, the disable it
            if (spoolRemaining == 0)
            {
                //Set the internalSpool's scale back to its original size(debug)
                internalSpool.localScale = spoolOrigSize;
                //Set the internalSpool gameobject's active state to false(debug)
                internalSpool.gameObject.SetActive(false);
            }
            //Otherwise, multiply the internalSpool's scale by the spoolScaleReduce variable(debug)
            else
            {
                internalSpool.localScale *= spoolScaleReduce;
            }

            //Halts the code in seconds based on the printTime variable(animation could be played here if added)
            yield return new WaitForSeconds(printTime);

            //Creates a null Transform that will be set in the following if statements
            Transform newAmmo = null;

            //Creates an integer spanning between 0 and the ammoChance variable
            int randomAmmoChoice = Random.Range(0, ammoChance);

            //If the randomAmmoChoice variable ends up equalling to half the ammoChance variable, choose a rare ammo type, otherwise choose a common ammo type
            if(randomAmmoChoice == ammoChance / 2)
            {
                //Gets the size of the rareAmmoModels array then create a random integer based on its size
                int rareArraySize = rareAmmoModels.Length;
                int randomRareAmmo = Random.Range(0, rareArraySize);

                //Assigns a random projectile to a transform using the randomAmmoChoice integer
                newAmmo = rareAmmoModels[randomRareAmmo];
            }
            else
            {
                //Gets the size of the commonAmmoModels array then create a random integer based on its size
                int commonArraySize = commonAmmoModels.Length;
                int randomCommonAmmo = Random.Range(0, commonArraySize);

                //Assigns a random projectile to a transform using the randomAmmoChoice integer
                newAmmo = commonAmmoModels[randomCommonAmmo];
            }            

            //Creates the projectile based on the newAmmo transform, and assigns the ammoSpawn transform as its parent
            Instantiate(newAmmo, ammoSpawn);

            //Allows the player to generate another projectile, if they're able to
            canGenerateAmmo = true;
        }       
    }

    //When a spool is placed within the Spool_Loading gameobject trigger, this function is called to replenish the spool charges
    public void NewSpool()
    {
        //Assigns the maxSpool value to the spoolRemaining value, replenishing its charges
        spoolRemaining = maxSpool;

        //If the internalSpool gameobject is disabled, re-enable it(debug)
        if(internalSpool.gameObject.activeSelf == false)
        {
            internalSpool.gameObject.SetActive(true);
        }

        //The internalSpool gameobject's scale is returned to its original size(debug)
        internalSpool.localScale = spoolOrigSize;
    }

    //This function fires a projectile from the 3DR
    public void Fire()
    {
        //Unparents the projectile from the 3DR's firingLocation
        currentAmmo.SetParent(null);

        //Disables the projectile's kinematic property
        currentAmmo.gameObject.GetComponent<Rigidbody>().isKinematic = false;

        //Applies an impulse force to the projectile in the forward direction of the firingLocations position using the firingForce variable
        currentAmmo.gameObject.GetComponent<Rigidbody>().AddForce(firingLocation.forward * firingForce, ForceMode.Impulse);

        //Calls the Despawn() function withing the Despawn_Item script atttached to the currentAmmo gameobject
        currentAmmo.gameObject.GetComponent<Despawn_Item>().Despawn();

        //Sets the currentAmmo in the 3DR to null
        currentAmmo = null;
    }

    //This function uses the parameters under the Line header and uses them to create a LineRenderer
    public LineRenderer LineCreator()
    {
        //Finds the LineRenderer component on the firingLocation gameObject
        LineRenderer line = aimingLine;

        //Sets the LineRenderer's width multiplier, how many points it has, and whether it uses the world space
        line.useWorldSpace = false;
        line.widthMultiplier = lineWidthMultiplier;
        line.positionCount = lineLength;

        //Returns the created LineRenderer
        return line;
    }

    //This function creates a path based on the forced applied to the projectile, and the total points within the line created
    public void CreateLinePositions()
    {

        //Use the LineCreator function to create a line
        LineRenderer aimingLine = LineCreator();

        //Loops through lineLength amount of times
        for (int i = 0; i < lineLength; i++)
        {

            //Gets the time at this point in the line
            float timeTravelled = (float)i / lineLength;

            //Calculates the vertical and horizontal velocities using PHYSICS EQUATIONS
            float horizVel = firingForce * Mathf.Cos(transform.localEulerAngles.x * Mathf.Deg2Rad);
            float vertVel = firingForce * Mathf.Sin(transform.localEulerAngles.x * Mathf.Deg2Rad);

            //Calculates the vertical and horizontal distances at the current time using PHYSICS EQUATIONS
            float vertDist = -(timeTravelled * ((49 * timeTravelled) + (10 * vertVel)) / 10);
            float horizDist = horizVel * timeTravelled;

            //Sets the position of the current point in the line using the variables set previously
            aimingLine.SetPosition(i, new Vector3(0, vertDist, horizDist));    
            
        }
    }
}

