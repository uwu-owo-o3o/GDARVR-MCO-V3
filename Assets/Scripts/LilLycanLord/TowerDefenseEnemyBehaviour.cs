using LilLycanLord_Official;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace LilLycanLord_Official
{
    [RequireComponent(typeof(BoxCollider))]
    public class TowerDefenseEnemyBehaviour : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        BoxCollider boxCollider;

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        // [Header("Displays")]

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        public UnityEvent onHit;

        [SerializeField]
        int healthOnDefeat = 5;

        [SerializeField]
        int damageOnHit = 1;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        SampleHealth playerHealth;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        void Update() { }

        void OnTriggerEnter(Collider other)
        {
            playerHealth = FindObjectsByType(typeof(SampleHealth), FindObjectsSortMode.InstanceID)[
                0
            ].GetComponent<SampleHealth>();
            if (other.gameObject.tag == "Turret")
            {
                if (
                    other.transform.parent.TryGetComponent<TurretBehaviour>(
                        out TurretBehaviour turretBehaviour
                    )
                )
                {
                    turretBehaviour.onHit.Invoke();
                    playerHealth.TakeDamage(damageOnHit);
                    Die();
                }
            }
            else if (other.gameObject.tag == "Bullet")
            {
                onHit.Invoke();
                playerHealth.HealDamage(healthOnDefeat);
                Destroy(other.gameObject);
            }
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
