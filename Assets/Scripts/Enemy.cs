using AllIn1SpriteShader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerDownHandler
{
    /*Выбор типа противника из выпадающего списка в инспекторе
     * Crum - Клоп
     * Fly - Муха
     * Cockroach - Таракан
     * Wood_Louse - Мокрица
     */
    public enum EnemyType
    {
        Crum,
        Fly,
        Cockroach,
        Wood_Louse
    }

    [Header("Тип врага + определение жук/глич")]
    //Перемена для записи выбора в Enemy Type
        public EnemyType enemyType;
    
    //Определение жук/глич (false/true), Default: false;
        public bool isGlitch = false;

    /*Переменные характиристик врага
     * speed - скорость передвижения (в чем?)
     * stressFactor - базовое значение начисляемого страсса от жука/глича которое меняется модификаторами
     * clickToKill -  базовое значение ХП (количество кликов по жуку для убийства)
     */
    [Header("Хар-ки противника")]
    [Space]
        [Tooltip("Скорость")]
        [SerializeField] private float speed;
                         private float base_speed;

        [Tooltip("Стресс Фактор")]
        [SerializeField] private float stressFactor;

        [Tooltip("ХП")]
        [SerializeField] private float clickToKill;
                         private float base_clickToKill;

    [Header("Хар-ки глича")]
    [Space]
    //
    //    [Tooltip("Длительность эффекта глича")]
    //    [SerializeField] private float glitchDuration;

        [Tooltip("Сила эффекта глича(например множитель скоростиs у мухи)")]
        [SerializeField] private float glitchModify;
    

    //Переменная для отслеживания уже сделанных кликов по жуку
    private int currentClics = 0;

    //Переменная для отслеживания входа в игровую зону
    private bool insideGameZone = false;

    //Ссылки на необходимые компоненты(Скрипт игрока, геймконтроллера и генератора точек, физ. тело врага)
    private Player _player;
    private GameController _gameController;
    private RandomPoint _randomPoint;
    private Rigidbody2D _rigidbody;

    /* Модификаторы смены стресса: 
     * Fail     - при ошибке(ПКМ по жуку);
     * Glitch   - при срабатывании глича; 
     * Kill     - при убийсве жука;
     */

    [Header("Модификаторы событий")]
    [Space]

    [Tooltip("Ошибка при выявлении глича")]
    [SerializeField] private float mod_Fail;

    [Tooltip("Срабатывание эффекта глича")]
    [SerializeField] private float mod_Glitch;

    [Tooltip("Убийство жука/глича")]
    [SerializeField] private float mod_Kill;
    //PS. сделать константными с конкремными значениями 

    private Vector3 targetPoint;
    Vector3 direction;
    float rotationAngle;

    Material _shader;
    AllIn1Shader _in1Shader;
    Animator _animator;
    private void Awake()
    {
        //this.gameObject.AddComponent<AllIn1Shader>();
        //this.gameObject.GetComponent<AllIn1Shader>().CleanMaterial();
        //this.gameObject.GetComponent<AllIn1Shader>().shaderTypes = AllIn1Shader.ShaderTypes.Default;
        //this.gameObject.GetComponent<AllIn1Shader>().TryCreateNew();
        //Запоминание базовой скорости и ХП
        base_speed = speed;
        base_clickToKill = clickToKill;

        //Получение нужных компонентов
        _rigidbody = GetComponent<Rigidbody2D>();
        _player = FindObjectOfType<Player>();
        _gameController = FindObjectOfType<GameController>();
        _randomPoint = new RandomPoint();
    }

    private void Start()
    {
        
        FindNextTargetPoint();
        transform.Rotate(Vector3.forward, rotationAngle);
        _rigidbody.velocity = this.transform.up * speed;

        StartCoroutine(MovePattern());
    }

    //Метод отслеживания нажатия ПКМ или ЛКМ по врагу
    public void OnPointerDown(PointerEventData eventData)
    {
        if (insideGameZone)
        {
            //Отслеживание ЛКМ
            if (eventData.button == PointerEventData.InputButton.Left)
                LMBReact();

            //Отслеживание ПКМ
            if (eventData.button == PointerEventData.InputButton.Right)
                RMBReact();
        }
    }

    //  Реакция на ЛКМ.
    private void LMBReact()
    {
        //Если жук  -засчитываем клики до clickToKill, после убиваем и снижаем стресс(StressFactor * mod_Kill);
        if (!isGlitch)
        {
            currentClics++;
            if (currentClics == clickToKill)
            {
                Death();
                _player.StressChange(-stressFactor * mod_Kill); 
            }
        }
        //Если глич - добавляем стресс (StressFactor * mod_Glitch) и вызываем GlichEffect;
        else
        {
            _player.StressChange(stressFactor * mod_Glitch);
            GlichEffect();
            Death();
        }    
    }

    // Реакция на ПКМ.
    private void RMBReact()
    {
        //Если глич - убиваем и снижаем стресс (StressFactor * mod_Kill);
        if (isGlitch)
        {
            Death();
            _player.StressChange(-stressFactor * mod_Kill);
        }
        //Если жук - добавляем стресс(StressFactor * mod_Fail) за ошибку;
        else
            _player.StressChange(stressFactor * mod_Fail);
    }

    /*  Реакция на вход в игровую зону.
     *  Если жук  - добавляем StressFactor;
     *  Если глич - ничего;
     *  Вне зависимости жук/баг включаем insideGameZone для отслеживания позиции на сцене;
     */
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if(!insideGameZone)
        {
            insideGameZone = true;
            _gameController.Enemies_Alive.Add(this.gameObject);
            if (!isGlitch)
                _player.StressChange(stressFactor);
        }
    }

    //Метод для поиска точки пути
    private Vector3 FindNextTargetPoint()
    {
        do
        {
            targetPoint = _randomPoint.InGameZone();
        } while (Vector3.Distance(this.transform.position, targetPoint) < 2);

        direction = targetPoint - this.transform.position;
        rotationAngle = Vector3.SignedAngle(this.transform.up.normalized, direction.normalized, Vector3.forward);
        return targetPoint;
    }

    //Метод для смерти жука/глича
    private void Death()
    {
        Destroy(gameObject);
        _gameController.Enemies_Alive.Remove(this.gameObject);
    }

    //Следующие 2 метода (MovePattern и GlitchEffect) будут уникальны для каждого противника, потому вынесены в самый низ, отдельно
    //Метод для передвижения, чтоб легче было связать передвижение с анимацией

    private IEnumerator MovePattern()
    {
        switch (enemyType)
        {
            case EnemyType.Crum:
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
                _rigidbody.velocity = Vector3.zero;
                FindNextTargetPoint();
                yield return new WaitForSecondsRealtime(1.5f);
                transform.Rotate(Vector3.forward, rotationAngle);
                _rigidbody.velocity = this.transform.up * speed;
                break;

            case EnemyType.Fly:
                //Затычка
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
                _rigidbody.velocity = Vector3.zero;
                FindNextTargetPoint();
                yield return new WaitForSecondsRealtime(1.5f);
                transform.Rotate(Vector3.forward, rotationAngle);
                _rigidbody.velocity = this.transform.up * speed;
                break;

            case EnemyType.Wood_Louse:
                //Затычка
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
                _rigidbody.velocity = Vector3.zero;
                FindNextTargetPoint();
                yield return new WaitForSecondsRealtime(1.5f);
                transform.Rotate(Vector3.forward, rotationAngle);
                _rigidbody.velocity = this.transform.up * speed;
                break;

            case EnemyType.Cockroach:
                //Затычка
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
                _rigidbody.velocity = Vector3.zero;
                FindNextTargetPoint();
                yield return new WaitForSecondsRealtime(1.5f);
                transform.Rotate(Vector3.forward, rotationAngle);
                _rigidbody.velocity = this.transform.up * speed;
                break;

        }
        StartCoroutine(MovePattern());
    }

    //Эффект срабатывания глича
    public void GlichEffect()
    {
        switch (enemyType)
        {
            //Клоп - спаун 3 Клопов(жуков) из позиции глича при клике
            case EnemyType.Crum:
                for (int i = 0; i < 3; i++)
                    _gameController.Spawn(_gameController.Enemies_Prefabs[0], transform.position, Quaternion.identity);
                break;

            //Муха - ускорение (speed*glitchModify) всех живых противников на сцене которые не были ускорены ранее
            case EnemyType.Fly:
                foreach (GameObject livingEnemy in _gameController.Enemies_Alive) 
                {
                    if (livingEnemy.GetComponent<Enemy>().speed == livingEnemy.GetComponent<Enemy>().base_speed)
                        livingEnemy.GetComponent<Enemy>().speed = livingEnemy.GetComponent<Enemy>().speed * glitchModify;              
                }
                break;

            //Таракан - Область вокруг глича затемняется на (glitchDuration) секкунд
            case EnemyType.Cockroach:
                break;
                
            //Мокрица - увеличение ХП (clickToKill*glitchModify) всех живых противников на сцене которые не были усилены ранее

            case EnemyType.Wood_Louse:
                foreach (GameObject livingEnemy in _gameController.Enemies_Alive)
                {
                    if (livingEnemy.GetComponent<Enemy>().clickToKill == livingEnemy.GetComponent<Enemy>().base_clickToKill)
                        livingEnemy.GetComponent<Enemy>().clickToKill = livingEnemy.GetComponent<Enemy>().clickToKill * glitchModify;
                }
                break;
        }
    }
}

