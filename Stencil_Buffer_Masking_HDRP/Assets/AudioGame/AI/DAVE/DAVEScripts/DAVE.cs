using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IDaveState
{
    void Initialize(DAVE dave);
    void UpdateCycle(DAVE dave);
}
public class DAVE : MonoBehaviour
{
    public GameObject[] patrolPathNodes;
    
    public ActiveSound[] soundsToInvestigate;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
