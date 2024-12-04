using System.Collections.Generic;
using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace LilLycanLord_Official
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AINavAgent : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        NavMeshAgent navMeshAgent;

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        // [Header("Displays")]

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        // [Space(10)]
        // [Header("Fields")]
        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Update() { }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public void MoveAgent(Vector3 destination, float speed = 1.0f)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = speed;
            navMeshAgent.destination = destination;
        }

        public void StopAgent()
        {
            navMeshAgent.isStopped = true;
        }
    }
}
