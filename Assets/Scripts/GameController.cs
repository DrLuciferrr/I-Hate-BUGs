using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    //List(������������ ������) � ������ ������� (����������� � ���� ����� ����� � ������� ����, ��������� ��� ������)
    public List<GameObject> Enemys_Alive = new List<GameObject>();

    //������ �� ������ ������ ��� �������������� � ��� ��������
    [SerializeField] private Player player;

    //������ �� ������ �������� ������
    [SerializeField] private GameObject[] Enemys_Prefabs = new GameObject[5];

    //������ �� ���������� ������������ ����
    private GameObject lastSpawnedEnemy;

    //���������� ���
    [SerializeField] private float tickTime;
    [SerializeField] private float tickModificator;
    private float tickValue;

    //������� ����� ��������� ���������� ����� ��� �������������� � ���
    RandomPoint randomPoint = new RandomPoint();

    //������ ����� ���������� ��������
    private int spaceClicks = 0;

    private void Awake()
    {
        //������ �������� ���������� �� ��������� ������� ������� �� ���-�� ����� �����
        StartCoroutine("StressTick");
    }
    private void Update()
    {
        //������ ����� ���������� ��������, �������
        if (Input.GetKeyDown("space"))
        {
            spaceClicks++;
            Spawn(Enemys_Prefabs[0], randomPoint.InSpawnZone(), Quaternion.identity);
        }
    }

    //����� ��� ������ �����. ��������� � ����: 1) ��� �����, 2) ����� ������, 3) ������� ������� �����
    public void Spawn(GameObject enemyType, Vector2 position, Quaternion rotation)
    {
        lastSpawnedEnemy = Instantiate(enemyType, position, rotation);

        //�������� ����� ���������� ��������(������ ������ 3�� ���� = ����, ������������� �� ����� �������
        if (spaceClicks == 3)
        {
            lastSpawnedEnemy.GetComponent<Enemy>().isGlitch = true;
            spaceClicks = 0;
        }
    }

    //�������� �������� (��� � tickTime �������) ������������� ������ � ����������� �� ���-�� ����� �����
    private IEnumerator StressTick()
    {
        if (Enemys_Alive.Count > 0)
        {
            if (Enemys_Alive.Count == 1)
                tickValue = tickModificator;
            else
                tickValue = Mathf.Log(Enemys_Alive.Count) * tickModificator;
            player.StressChange(tickValue);
        }
        yield return new WaitForSecondsRealtime(tickTime);
        Debug.Log("TICK " + tickValue);
        StartCoroutine("StressTick");
    }
    
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
            point = new Vector2(Random.Range(-8.5f, 8.5f), Random.Range(-4.75f, 4.75f));
            if (point.x <= 8.5 && point.x >= 5.5 || point.x <= -5.5 && point.x >= -8.5)
                if (point.y <= 4.75 && point.y >= 3.75 || point.y <= -3.75 && point.y >= -4.75)
                {
                    isCorrect = true;
                }
                    
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
