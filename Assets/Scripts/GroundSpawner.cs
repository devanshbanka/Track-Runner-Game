using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public GameObject groundTilePrefab;
    public bool noObs = false;
    Vector3 nextSpawnpoint;

    public void spawnTile()
    {
        Debug.Log("CALLED SPAWNTILE");

        GameObject tempGround = Instantiate(groundTilePrefab, nextSpawnpoint, Quaternion.identity);
        nextSpawnpoint = tempGround.transform.GetChild(1).transform.position;        

        if (GameManager.MyInstance.DifficultyMode == 0)
        {
            noObs = !noObs;
        }

        Debug.Log($"Spawning tile with noObs={noObs}");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("CALLED START");

        if (GameManager.MyInstance.DifficultyMode == 0)
        {
            noObs = true;
        }
    }
}
