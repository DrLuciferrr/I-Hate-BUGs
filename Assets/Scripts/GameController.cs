using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    //������ �� ������ ������ ��� �������������� � ��� ��������
    [SerializeField] private Player _player;

    //������ �� ������ �������� ������
    [Header("������� ���� ������")]
    [Space] 
        public GameObject[] Enemies_Prefabs = new GameObject[5];

    //List(������������ ������) � ������ ������� (����������� � ���� ����� ����� � ������� ����, ��������� ��� ������)
    [Header("������ ����� �� ������ ������ ������")]
    [Space] 
        public List<GameObject> Enemies_Alive = new List<GameObject>();

    //������ �� ���������� ������������ ����
    private GameObject lastSpawnedEnemy;

    [Header("����������� ����")]
    [Space] 
        public WaveList WaveList = new WaveList();

    [HideInInspector]
    public enum SpawnedEnemyType
    {
        Crum,
        Crum_Glitch,
        Fly,
        Fly_Glitch,
        Cockroach,
        Cockroach_Glitch,
        Wood_Louse,
        Wood_Louse_Glitch
    }

    [Header("�������� ����")]
    [Space]
        [Tooltip("����� ����� ������� � �������� �����")]
        [SerializeField] private float spawnDelay;

        [Tooltip("����� ����� �������")]
        [SerializeField] private float waveDelay;

    //���������� ���
    [Header("��� �������")]
    [Space]
        [Tooltip("����� ����� ������")]
        [SerializeField] private float tickTime;

        [Tooltip("���������")]
        [SerializeField] private float tickModificator;
        
        private float tickValue;
        [SerializeField] private int ID;

    //������� ����� ��������� ���������� ����� ��� �������������� � ���
    RandomPoint randomPoint = new RandomPoint();

    private void Awake()
    {
        //������ �������� ��������
        StartCoroutine(Spawner());

        //������ �������� ���������� �� ��������� ������� ������� �� ���-�� ����� �����
        StartCoroutine(StressTick());
    }

    /*private void Start()
    {
        BGM(ID);
    }

    private void BGM(int ID)
    {

        switch (ID)
        {
            case 0:
                break;
            case 1:
                SoundManagerScript.PlaySound("Level_1");
                break;
            case 2:
                SoundManagerScript.PlaySound("Level_2");
                break;
            case 3:
                SoundManagerScript.PlaySound("Level_3");
                break;
            case 4:
                SoundManagerScript.PlaySound("Level_4");
                break;
        }

    }*/
    //�������� �������� (��� � tickTime �������) ������������� ������ � ����������� �� ���-�� ����� �����
    private IEnumerator StressTick()
    {
        if (Enemies_Alive.Count > 0)
        {
            if (Enemies_Alive.Count <= 2)
                tickValue = tickModificator;
            else
                tickValue = Mathf.Log(Enemies_Alive.Count) * tickModificator;
            _player.StressChange(tickValue);
        }
        yield return new WaitForSecondsRealtime(tickTime);
        Debug.Log("TICK " + tickValue);
        StartCoroutine("StressTick");
    }

    //����� ��� ������ �����. ��������� � ����: 1) ��� �����, 2) ����� ������, 3) ������� ������� �����
    public void Spawn(GameObject enemyType, Vector2 position, Quaternion rotation)
    {
        lastSpawnedEnemy = Instantiate(enemyType, position, rotation);
    }
    
    //������� �������
    private IEnumerator Spawner()
    {
        yield return new WaitUntil(() => Input.GetKeyDown("space"));

        int currentWave = 0;
        foreach (Wave wave in WaveList.Waves)
        {
            foreach (SpawnedEnemyType enemy in WaveList.Waves[currentWave].EnemyInWave)
            {
                switch (enemy)
                {
                    case SpawnedEnemyType.Crum:
                        Spawn(Enemies_Prefabs[0], randomPoint.InSpawnZone(), Quaternion.identity);
                        break;

                    case SpawnedEnemyType.Crum_Glitch:
                        Spawn(Enemies_Prefabs[0], randomPoint.InSpawnZone(), Quaternion.identity);
                        lastSpawnedEnemy.GetComponent<Enemy>().isGlitch = true;
                        break;

                    case SpawnedEnemyType.Fly:
                        Spawn(Enemies_Prefabs[1], randomPoint.InSpawnZone(), Quaternion.identity);
                        break;

                    case SpawnedEnemyType.Fly_Glitch:
                        Spawn(Enemies_Prefabs[1], randomPoint.InSpawnZone(), Quaternion.identity);
                        lastSpawnedEnemy.GetComponent<Enemy>().isGlitch = true;
                        break;

                    case SpawnedEnemyType.Cockroach:
                        Spawn(Enemies_Prefabs[2], randomPoint.InSpawnZone(), Quaternion.identity);
                        break;

                    case SpawnedEnemyType.Cockroach_Glitch:
                        Spawn(Enemies_Prefabs[2], randomPoint.InSpawnZone(), Quaternion.identity);
                        lastSpawnedEnemy.GetComponent<Enemy>().isGlitch = true;
                        break;

                    case SpawnedEnemyType.Wood_Louse:
                        Spawn(Enemies_Prefabs[3], randomPoint.InSpawnZone(), Quaternion.identity);
                        break;

                    case SpawnedEnemyType.Wood_Louse_Glitch:
                        Spawn(Enemies_Prefabs[3], randomPoint.InSpawnZone(), Quaternion.identity);
                        lastSpawnedEnemy.GetComponent<Enemy>().isGlitch = true;
                        break;
                }
                yield return new WaitForSecondsRealtime(spawnDelay);
            }
            yield return new WaitForSecondsRealtime(waveDelay);
            currentWave++;
        }
        
    }
    
    
}

//2 ������ ��� �������� ������� �� ������ � ����� �� � ���������
[System.Serializable]
public class Wave
{    
    public List<GameController.SpawnedEnemyType> EnemyInWave; 
}

[System.Serializable]
public class WaveList
{  
    public List<Wave> Waves;    
}


//������� ���� ����� ��� �������� ��������� ���������� �����
public class RandomPoint
{
    private bool isCorrect = false;

    //� ����� ���� (���, �� ��������, ������)
    public Vector3 InSpawnZone()
    {
        Vector3 point;
        do
        {
            point = new Vector2(Random.Range(-6.5f, 6.5f), Random.Range(-5.75f, 5.75f));
            if (Vector2.Distance(point, Vector2.zero) >= 5.5 && Vector2.Distance(point, Vector2.zero) <= 6)
                isCorrect = true;
        } while (!isCorrect);
        isCorrect = false;
        return point;  
    }

    //� ������� ���� (��� ����� ��������)
    public Vector3 InGameZone()
    {
        Vector3 point = new Vector2(Random.Range(-4.5f, 4.5f), Random.Range(-3f, 3f));
        return point;
    }
}
