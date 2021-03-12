using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AMT_Hud : MonoBehaviour
{

    public Slider energy;
    public Slider charge;

    public void SetEnergy(float currEnergy)
    {
        energy.value = currEnergy;
    }

    public void SetCharge(float currCharge)
    {
        charge.value = currCharge;
    }
}
