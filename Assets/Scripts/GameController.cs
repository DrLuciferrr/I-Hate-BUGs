using UnityEngine;

public class GameController : MonoBehaviour
{
    public int enemyCount;
    [SerializeField] private GameObject enemy;
    [SerializeField] private Player player;
    private GameObject spawnedEnemy;

    private int spaceClicks = 0;
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
}
