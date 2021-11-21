using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerDownHandler
{
    //����������� ���/���� (false/true), Default: false;
    public bool isGlitch = false;

    /*���������� ������������� �����
     * ID - ��� �������� ������������ � ������� �����
     * speed - �������� ������������ (� ���?)
     * stressFactor - ������� �������� ������������ ������� �� ����/����� ������� �������� ��������������
     * clickToKill -  ������� �������� �� (���������� ������ �� ���� ��� ��������)
     */
    [SerializeField] private int ID;
    [SerializeField] private float speed;
    [SerializeField] private float stressFactor;
    [SerializeField] private int clickToKill;

    //���������� ��� ������������ ��� ��������� ������ �� ����
    private int currentClics = 0;

    //���������� ��� ������������ ����� � ������� ����
    private bool insideGameZone = false;

    //������ �� ����������� ����������(������ ������, ��������������� � ���������� �����, ���. ���� �����)
    private Player player;
    private GameController gameController;
    private RandomPoint randomPoint = new RandomPoint();
    private Rigidbody2D rb;

    /* ������������ ����� �������: 
     * Fail     - ��� ������(��� �� ����);
     * Glitch   - ��� ������������ �����; 
     * Kill     - ��� ������� ����;
     */
    [SerializeField]
    private float
        mod_Fail,
        mod_Glitch,
        mod_Kill;
    //PS. ������� ������������ � ����������� ���������� 

    private void Awake()
    {
        //��������� ������ �����������
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        gameController = FindObjectOfType<GameController>();

        Move(randomPoint.InGameZone());
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

    //  ������� �� ���.
    private void LMBReact()
    {
        //���� ���  -����������� ����� �� clickToKill, ����� ������� � ������� ������(StressFactor * mod_Kill);
        if (!isGlitch)
        {
            currentClics++;
            if (currentClics == clickToKill)
            {
                Death();
                player.StressChange(-stressFactor * mod_Kill); 
            }
        }
        //���� ���� - ��������� ������ (StressFactor * mod_Glitch) � �������� GlichEffect;
        else
        {
            player.StressChange(stressFactor * mod_Glitch);
            GlichEffect();
            Death();
        }
           
    }

    // ������� �� ���.
    private void RMBReact()
    {
        //���� ���� - ������� � ������� ������ (StressFactor * mod_Kill);
        if (isGlitch)
        {
            Death();
            player.StressChange(-stressFactor * mod_Kill);
        }
        //���� ��� - ��������� ������(StressFactor * mod_Fail) �� ������;
        else
              player.StressChange(stressFactor * mod_Fail);
    }

    //����� ��� ������ ����/�����
    private void Death()
    {
        Destroy(gameObject);
        gameController.Enemys_Alive.Remove(this.gameObject);
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
            gameController.Enemys_Alive.Add(this.gameObject);
            if (!isGlitch)
                player.StressChange(stressFactor);
        }
    }
    private void OnTriggerExit2D(Collider2D trigger)
    {
        Move(randomPoint.InGameZone());
    }

    //��������� 2 ������ (Move � GlitchEffect) ����� ��������� ��� ������� ����������, ������ �������� � ����� ���, ��������
    //����� ��� ������������, ���� ����� ���� ������� ������������ � ���������
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

    //������ ������������ �����
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

