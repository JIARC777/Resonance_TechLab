using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR.InteractionSystem
{
    public class Fire_AMT : MonoBehaviour
    {
        [Header("AMT Particle System")]
        public ParticleSystem particles;

        [Header("AMT States")]
        //Is the AMT being held
        public bool isHeld = false;
        //Can the AMT be fired/Is the AMT recharging from a recent shot
        public bool canFire = true;

        [Header("AMT Energy Settings")]
        //How much energy the AMT starts with in reserve
        public float energy;
        //How much energy the AMT can hold in reserve in total, and how much is given at startup
        public float energyReserve = 100f;
        //How much energy is used when the AMT makes a Clap
        public float clapEnergyUsed = 25f;
        //Time it takes to recharge for another clap
        public float clapRechargeTime = 0.1f;

        [Header("Beam Properties")]
        //How much has the AMT been charged; always starts at 0 (compared with beamChargeUpTime)
        public float charge = 0f;
        //Time it takes to recharge for another shot
        public float beamRechargeTime = 3f;
        //How much time it takes for the AMT to charge before firing
        public float beamChargeUpTime = 2f;
        //The rate at which the beamCharge gameobject scales in size
        public float beamChargeScale = 0.5f;
        //How much energy is taken away per shot
        public float beamEnergyUsed = 50f;
        //Float that represents the beamCharge gameobject's original scale (used for resetting its scale)
        public float beamChargeOriginalScale = 0.1f;
        //How much energy is regenerated per second
        public float beamEnergyRegen = 2.5f;
        //How long the beam is fired for
        public float beamUpTime = 0.25f;
        //How long the beamDetect is active for
        public float beamDetectUpTime = 0.5f;
        //How long the beam is
        public float beamLength = 5f;

        [Header("AMT Objects")]
        //The Transform object representing the Beam
        private Transform beam;
        //The Transform object representing the Beam's charging animation
        private Transform beamCharge;
        //The Transform object representing the Beam's additional hitbox (Added for reliability with onTriggerEnter)
        private Transform beamDetect;

        public bool isFiring = false;
        public bool isClapping = false;

        // Start is called before the first frame update
        void Start()
        {
            //Find the Beam, Beam Charge, and Beam Detect Transforms within the AMT parent and assign them to the beam, beamCharge, and beamDetect Transforms respectively
            beam = transform.GetChild(1);
            beamCharge = transform.GetChild(2);
            beamDetect = transform.GetChild(3);

            //Adjust the size and position of the beam and beamDetect Transforms based on the beamLength variable
            AdjustBeamLength(beamLength);

            //Set the beam, beamCharge, and beamDetect gameObjects active state to false
            beam.gameObject.SetActive(false);
            beamCharge.gameObject.SetActive(false);
            beamDetect.gameObject.SetActive(false);

            //Set the beginning energy reserve to the maximum energyReserve variable
            energy = energyReserve;
        }

        public void startFiring()
        {
            isFiring = true;
        }
        public void stopFiring()
        {
            isFiring = false;
        }
        public void startClapping()
        {
            isClapping = true;
        }

        public void stopClapping()
        {
            isClapping = false;
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
        void Update()
        {
            //Assign the left mouse button(LMB) to a boolean to obtain its current state (to be switched with SteamVR input)
            //bool isFiring = Input.GetKey(KeyCode.Mouse0);

            //Assign the right mouse button(RMB) to a boolean to obtain its current state (to be switched with SteamVR input)
            //bool isClapping = Input.GetKey(KeyCode.Mouse1);

            //Variable that is always counting up, used for building up the charge over time
            float buildingCharge = Time.deltaTime;

            //IF the player isn's holding down either main mouse button, the player can attempt to fire another beam or clap
            if (isClapping == false && isFiring == false)
            {
                canFire = true;
            }

            //If the RMB is down, the AMT is being held, it isn't charging from a previous shot/clap, and the energy is greater or equal to the clapEnergyUsed variable
            if (isClapping && isHeld && canFire && (energy >= clapEnergyUsed))
            {
                //Start the Clap() coroutine
                StartCoroutine(Clap());
            }

            //If LMB is down, the AMT is being held, it isn't recharging from a previous shot/clap, and the energy is greater or equal to the beamEnergyUsed variable
            if (isFiring && isHeld && canFire && (energy >= beamEnergyUsed))
            {

                //Set the beamCharge gameobject's active state to true
                beamCharge.gameObject.SetActive(true);

                //Increase the charge of the AMT using the incrementing buildingCharge variable stated previously
                charge += buildingCharge;

                //Increment the beamCharge gameobject's scale by the beamChargeScale float multiplied by Time.deltaTime
                beamCharge.localScale += new Vector3(beamChargeScale * Time.deltaTime, beamChargeScale * Time.deltaTime, beamChargeScale * Time.deltaTime);

                //If the charge time of the AMT is greater or equal to beamChargeTime, then call the Fire function
                if (charge >= beamChargeUpTime)
                {
                    Fire();
                }
            }
            else
            {
                //Set the beamCharge gameobject's active state to false
                beamCharge.gameObject.SetActive(false);

                //Set the AMT's charge back to 0
                charge = 0;

                //If the AMT's energy is less-than 100
                if (energy < 100)
                {
                    //Increase energy at a rate of beamRegen per second
                    energy += Time.deltaTime * beamEnergyRegen;
                }
                //Otherwise, set the AMT's energy to the max energyReserve value
                else
                {
                    energy = energyReserve;
                }
            }
        }

        //This function fires the AMT, quite self-explanatory
        public void Fire()
        {

            //Set the active state of the beam gameobject and the beamDetect gameobject to true
            beam.gameObject.SetActive(true);
            beamDetect.gameObject.SetActive(true);

            //Start the FireBeam coroutine
            StartCoroutine(FireBeam());

            //This coroutine controls the events that occur when the AMT is fired
            IEnumerator FireBeam()
            {

                //Set the canFire boolean to false
                canFire = false;

                //Reduce the total energy count by the beamEnergyUsed variable
                energy -= beamEnergyUsed;

                //Reset the beamCharge gameobject's scale to the beamChargeOrigScale variable
                beamCharge.localScale = new Vector3(beamChargeOriginalScale, beamChargeOriginalScale, beamChargeOriginalScale);

                //Set the active state of the beamCharge gameobject to false
                beamCharge.gameObject.SetActive(false);

                //Wait a certain amount of time represented within the beamUpTime variable
                yield return new WaitForSeconds(beamUpTime);

                //Set the active state of the beam gameobject to false
                beam.gameObject.SetActive(false);

                StartCoroutine(BeamDetectUptime());

                //Wait 3 seconds
                yield return new WaitForSeconds(beamRechargeTime);

                //Reset the charge float back to 0
                charge = 0;

                //Set the canFire boolean back to true
                //canFire = true;
            }

            IEnumerator BeamDetectUptime()
            {
                //Wait a certain amount of time within the beamDetectUpTime variable
                yield return new WaitForSeconds(beamDetectUpTime);

                //Set the active state of the beam gameobject to false
                beamDetect.gameObject.SetActive(false);
            }
        }

        IEnumerator Clap()
        {
            //Set the canFire boolean to false
            canFire = false;

            //Reduce the total energy count by the clapEnergyUsed variable
            energy -= clapEnergyUsed;

            //Add Sound Emitting code here
            particles.Play();

            //Wait 3 seconds
            yield return new WaitForSeconds(clapRechargeTime);

        }

        //This function adjusts the beam and beamDetect lengths using a common variable
        private void AdjustBeamLength(float newSize)
        {
            //Float that represents the equation to properly position each object using the newSize variable
            float beamPosAdjust = 1f + (0.5f * newSize);

            //Set the beam gameObject's size and position using the beamPosAdjust variable
            beam.localScale = new Vector3(0.1f, 0.1f, newSize);
            beam.localPosition = new Vector3(0, 0, -beamPosAdjust);

            //Set the beamDetect gameObject's size and position using the beamPosAdjust variable
            beamDetect.localScale = new Vector3(0.5f, 0.5f, newSize);
            beamDetect.localPosition = new Vector3(0, 0, -beamPosAdjust);
        }
    }

}
