using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAVEChaser : IDaveState
{
    // Start is called before the first frame update
    public void Initialize(DAVE dave)
    {
        Debug.Log("Player Found: Initialized Chaser");
    }

    // Update is called once per frame
    public void UpdateCycle(DAVE dave)
    {
    }


    public void Exit()
    {
        
    }
}
