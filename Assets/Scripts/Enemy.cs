using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerDownHandler
{
     public bool isGlitch;

    [SerializeField] private float speed;
    [SerializeField] private float stressFactor;
    [SerializeField] private int clickToKill;

    private int currentClics = 0;
    [SerializeField] private bool insideGameZone = false;

    private Player player;
    private GameController gameController;

    /* ������������ ����� �������: 
     * Fail     - ��� ������(��� �� ����);
     * Glitch   - ��� ������������ �����; 
     * Kill     - ��� ������� ����;
     * Tick     - ���������� ������� �� ��������; ------------ (�����???)
     */
    [SerializeField]
    private float
        mod_Fail    = 1.5f,
        mod_Glitch  = 2.2f,
        mod_Kill    = 1.1f;
     // mod_Tick    = 0.2f;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        gameController = FindObjectOfType<GameController>();
            
    }

    private void FixedUpdate()
    {
        Move();
    }

    //����� ������������ ������� ��� ��� ��� �� �����
    public void OnPointerDown(PointerEventData eventData)
    {
        if (insideGameZone)
        {
            //������������ ���
            if (eventData.button == PointerEventData.InputButton.Left)
                LMBReact();

            //������������ ���
            if (eventData.button == PointerEventData.InputButton.Right)
                RMBReact();
        }
    }

    /*  ������� �� ���.
     *  ���� ���  - ����������� ����� �� clickToKill, ����� ������� � ������� ������ (StressFactor * mod_Kill);
     *  ���� ���� - ��������� ������ (StressFactor * mod_Glitch) � �������� GlichEffect;
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

    /*  ������� �� ���.
     *  ���� ���  - ��������� ������ (StressFactor * mod_Fail) �� ������;
     *  ���� ���� - ������� � ������� ������ (StressFactor * mod_Kill);
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

    //����� ��� ������ ����/�����
    private void Death()
    {
        Destroy(gameObject);
        gameController.enemyCount--;
    }

    /*  ������� �� ���� � ������� ����.
     *  ���� ���  - ��������� StressFactor;
     *  ���� ���� - ������;
     *  ��� ����������� ���/��� �������� insideGameZone ��� ������������ ������� �� �����;
     */
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if(!insideGameZone)
        {
            insideGameZone = true;
            gameController.enemyCount++;
            if (!isGlitch)
                player.StressChange(stressFactor);
        }
    }
    private void OnTriggerExit2D(Collider2D trigger)
    {
        transform.Rotate(new Vector3(0,0,Random.Range(120,240)));
    }

    //��������� 2 ������ (Move � GlitchEffect) ����� ��������� ��� ������� ����������, ������ �������� � ����� ���, ��������
    //����� ��� ������������, ���� ����� ���� ������� ������������ � ���������
    private void Move()
    {
        if (!insideGameZone)
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, 0), speed * Time.deltaTime);
        else
            transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private IEnumerator Movement()
    {

        yield return new WaitForSeconds(5);

    }
    //������ ������������ �����
    private void GlichEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            gameController.Spawn(gameObject, transform.position, Quaternion.Euler(0, 0, i * 120));
        }   
    }
}
