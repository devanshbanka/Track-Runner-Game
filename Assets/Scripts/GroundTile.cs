using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
    private GroundSpawner groundspawner;

    private PlayerController playercontroller;

    public GameObject CoinPrefab;
    public GameObject StarPrefab;
    public GameObject MagnetPrefab;
    public GameObject[] obstaclePrefabs;
    public Transform[] spawnpoints;

    private GameObject PowerupPrefab;

    List<int> obsPos = new List<int>();
    List<Vector3> coinPos = new List<Vector3>();
    List<Rigidbody> Coins = new List<Rigidbody>();

    public int obsSpawnAmount = 1;
    public int coinSpawnAmount = 2;

    private bool playerOnTile = false;

    private void Awake()
    {
        Debug.Log("1");

        groundspawner = GameObject.FindObjectOfType<GroundSpawner>();
        playercontroller = GameObject.FindObjectOfType<PlayerController>();

        switch (GameManager.MyInstance.DifficultyMode)
        {
            case 0:
                Debug.Log("Easy");
                obsSpawnAmount = 2;
                break;
            case 1:
                Debug.Log("Medium");
                obsSpawnAmount = 1;
                break;
            case 2:
                Debug.Log("Hard");
                obsSpawnAmount = 2;
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("2");

        if (GameManager.MyInstance.DifficultyMode == 0)
        {
            Debug.Log("EASY MODE REACHED" + groundspawner.noObs);

            if (groundspawner.noObs == false)
            {
                Debug.Log("SPAWN OBJ");
                SpawnObs();
            }
        }
        else
        {
            SpawnObs();
        }
        SpawnCoin();
        SpawnPowerUp();
    }

    void Update()
    {
        if (playerOnTile && playercontroller.magnetActive)
        {
            MoveCoinsTowardsPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnTile = false;
        }
        groundspawner.spawnTile();
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnTile = true;
            Debug.Log("Player entered tile, magnetActive: " + playercontroller.magnetActive);
        }
    }

    private void MoveCoinsTowardsPlayer()
    {
        Vector3 playerPosition = playercontroller.player.transform.position;

        foreach (Rigidbody coin in Coins)
        {
            if (coin != null)
            {
                Vector3 direction = (playerPosition - coin.position).normalized;
                coin.MovePosition(coin.position + direction * 60f * Time.deltaTime);
                Debug.Log("Moving coin towards player: " + coin.position);
            }
        }
    }

    public void SpawnObs()
    {
        obsPos.Clear();
        for (int i = 0; i < obsSpawnAmount; i++)
        {
            int ChooseSpawnObsPoint = Random.Range(0, spawnpoints.Length);
            int SpawnPrefab = Random.Range(0, obstaclePrefabs.Length);

            if (!obsPos.Contains(ChooseSpawnObsPoint))
            {
                Instantiate(obstaclePrefabs[SpawnPrefab], spawnpoints[ChooseSpawnObsPoint].transform.position, Quaternion.identity, transform);
                obsPos.Add(ChooseSpawnObsPoint);
            }
            else
            {
                return;
            }
        }
    }

    public void SpawnCoin()
    {
        coinPos.Clear();
        Coins.Clear();
        for (int i = 0; i < coinSpawnAmount; i++)
        {
            int ChooseSpawnCoinPoint = Random.Range(0, spawnpoints.Length);
            GameObject tempCoin = Instantiate(CoinPrefab, transform);
            Rigidbody rb = tempCoin.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = tempCoin.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;
            tempCoin.transform.position = SpawnRandomPoint(spawnpoints[ChooseSpawnCoinPoint]);
            coinPos.Add(tempCoin.transform.position);
            Coins.Add(rb);
        }
    }

    public void SpawnPowerUp()
    {
        int ChoosePowerupPrefab = Random.Range(1, 3);
        if(ChoosePowerupPrefab == 1)
        {
            PowerupPrefab = StarPrefab;
        }
        else if(ChoosePowerupPrefab == 2)
        {
            PowerupPrefab = MagnetPrefab;
        }

        int random = Random.Range(0, 15);
        if (random == 1)
        {
            int ChooseSpawnPowerupPoint = Random.Range(0, spawnpoints.Length);
            GameObject tempPowerup = Instantiate(PowerupPrefab);
            Vector3 tempPowerupPos = SpawnRandomPoint(spawnpoints[ChooseSpawnPowerupPoint]);
            for (int i = 0; i < coinSpawnAmount; i++)
            {
                if (coinPos.Contains(tempPowerupPos))
                {
                    Destroy(tempPowerup);
                }
                else
                {
                    tempPowerup.transform.position = tempPowerupPos;
                }
            }
        }
    }

    Vector3 SpawnRandomPoint(Transform lanePosition)
    {
        Vector3 point = new Vector3(lanePosition.position.x, lanePosition.position.y, Random.Range(lanePosition.position.z + 13f, lanePosition.position.z + 2f));
        point.y = 1;
        return point;
    }
}
