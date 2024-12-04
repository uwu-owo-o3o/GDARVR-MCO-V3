using System;
using LilLycanLord_Official;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace LilLycanLord_Official
{
    public class SampleHealth : MonoBehaviour
    {
        //* ╔════════════╗
        //* ║ Components ║
        //* ╚════════════╝
        [SerializeField]
        TMP_Text healthText;

        //* ╔══════════╗
        //* ║ Displays ║
        //* ╚══════════╝
        [Header("Displays")]
        public int health = 100;

        //* ╔════════╗
        //* ║ Fields ║
        //* ╚════════╝
        [Space(10)]
        [Header("Fields")]
        [SerializeField]
        string healthPrefix = "Health: ";

        [SerializeField]
        int maxHealth = 100;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        void Awake() { }

        void Start() { }

        void Update()
        {
            if (health <= 0)
                Debug.Log("Player has no more health");
            healthText.text = healthPrefix + health.ToString();
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        public int TakeDamage(int damage)
        {
            health = Mathf.Clamp(health - damage, 0, maxHealth);
            return health;
        }

        public int HealDamage(int damage)
        {
            health = Mathf.Clamp(health + damage, 0, maxHealth);
            return health;
        }
    }
}
