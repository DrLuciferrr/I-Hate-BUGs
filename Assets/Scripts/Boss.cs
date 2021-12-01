using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject spawnedEnemy;
    [Space]
    [SerializeField] private float speed;
    [SerializeField] private float stressFactor;
    [SerializeField] private float clickToKill;
    [SerializeField] private float mod_Kill;
    [Space]
    [SerializeField] private float bossEffectModificator;
    [SerializeField] private float bossEffectInterwal;
    [SerializeField] private float bossEffectCooldown;

    private Player _player;
    private GameController _gameController;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private RandomPoint _randomPoint;

    private bool insideGameZone = false;
    private int currentClics = 0;

    private Vector3 targetPoint;
    Vector3 direction;
    float rotationAngle;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _gameController = FindObjectOfType<GameController>();

        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _randomPoint = new RandomPoint();
    }
    private void Start()
    {
        FindNextTargetPoint();
        transform.Rotate(Vector3.forward, rotationAngle);
        _rigidbody.velocity = this.transform.up * speed;

        StartCoroutine(Move());
        StartCoroutine(BossEffect());
        StartCoroutine(StressTick());
    }
    private void OnMouseDown()
    {
        currentClics++;
        if (currentClics == clickToKill)
        {
            _player.StressChange(-stressFactor * mod_Kill);
            SoundManagerScript.PlaySound("BugDeath");
            playDeathAnim();
        }
        else            
            SoundManagerScript.PlaySound("ClickReaction");
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
    private IEnumerator Move()
    {
        yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
        _rigidbody.velocity = Vector3.zero;
        FindNextTargetPoint();
        yield return new WaitForSeconds(1.5f);
        transform.Rotate(Vector3.forward, rotationAngle);
        _rigidbody.velocity = this.transform.up * speed;
        

        StartCoroutine(Move());
    }
    private IEnumerator BossEffect()
    {
        yield return new WaitUntil(() => insideGameZone);
        yield return new WaitForSeconds(bossEffectCooldown);
        for (int i = 0; i < bossEffectModificator; i++)
        { 
            Instantiate(spawnedEnemy, this.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(bossEffectInterwal);
        }
        StartCoroutine(BossEffect());
    }
    private IEnumerator StressTick()
    {
        _player.StressChange(stressFactor);
        yield return new WaitForSeconds(2);
        StartCoroutine(StressTick());
    }
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (!insideGameZone)
        {
            insideGameZone = true;
            _gameController.Enemies_Alive.Add(this.gameObject);
        }
    }
    private void playDeathAnim()
    {
        _rigidbody.velocity = Vector2.zero;
        _animator.SetBool("isDeath", true);

    }
    private void Death()
    {
        Destroy(this.gameObject);
        _gameController.Enemies_Alive.Remove(this.gameObject);
    }
}
