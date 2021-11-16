using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerDownHandler
{
     public bool isGlitch;

    [SerializeField] private float speed;
    [SerializeField] private float stressFactor;
    [SerializeField] private int clickToKill;
    [SerializeField] private const int glitchID = 0;

    private int currentClics = 0;
    private bool insideGameZone = false;

    private Player player;
    private GameController gameController;

    /* Модификаторы смены стресса: 
     * Fail     - при ошибке(ПКМ по жуку);
     * Glitch   - при срабатывании глича; 
     * Kill     - при убийсве жука;
     * Tick     - Увелечение стресса со временем; ------------ (нужен???)
     */
    [SerializeField]  private float
        mod_Fail    = 1.5f, 
        mod_Glitch  = 2.2f, 
        mod_Kill    = 1.1f, 
        mod_Tick    = 0.2f;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        gameController = FindObjectOfType<GameController>();
            
    }

    private void FixedUpdate()
    {
        Move();
    }

    //Метод для передвижения, чтоб легче было связать передвижение с анимацией
    private void Move()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    /*  Реакция на ЛКМ.
     *  Если жук  - засчитываем клики до clickToKill, после убиваем и снижаем стресс (StressFactor * mod_Kill);
     *  Если глич - добавляем стресс (StressFactor * mod_Glitch) и вызываем GlichEffect;
     */
    private void LMBReact()
    {
        if (!isGlitch)
        {
            currentClics++;
            if (currentClics == clickToKill)
            {
                Death();
                player.StressChange(-stressFactor * mod_Kill);
            }
        }
        else
        {
            player.StressChange(stressFactor * mod_Glitch);
            GlichEffect();
            Death();
        }    
    }

    /*  Реакция на ПКМ.
     *  Если жук  - добавляем стресс (StressFactor * mod_Fail) за ошибку;
     *  Если глич - убиваем и снижаем стресс (StressFactor * mod_Kill);
     */
    private void RMBReact()
    {
        if (isGlitch)
        {
            Death();
            player.StressChange(-stressFactor * mod_Kill);
        }
        else
            player.StressChange(stressFactor * mod_Fail);
    }

    //Эффект срабатывания глича
    

    //Метод для смерти жука/глича
    private void Death()
    {
        Destroy(gameObject);
        gameController.enemyCount--;
    }

    /*  Реакция на вход в игровую зону.
     *  Если жук  - добавляем StressFactor;
     *  Если глич - ничего;
     *  Вне зависимости жук/баг включаем insideGameZone для отслеживания позиции на сцене;
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isGlitch) 
            player.StressChange(stressFactor);
        insideGameZone = true;
    }

    //Event отслеживания нажатия ПКМ или ЛКМ по врагу
    public void OnPointerDown(PointerEventData eventData)
    {
        //Отслеживание ЛКМ
        if (eventData.button == PointerEventData.InputButton.Left)
            LMBReact();
        //Отслеживание ПКМ
        if (eventData.button == PointerEventData.InputButton.Right)
            RMBReact();
    }

    /*
     * 
     * 
     * 
     * 
     */
    private void GlichEffect()
    {
        switch (glitchID)
        {
            case 0:
                for (int i = 0; i < 3; i++)
                {
                    gameController.Spawn(gameObject, transform.position, Quaternion.Euler(0, 0, i * 120));
                }
                break;
        }
    }
}
