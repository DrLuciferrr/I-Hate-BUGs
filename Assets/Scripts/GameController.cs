using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    //Ссылка на скрипт игрока для взаимодействия с его методами
    [SerializeField] private Player _player;

    //Ссылка на массив префабов врагов
    [Header("Префабы всех врагов")]
    [Space] 
        public GameObject[] Enemies_Prefabs = new GameObject[5];

    //List(динамический массив) с живыми врагами (добавляются в него после входа в игровую зону, удаляются при смерти)
    [Header("Список живых на данный момент врагов")]
    [Space] 
        public List<GameObject> Enemies_Alive = new List<GameObject>();

    //Ссылка на последнего заспауненого моба
    private GameObject lastSpawnedEnemy;

    [Header("Конструктор волн")]
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

    [Header("Тайминги волн")]
    [Space]
        [Tooltip("Время между врагами в пределах волны")]
        [SerializeField] private float spawnDelay;

        [Tooltip("Время между волнами")]
        [SerializeField] private float waveDelay;

    //Переменные для
    [Header("Тик стресса")]
    [Space]
        [Tooltip("Время между тиками")]
        [SerializeField] private float tickTime;

        [Tooltip("Множитель")]
        [SerializeField] private float tickModificator;
        
        private float tickValue;
        [SerializeField] private int ID;

    //Создаем новый экземпляр генератора точек для взаимодействия с ним
    RandomPoint randomPoint = new RandomPoint();

    private void Awake()
    {
        //Запуск корутины Спаунера
        StartCoroutine(Spawner());

        //Запуск корутины отвечающей за пассивный прирост стресса от кол-ва живых жуков
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
    //Корутина пассивно (раз в tickTime секкунд) увеличивающая стресс в зависимости от кол-ва живых жуков
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

    //Метод для спауна врага. Принимает в себя: 1) Тип врага, 2) Точку спауна, 3) Базовый поворот врага
    public void Spawn(GameObject enemyType, Vector2 position, Quaternion rotation)
    {
        lastSpawnedEnemy = Instantiate(enemyType, position, rotation);
    }
    
    //Спаунер волнами
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

//2 класса для создания таблицы из таблиц и вывод их в инспектор
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


//Сочинил свой класс для создания рандомной подходящей точки
public class RandomPoint
{
    private bool isCorrect = false;

    //В спаун зоне (для, не поверишь, спауна)
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

    //В игровой зоне (для точек маршрута)
    public Vector3 InGameZone()
    {
        Vector3 point = new Vector2(Random.Range(-4.5f, 4.5f), Random.Range(-3f, 3f));
        return point;
    }
}
