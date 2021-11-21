using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerDownHandler
{
    //Определение жук/глич (false/true), Default: false;
    public bool isGlitch = false;

    /*Переменные характиристик врага
     * ID - для паттерна передвижения и еффекта глича
     * speed - скорость передвижения (в чем?)
     * stressFactor - базовое значение начисляемого страсса от жука/глича которое меняется модификаторами
     * clickToKill -  базовое значение ХП (количество кликов по жуку для убийства)
     */
    [SerializeField] private int ID;
    [SerializeField] private float speed;
    [SerializeField] private float stressFactor;
    [SerializeField] private int clickToKill;

    //Переменная для отслеживания уже сделанных кликов по жуку
    private int currentClics = 0;

    //Переменная для отслеживания входа в игровую зону
    private bool insideGameZone = false;

    //Ссылки на необходимые компоненты(Скрипт игрока, геймконтроллера и генератора точек, физ. тело врага)
    private Player player;
    private GameController gameController;
    private RandomPoint randomPoint = new RandomPoint();
    private Rigidbody2D rb;

    /* Модификаторы смены стресса: 
     * Fail     - при ошибке(ПКМ по жуку);
     * Glitch   - при срабатывании глича; 
     * Kill     - при убийсве жука;
     */
    [SerializeField]
    private float
        mod_Fail,
        mod_Glitch,
        mod_Kill;
    //PS. сделать константными с конкремными значениями 

    private void Awake()
    {
        //Получение нужных компонентов
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        gameController = FindObjectOfType<GameController>();

        Move(randomPoint.InGameZone());
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
                player.StressChange(-stressFactor * mod_Kill); 
            }
        }
        //Если глич - добавляем стресс (StressFactor * mod_Glitch) и вызываем GlichEffect;
        else
        {
            player.StressChange(stressFactor * mod_Glitch);
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
            player.StressChange(-stressFactor * mod_Kill);
        }
        //Если жук - добавляем стресс(StressFactor * mod_Fail) за ошибку;
        else
              player.StressChange(stressFactor * mod_Fail);
    }

    //Метод для смерти жука/глича
    private void Death()
    {
        Destroy(gameObject);
        gameController.Enemys_Alive.Remove(this.gameObject);
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
            gameController.Enemys_Alive.Add(this.gameObject);
            if (!isGlitch)
                player.StressChange(stressFactor);
        }
    }
    private void OnTriggerExit2D(Collider2D trigger)
    {
        Move(randomPoint.InGameZone());
    }

    //Следующие 2 метода (Move и GlitchEffect) будут уникальны для каждого противника, потому вынесены в самый низ, отдельно
    //Метод для передвижения, чтоб легче было связать передвижение с анимацией
    public void Move(Vector3 targetPoint)
    {
        switch (ID)
        {
            case 0:
                Vector3 direction = targetPoint - this.transform.position;
                rb.velocity = direction.normalized * speed;
                transform.Rotate(Vector3.forward, Vector3.SignedAngle(this.transform.up.normalized, direction, Vector3.forward));
                break;
        }
    }

    //Эффект срабатывания глича
    private void GlichEffect()
    {
        switch (ID)
        {
            case 0:
                for (int i = 0; i < 3; i++)
                {
                    gameController.Spawn(gameObject, transform.position, Quaternion.Euler(0, 0, i * 120 + Random.Range(0, 20)));
                }
                break;
        }
    }
}

