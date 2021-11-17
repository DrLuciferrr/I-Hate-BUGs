using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    public int enemyCount;
    [SerializeField] private GameObject enemy;
    [SerializeField] private Player player;
    private GameObject spawnedEnemy;

    private int spaceClicks = 0;

    private void Awake()
    {
        //StartCoroutine("Test");
    }
    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            spaceClicks++;
            Spawn(enemy, new Vector2(0, -4), Quaternion.identity);
            if (spaceClicks == 3)
            {
                spawnedEnemy.GetComponent<Enemy>().isGlitch = true;
                spaceClicks = 0;
            }
        }
    }

    public void Spawn(GameObject enemyType, Vector2 position, Quaternion rotation)
    { 
        spawnedEnemy = Instantiate(enemy, position, rotation);
        enemyCount++;
    }

    /*
    private IEnumerator Test()
    {
        for (int i = 0; i < 60; i++)
        {
            Debug.Log("Moving");
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(5);
        Debug.Log("Rotating");
        StartCoroutine("Test");
    }
    */
}
