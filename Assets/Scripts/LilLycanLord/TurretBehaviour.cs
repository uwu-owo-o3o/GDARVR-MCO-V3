using LilLycanLord_Official;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace LilLycanLord_Official
{
    public class TurretBehaviour : MonoBehaviour
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
        [Min(0)]
        int maxEnemiesOnLane = 1;

        [SerializeField]
        float spawnIntervalInSeconds = 2.0f;

        [SerializeField]
        int spawnChancePerInterval = 3;

        [SerializeField]
        GameObject enemyPrefab;

        [SerializeField]
        GameObject enemySpawnPoint;

        [SerializeField]
        [Min(0.001f)]
        float enemySpeed = 0.1f;

        [SerializeField]
        GameObject turret;
        public UnityEvent onHit;

        [SerializeField]
        [Min(0.0f)]
        float shootingCooldown = 0.5f;

        [SerializeField]
        [Min(0.001f)]
        float bulletSpeed = 0.5f;

        [SerializeField]
        float bulletScale = 0.1f;

        [SerializeField]
        Transform barrelPosition;

        //* ╔════════════╗
        //* ║ Attributes ║
        //* ╚════════════╝
        GameObject bulletPool;

        //* ╔═══════════════╗
        //* ║ Monobehaviour ║
        //* ╚═══════════════╝
        float spawnTimer = 0.0f;
        float shootingTimer = 0.0f;

        void Awake() { }

        void Start()
        {
            bulletPool = new GameObject("Bullet Pool");
            bulletPool.transform.parent = turret.transform;
        }

        void Update()
        {
            enemySpawnPoint.transform.LookAt(turret.transform.position);
            if (spawnTimer >= spawnIntervalInSeconds)
            {
                if (Random.Range(1, spawnChancePerInterval) == 1)
                    SpawnEnemy();
                spawnTimer = 0;
            }
            else
                spawnTimer += Time.deltaTime;

            if (shootingTimer > 0.0f)
                shootingTimer -= Time.deltaTime;
            MoveEnemies();
            MoveBullets();
        }

        //* ╔═════════════════════╗
        //* ║ Non - Monobehaviour ║
        //* ╚═════════════════════╝
        void SpawnEnemy()
        {
            if (enemySpawnPoint.transform.childCount >= maxEnemiesOnLane)
                return;
            GameObject newEnemy = GameObject.Instantiate(enemyPrefab, enemySpawnPoint.transform);
            newEnemy.transform.position = enemySpawnPoint.transform.position;
            newEnemy.transform.rotation = enemySpawnPoint.transform.rotation;
        }

        void MoveEnemies()
        {
            if (enemySpawnPoint.transform.childCount <= 0)
                return;
            foreach (Transform enemy in enemySpawnPoint.transform)
                enemy.position += enemySpawnPoint.transform.forward * (enemySpeed * Time.deltaTime);
        }

        [ContextMenu("Fire Bullet")]
        public void FireBullet()
        {
            if (shootingTimer > 0.0f)
                return;
            GameObject newBullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newBullet.AddComponent<BoxCollider>().size = newBullet.transform.localScale;
            newBullet.transform.parent = bulletPool.transform;
            newBullet.transform.position = barrelPosition.position;
            newBullet.transform.localScale *= bulletScale;
            newBullet.transform.localScale *= transform.localScale.x;
            newBullet.tag = "Bullet";

            shootingTimer = shootingCooldown;
        }

        void MoveBullets()
        {
            if (bulletPool.transform.childCount <= 0)
                return;
            foreach (Transform bullet in bulletPool.transform)
                bullet.position += turret.transform.forward * (bulletSpeed * Time.deltaTime);
        }
    }
}
