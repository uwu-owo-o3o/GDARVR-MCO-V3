using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;

namespace LilLycanLord_Official
{
    public class AIBeacon : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        // [Header("Displays")]

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        Vector3 beaconDirection = Vector3.up;

        [SerializeField]
        string targetNavMeshLayerTag;

        [SerializeField]
        LayerMask navMeshLayer;

        public float agentSpeed = 5.0f;

        [SerializeField]
        float updatesPerSecond = 1.0f;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        float updateTimer = 0.0f;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start() { }

        void Update()
        {
            if (updateTimer >= (1.0f / updatesPerSecond))
            {
                MoveAgentsToBeacon();
                updateTimer = 0.0f;
            }
            else
                updateTimer += Time.deltaTime;
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        void MoveAgentsToBeacon()
        {
            if (targetNavMeshLayerTag == "" || navMeshLayer == 0)
                return;
            Debug.DrawRay(transform.position, beaconDirection);
            if (
                Physics.Raycast(
                    transform.position,
                    beaconDirection,
                    out RaycastHit hit,
                    navMeshLayer
                )
            )
            {
                Debug.Log(hit.collider.gameObject.name);
                Debug.Log("awa" + hit.collider.gameObject.layer + targetNavMeshLayerTag);
                if (hit.collider.gameObject.tag == targetNavMeshLayerTag)
                    if (
                        hit.collider.gameObject.TryGetComponent<NavAgentManager>(
                            out NavAgentManager agentManager
                        )
                    )
                        agentManager.MoveAllAgents(hit.point, agentSpeed);
            }
        }
    }
}
