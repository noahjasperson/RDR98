using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public int spread;
    public int initialEnemyCount;
    private MeshRenderer renderer;
    private int xPos;
    private int yPos;
    private int currentEnemyCount;

    private void Awake()
    {
        renderer = this.gameObject.GetComponent<MeshRenderer>();
        renderer.enabled = false;
    }

    void Start()
    {
        StartCoroutine(spawnEnemies());
    }

    IEnumerator spawnEnemies()
    {
        while (currentEnemyCount < initialEnemyCount)
        {
            xPos = (int)(Random.Range(-spread, spread) + this.transform.position.x);
            yPos = (int)(Random.Range(-spread, spread) + this.transform.position.z);
            Instantiate(enemy, new Vector3(xPos, 0, yPos), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            currentEnemyCount += 1;
        }
    }
}
