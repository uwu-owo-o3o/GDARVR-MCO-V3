using System.Collections.Generic;
using System.Linq;
using LilLycanLord_Official;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace LilLycanLord_Official
{
    public class NavAgentManager : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        [SerializeField]
        TrackablePoolManager trackablePoolManager;

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        [SerializeField]
        List<AINavAgent> navAgents = new List<AINavAgent>();

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        InputMode inputMode;
        public bool enableDestinationTargeting = true;

        [SerializeField]
        bool getAllNavAgentsAtStart = true;

        [SerializeField]
        LayerMask destinationLayer = 0;

        [SerializeField]
        float agentSpeed = 1.0f;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start()
        {
            if (trackablePoolManager == null)
                trackablePoolManager = GetComponent<TrackablePoolManager>();
            if (trackablePoolManager == null)
                trackablePoolManager = FindObjectsByType(
                    typeof(TrackablePoolManager),
                    FindObjectsSortMode.InstanceID
                )[0].GetComponent<TrackablePoolManager>();
            if (inputMode == InputMode.Individual)
            {
                Debug.LogWarning(
                    "The Individual input mode is reserved for the TrackablePoolManager; Do not use."
                );
                inputMode = InputMode.PC;
            }
            if (trackablePoolManager.globalInputMode != InputMode.Individual)
                inputMode = trackablePoolManager.globalInputMode;
            if (getAllNavAgentsAtStart)
            {
                List<UnityEngine.Object> agents = new List<UnityEngine.Object>();
                agents = FindObjectsByType(typeof(AINavAgent), FindObjectsSortMode.InstanceID)
                    .ToList();
                foreach (UnityEngine.Object agent in agents)
                    navAgents.Add(agent.GetComponent<AINavAgent>());
            }
        }

        void Update()
        {
            Ray inputRay = new Ray();
            switch (inputMode)
            {
                case InputMode.PC:
                    if (Input.GetMouseButtonDown(0))
                        inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    break;
                case InputMode.Mobile:
                    inputRay = trackablePoolManager.CheckTouch();
                    break;
                default:
                    break;
            }
            // if (inputRay.origin != Vector3.zero)
            //     if (destinationLayer == 0)
            //         MoveAllAgents(
            //             trackablePoolManager
            //                 .RaycastTillTrackable(
            //                     inputRay,
            //                     UnityEngine.XR.ARSubsystems.TrackableType.Planes
            //                 )
            //                 .pose.position,
            //             agentSpeed
            //         );
            //     else if (Physics.Raycast(inputRay, out RaycastHit hit))
            //     {
            //         Debug.Log("Agents moving to" + hit.point);
            //         MoveAllAgents(hit.point, agentSpeed);
            //     }
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public void MoveAllAgents(Vector3 destination, float speed = 1.0f)
        {
            foreach (AINavAgent navAgent in navAgents)
                navAgent.MoveAgent(destination, speed);
        }

        [ContextMenu("Stop All Agents")]
        public void StopAllAgents()
        {
            foreach (AINavAgent navAgent in navAgents)
                navAgent.StopAgent();
        }
    }
}
