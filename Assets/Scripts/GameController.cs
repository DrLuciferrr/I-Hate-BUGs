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

    [SerializeField] private float tickTime;
    [SerializeField] private float mod_Tick;
    private float tickValue;

    private void Awake()
    {
        StartCoroutine("StressTick");
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
    }

    
    private IEnumerator StressTick()
    {
        if (enemyCount > 0)
        {
            tickValue = Mathf.Log(enemyCount) * mod_Tick;
            player.StressChange(tickValue);
        }
        yield return new WaitForSecondsRealtime(tickTime);
        Debug.Log("TICK " + tickValue);
        StartCoroutine("StressTick");
    }
    
}
