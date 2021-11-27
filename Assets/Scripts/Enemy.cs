using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerDownHandler
{
    /*����� ���� ���������� �� ����������� ������ � ����������
     * Crum - ����
     * Fly - ����
     * Cockroach - �������
     * Wood_Louse - �������
     */
    public enum EnemyType
    {
        Crum,
        Fly,
        Cockroach,
        Wood_Louse
    }

    //�������� ��� ������ ������ � Enemy Type
    public EnemyType enemyType;
    
    //����������� ���/���� (false/true), Default: false;
    public bool isGlitch = false;

    /*���������� ������������� �����
     * speed - �������� ������������ (� ���?)
     * stressFactor - ������� �������� ������������ ������� �� ����/����� ������� �������� ��������������
     * clickToKill -  ������� �������� �� (���������� ������ �� ���� ��� ��������)
     */
    
    [SerializeField] private float speed;
    [SerializeField] private float stressFactor;
    [SerializeField] private int clickToKill;

    //���������� ��� ������������ ��� ��������� ������ �� ����
    private int currentClics = 0;

    //���������� ��� ������������ ����� � ������� ����
    private bool insideGameZone = false;

    //������ �� ����������� ����������(������ ������, ��������������� � ���������� �����, ���. ���� �����)
    private Player _player;
    private GameController _gameController;
    private RandomPoint _randomPoint;
    private Rigidbody2D _rigidbody;

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

    private Vector3 targetPoint;
    Vector3 direction;
    float rotationAngle;
    private void Awake()
    {
        //��������� ������ �����������
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

    private void FixedUpdate()
    {

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
                _player.StressChange(-stressFactor * mod_Kill); 
            }
        }
        //���� ���� - ��������� ������ (StressFactor * mod_Glitch) � �������� GlichEffect;
        else
        {
            _player.StressChange(stressFactor * mod_Glitch);
            StartCoroutine(GlichEffect());
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
            _player.StressChange(-stressFactor * mod_Kill);
        }
        //���� ��� - ��������� ������(StressFactor * mod_Fail) �� ������;
        else
            _player.StressChange(stressFactor * mod_Fail);
    }

    //����� ��� ������ ����/�����
    private void Death()
    {
        Destroy(gameObject);
        _gameController.Enemies_Alive.Remove(this.gameObject);
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
            _gameController.Enemies_Alive.Add(this.gameObject);
            if (!isGlitch)
                _player.StressChange(stressFactor);
        }
    }
    private void OnTriggerExit2D(Collider2D trigger)
    {
        //StopCoroutine("Move");
        //StartCoroutine(Move(_randomPoint.InGameZone()));
    }

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

    //��������� 2 ������ (MovePattern � GlitchEffect) ����� ��������� ��� ������� ����������, ������ �������� � ����� ���, ��������
    //����� ��� ������������, ���� ����� ���� ������� ������������ � ���������

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
                //�������
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
                _rigidbody.velocity = Vector3.zero;
                FindNextTargetPoint();
                yield return new WaitForSecondsRealtime(1.5f);
                transform.Rotate(Vector3.forward, rotationAngle);
                _rigidbody.velocity = this.transform.up * speed;
                break;

            case EnemyType.Wood_Louse:
                break;

            case EnemyType.Cockroach:
                break;

        }
        StartCoroutine(MovePattern());
    }

    //������ ������������ �����
    private IEnumerator GlichEffect()
    {
        switch (enemyType)
        {
            //���� - ����� 3 ������(�����) �� ������� ����� ��� �����
            case EnemyType.Crum:
                for (int i = 0; i < 3; i++)
                    _gameController.Spawn(_gameController.Enemies_Prefabs[0], transform.position, Quaternion.identity);
                yield break;

            //���� - ��������� � 1.5 ���� (speed*1.5) ���� ����� ����������� �� ����� �� 5 �������.
            //� ������ ���� ���� ������������ �������� � ������� �������� ������� - ������������ �����������
            case EnemyType.Fly:


                List<GameObject> Affected_Enemies = new List<GameObject>();
                foreach (GameObject livingEnemy in _gameController.Enemies_Alive)
                    Affected_Enemies.Add(livingEnemy);

                foreach (GameObject affectedEnemy in Affected_Enemies)
                    affectedEnemy.GetComponent<Enemy>().speed = affectedEnemy.GetComponent<Enemy>().speed * 1.5f;
                Debug.Log("SPEED UP");
                yield return new WaitForSecondsRealtime(5);

                /*
                foreach (GameObject affectedEnemy in Affected_Enemies)
                {
                    if (affectedEnemy != null)
                        affectedEnemy.GetComponent<Enemy>().speed = affectedEnemy.GetComponent<Enemy>().speed / 1.5f;
                        
                    else
                        continue;
                }
                */
                Debug.Log("SPEED DOWN");
                yield break;                

            case EnemyType.Wood_Louse:
                break;

            case EnemyType.Cockroach:
                break;
        }
    }
}

