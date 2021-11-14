using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool isGlitch;
    [SerializeField] private float speed;
    [SerializeField] private float stressFactor;
    [SerializeField] private int clickToKill;

    
    private Player player;
    private int currentClics = 0;
    private Rigidbody2D rigidBody;

    //Различные модификаторы стресса выраженные константами
    [SerializeField] private float mod_Fail = 2f, mod_Tick = 0.5f, mod_Glitch = 3f, mod_Kill = 1.5f;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        Move();
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {

        }
    }

    //Метод для передвижения, чтоб легче было связать передвижение с анимацией
    private void Move()
    {
        rigidBody.AddRelativeForce(Vector2.up * speed, ForceMode2D.Force);
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
                Destroy(gameObject);
                player.StressChange(-stressFactor * mod_Kill);
            }
        }
        else
            GlichEffect();
    }

    /*  Реакция на ПКМ.
     *  Если жук  - добавляем стресс (StressFactor * mod_Fail) за ошибку;
     *  Если глич - убиваем и снижаем стресс (StressFactor * mod_Kill);
     */
    private void RMBReact()
    {
        if (isGlitch)
        {
            Destroy(gameObject);
            player.StressChange(-stressFactor);
        }
        else
            player.StressChange(stressFactor * mod_Fail);
    }

    //Эффект срабатывания глича
    private void GlichEffect()
    {
        player.StressChange(stressFactor*2);
    }

    /*  Реакция на появление в игровой зоне.
     *  Если жук  - добавляем StressFactor;
     *  Если глич - ничего;
     */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        player.StressChange(stressFactor);
    }
    private void OnMouseDown()
    {
        LMBReact();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Отслеживание ЛКМ
        if (eventData.button == PointerEventData.InputButton.Left)
            //LMBReact();
            print("LMB");
        //Отслеживание ПКМ
        if (eventData.button == PointerEventData.InputButton.Right)
            //RMBReact();
            print("RMB");
    }
}
