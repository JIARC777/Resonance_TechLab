using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Fire_AMT : MonoBehaviour
{
    #region AMT Particle System
    [Header("AMT Particle System")]
    public ParticleSystem particles;
    #endregion AMT Particle System

    #region AMT States
    [Header("AMT States")]
    public bool isHeld = false; //Is the AMT being held        
    public bool canFire = true; //Can the AMT be fired/Is the AMT recharging from a recent shot
    public bool isClapping = false; //Is the player pressing the Clap button
    public bool isFiring = false; //Is the player pressing the Fire trigger
    private SteamVR_Input_Sources holdingHand; //The hand the player is using to hold the AMT
    #endregion AMT Energy Settings

    #region AMT Energy Settings
    [Header("AMT Energy Settings")]
    public float energy; //How much energy the AMT starts with in reserve
    public float energyReserve = 100f; //How much energy the AMT can hold in reserve in total, and how much is given at startup
    public float clapEnergyUsed = 25f; //How much energy is used when the AMT makes a Clap
    public float clapRechargeTime = 0.1f; //Time it takes to recharge for another Clap
    #endregion AMT Energy Settings

    #region Beam Props
    [Header("Beam Properties")]
    public float charge = 0f; //How much has the AMT been charged    
    public float beamRechargeTime = 3f; //Time it takes to recharge for another shot        
    public float beamChargeUpTime = 2f; //How much time it takes for the AMT to charge before firing
    public float beamChargeScale = 0.5f; //The rate at which the beamCharge gameobject scales in size
    public float beamEnergyUsed = 50f; //How much energy is taken away per shot
    public float beamChargeOriginalScale = 0.1f; //Float that represents the beamCharge gameobject's original scale
    public float beamEnergyRegen = 2.5f; //How much energy is regenerated per second
    public float beamUpTime = 0.25f; //How long the beam is fired for
    public float beamLength = 5f; //How long the beam is        
    #endregion Beam Props

    #region AMT Objects
    [Header("AMT Objects")]
    public Spin_Emitter spinning;
    public Charge_Lighting chargeLight;
    public AMT_Hud hud;
    private Transform beam; //The Transform object representing the Beam
    private Transform beamCharge; //The Transform object representing the Beam's charging animation
    private Transform lineObj;
    private LineRenderer aimingLine;
    #endregion AMT Objects  

    #region SteamVR Input Functions
    public void StartFiring(SteamVR_Behaviour_Boolean behavBool, SteamVR_Input_Sources source, bool boolStat)
    {
        var bTriggerPulledIsSameHand = behavBool.booleanAction.activeDevice == holdingHand;
        if (bTriggerPulledIsSameHand)
            isFiring = true;
    }

    public void StopFiring() => isFiring = false;

    public void StartClapping() => isClapping = true;

    public void StopClapping() => isClapping = false;

    public void OnPickUp()
    {
        isHeld = true;
        holdingHand = GetComponentInParent<Hand>().handType;
    }

    public void OnDrop() => isHeld = false;
    #endregion  SteamVR Input Functions

    // Start is called before the first frame update
    void Start()
    {
        //Find the beam, beamCharge, and aimingLine within the AMT parent and assign them to the beam, beamCharge, and beamDetect Transforms respectively
        beam = transform.GetChild(1);
        beamCharge = transform.GetChild(2);
        lineObj = transform.GetChild(3);

        aimingLine = lineObj.GetComponent<LineRenderer>();

        //Set the beam, beamCharge, and beamDetect gameObjects active state to false
        beam.gameObject.SetActive(false);
        beamCharge.gameObject.SetActive(false);
        aimingLine.enabled = false;

        energy = energyReserve; //Set the beginning energy reserve to the maximum energyReserve variable
    }

    // Update is called once per frame
    void Update()
    {

        float buildingCharge = Time.deltaTime; //Variable that is always counting up, used for building up the charge over time

        hud.SetCharge(charge);
        hud.SetEnergy(energy);

        if (isHeld) //If the AMT is being held
        {
            if (canFire) //If the AMT isn't recharging from a previous shot/clap
            {
                //If the corresponding button is down and the energy is greater or equal to the clapEnergyUsed variable, clap
                if (isClapping && (energy >= clapEnergyUsed))
                {
                    chargeLight.ClapLighting();
                    StartCoroutine(Clap());
                }

                //If the corresponding trigger is pressed and the energy is greater or equal to the beamEnergyUsed variable
                if (isFiring && (energy >= beamEnergyUsed))
                {

                    aimingLine.enabled = true;

                    lineObj.transform.position = beamCharge.position;

                    AdjustBeamLength(beamLength); //Adjust the size and position of the beam and beamDetect Transforms based on the beamLength variable

                    beamCharge.gameObject.SetActive(true); //Set the beamCharge gameobject's active state to true

                    charge += buildingCharge; //Increase the charge of the AMT using the incrementing buildingCharge variable stated previously

                    //Increment the beamCharge gameobject's scale by the beamChargeScale float multiplied by buildingCharge
                    beamCharge.localScale += new Vector3(beamChargeScale, beamChargeScale, beamChargeScale) * buildingCharge;

                    chargeLight.ChargeUp(buildingCharge);
                    spinning.SpinUp(buildingCharge);

                    //If the charge time of the AMT is greater or equal to beamChargeTime, fire the AMT
                    if (charge >= beamChargeUpTime)
                    {
                        Fire();
                    }
                }
            }
        }

        if (!canFire)
        {
            spinning.SpinDown(buildingCharge);
            chargeLight.ChargeDown(buildingCharge);
        }

        if (!isFiring) //If the firing trigger isn't being pressed
        {
            //Reset the beamCharge size and disable the aimingLine
            beamCharge.localScale = new Vector3(beamChargeOriginalScale, beamChargeOriginalScale, beamChargeOriginalScale);

            spinning.SpinDown(buildingCharge);

            aimingLine.enabled = false;

            //Set the beamCharge gameobject's active state to false and set the AMT's charge back to 0
            beamCharge.gameObject.SetActive(false);
            charge = 0;                        

            //If the AMT's energy is less-than 100, increase energy at a rate of beamRegen per second, otherwise, set the AMT's energy to the max energyReserve value
            if (energy < 100)
            {
                energy += Time.deltaTime * beamEnergyRegen;
            }
            else
            {
                energy = energyReserve;
            }
        }
    }

    //This function fires the AMT, quite self-explanatory
    public void Fire()
    {

        beam.gameObject.SetActive(true); //Set the active state of the beam gameobject and the beamDetect gameobject to true

        StartCoroutine(FireBeam()); //Start the FireBeam coroutine

        //This coroutine controls the events that occur when the AMT is fired
        IEnumerator FireBeam()
        {

            //Set the canFire boolean to false and reduce the total energy count by the beamEnergyUsed variable
            canFire = false;
            energy -= beamEnergyUsed;

            //Reset the beamCharge gameobject's scale to the beamChargeOrigScale variable, then set the active state of the beamCharge gameobject to false
            beamCharge.localScale = new Vector3(beamChargeOriginalScale, beamChargeOriginalScale, beamChargeOriginalScale);
            beamCharge.gameObject.SetActive(false);

            //Wait beamUpTime seconds, set the active state of the beam gameobject to false, then wait beamRechargeTime seconds
            yield return new WaitForSeconds(beamUpTime);
            beam.gameObject.SetActive(false);
            yield return new WaitForSeconds(beamRechargeTime);

            //Reset the charge float back to 0 and set the canFire boolean back to true
            charge = 0;
            canFire = true;
        }
    }

    IEnumerator Clap()
    {
        //Set the canFire boolean to false, then reduce the total energy count by the clapEnergyUsed variable
        canFire = false;
        energy -= clapEnergyUsed;

        //Emit sound particles, then wait clapRechargeTime seconds
        particles.Play();        

        yield return new WaitForSeconds(clapRechargeTime);

        canFire = true;

    }

    //This function adjusts the beam and beamDetect lengths using a common variable
    private void AdjustBeamLength(float newSize)
    {

        float beamPosAdjust = 0.375f + (0.5f * newSize); //Float that represents the equation to properly position each object using the newSize variable

        //Set the beam gameObject's size and position using the beamPosAdjust variable
        beam.localScale = new Vector3(0.1f, 0.1f, newSize);
        beam.localPosition = new Vector3(0, 0, beamPosAdjust);
    }
}
