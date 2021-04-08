// using UnityEngine;
//
// namespace AudioGame.Scripts.AI
// {
//     public class NewDAVEChaser : IDaveState
//     {
//         DAVE thisDave;
//         
//
//         /// <summary>
//         /// Fields about Ping info
//         /// </summary>
//
//         #region PingInfo
//
//         float timeSinceLastPing;
//
//         bool bIsWaitingAtLocation = false;
//
//         private float pingWaitUntilFinishedTime = 2f;
//
//
//         private float noiseStartWaitTime;
//
//         #endregion PingInfo
//
//
//         public void OnStateEnter(DAVE bigDave)
//         {
//             Debug.Log("<color=red>Entering: Chaser</color>");
//             thisDave = dave;
//             thisDave.waitingAtLocation = false;
//             TravelToSuspectedPlayerPos(thisDave.lastKnownPlayerLocation);
//             thisDave.statusLight.color = thisDave.chaserModeColor;
//             thisDave.ArrivedAtDestination += ReachedOldPlayerPosPing;
//         }
//
//         public void OnStateUpdate(DAVE bigDave)
//         {
//             var doneWaitingOnInvestigation = bIsWaitingAtLocation && Time.time >= noiseStartWaitTime + pingWaitUntilFinishedTime;
//             if (doneWaitingOnInvestigation)
//             {
//                 //  Debug.Log("Done Investiagting");
//                 // Only after this waiting period, check to see if the ping hit anything
//                 if (thisDave.pingFoundPlayer)
//                 {
//                     Debug.Log("Found Player Again");
//                     bIsWaitingAtLocation = false;
//                     // As soon as its true, set false
//                     thisDave.pingFoundPlayer = false;
//                     TravelToSuspectedPlayerPos(thisDave.lastKnownPlayerLocation);
//                 }
//                 else
//                 {
//                     Exit();
//                 }
//             }
//         }
//
//         public void OnStateMove(DAVE bigDave)
//         {
//         }
//
//         public void OnStateExit(DAVE bigDave)
//         {
//         }
//     }
// }