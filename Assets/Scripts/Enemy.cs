using AllIn1SpriteShader;
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

    [Header("��� ����� + ����������� ���/����")]
    //�������� ��� ������ ������ � Enemy Type
        public EnemyType enemyType;
    
    //����������� ���/���� (false/true), Default: false;
        public bool isGlitch = false;

    /*���������� ������������� �����
     * speed - �������� ������������ (� ���?)
     * stressFactor - ������� �������� ������������ ������� �� ����/����� ������� �������� ��������������
     * clickToKill -  ������� �������� �� (���������� ������ �� ���� ��� ��������)
     */
    [Header("���-�� ����������")]
    [Space]
        [Tooltip("��������")]
        [SerializeField] private float speed;
                         private float base_speed;

        [Tooltip("������ ������")]
        [SerializeField] private float stressFactor;

        [Tooltip("��")]
        [SerializeField] private float clickToKill;
                         private float base_clickToKill;

    [Header("���-�� �����")]
    [Space]
    //
    //    [Tooltip("������������ ������� �����")]
    //    [SerializeField] private float glitchDuration;

        [Tooltip("���� ������� �����(�������� ��������� ��������s � ����)")]
        [SerializeField] private float glitchModify;
    

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

    [Header("������������ �������")]
    [Space]

    [Tooltip("������ ��� ��������� �����")]
    [SerializeField] private float mod_Fail;

    [Tooltip("������������ ������� �����")]
    [SerializeField] private float mod_Glitch;

    [Tooltip("�������� ����/�����")]
    [SerializeField] private float mod_Kill;
    //PS. ������� ������������ � ����������� ���������� 

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
        //����������� ������� �������� � ��
        base_speed = speed;
        base_clickToKill = clickToKill;

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
            _player.StressChange(-stressFactor * mod_Kill);
        }
        //���� ��� - ��������� ������(StressFactor * mod_Fail) �� ������;
        else
            _player.StressChange(stressFactor * mod_Fail);
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

    //����� ��� ������ ����� ����
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

    //����� ��� ������ ����/�����
    private void Death()
    {
        Destroy(gameObject);
        _gameController.Enemies_Alive.Remove(this.gameObject);
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
                //�������
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
                _rigidbody.velocity = Vector3.zero;
                FindNextTargetPoint();
                yield return new WaitForSecondsRealtime(1.5f);
                transform.Rotate(Vector3.forward, rotationAngle);
                _rigidbody.velocity = this.transform.up * speed;
                break;

            case EnemyType.Cockroach:
                //�������
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

    //������ ������������ �����
    public void GlichEffect()
    {
        switch (enemyType)
        {
            //���� - ����� 3 ������(�����) �� ������� ����� ��� �����
            case EnemyType.Crum:
                for (int i = 0; i < 3; i++)
                    _gameController.Spawn(_gameController.Enemies_Prefabs[0], transform.position, Quaternion.identity);
                break;

            //���� - ��������� (speed*glitchModify) ���� ����� ����������� �� ����� ������� �� ���� �������� �����
            case EnemyType.Fly:
                foreach (GameObject livingEnemy in _gameController.Enemies_Alive) 
                {
                    if (livingEnemy.GetComponent<Enemy>().speed == livingEnemy.GetComponent<Enemy>().base_speed)
                        livingEnemy.GetComponent<Enemy>().speed = livingEnemy.GetComponent<Enemy>().speed * glitchModify;              
                }
                break;

            //������� - ������� ������ ����� ����������� �� (glitchDuration) �������
            case EnemyType.Cockroach:
                break;
                
            //������� - ���������� �� (clickToKill*glitchModify) ���� ����� ����������� �� ����� ������� �� ���� ������� �����

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

