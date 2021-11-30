using AllIn1SpriteShader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerDownHandler
{
    /*Âûáîð òèïà ïðîòèâíèêà èç âûïàäàþùåãî ñïèñêà â èíñïåêòîðå
     * Crum - Êëîï
     * Fly - Ìóõà
     * Cockroach - Òàðàêàí
     * Wood_Louse - Ìîêðèöà
     */
    public enum EnemyType
    {
        Crum,
        Fly,
        Cockroach,
        Wood_Louse
    }

    [Header("Òèï âðàãà + îïðåäåëåíèå æóê/ãëè÷")]
    //Ïåðåìåíà äëÿ çàïèñè âûáîðà â Enemy Type
        public EnemyType enemyType;
    
    //Îïðåäåëåíèå æóê/ãëè÷ (false/true), Default: false;
        public bool isGlitch = false;

    /*Ïåðåìåííûå õàðàêòèðèñòèê âðàãà
     * speed - ñêîðîñòü ïåðåäâèæåíèÿ (â ÷åì?)
     * stressFactor - áàçîâîå çíà÷åíèå íà÷èñëÿåìîãî ñòðàññà îò æóêà/ãëè÷à êîòîðîå ìåíÿåòñÿ ìîäèôèêàòîðàìè
     * clickToKill -  áàçîâîå çíà÷åíèå ÕÏ (êîëè÷åñòâî êëèêîâ ïî æóêó äëÿ óáèéñòâà)
     */
    [Header("Õàð-êè ïðîòèâíèêà")]
    [Space]
        [Tooltip("Ñêîðîñòü")]
        [SerializeField] private float speed;
                         private float base_speed;

        [Tooltip("Ñòðåññ Ôàêòîð")]
        [SerializeField] private float stressFactor;

        [Tooltip("ÕÏ")]
        [SerializeField] private float clickToKill;
                         private float base_clickToKill;

    [Header("Õàð-êè ãëè÷à")]
    [Space]
        [Tooltip("Ñèëà ýôôåêòà ãëè÷à(íàïðèìåð ìíîæèòåëü ñêîðîñòès ó ìóõè)")]
        [SerializeField] private float glitchModify;

        [Tooltip("Äëèòåëüíîñòü ïîäñâåòêè ãëè÷à")]
        [SerializeField] private float glitchingDuration;

        [Tooltip("Ïàóçà ìåæäó ïîäñâåòêàìè")]
        public float glitchingCooldown;

    //Ïåðåìåííàÿ äëÿ îòñëåæèâàíèÿ óæå ñäåëàííûõ êëèêîâ ïî æóêó
    private int currentClics = 0;

    //Ïåðåìåííàÿ äëÿ îòñëåæèâàíèÿ âõîäà â èãðîâóþ çîíó
    private bool insideGameZone = false;

    //Ññûëêè íà íåîáõîäèìûå êîìïîíåíòû(Ñêðèïò èãðîêà, ãåéìêîíòðîëëåðà è ãåíåðàòîðà òî÷åê, ôèç. òåëî âðàãà)
    private Player _player;
    private GameController _gameController;
    private RandomPoint _randomPoint;
    private Rigidbody2D _rigidbody;

    /* Ìîäèôèêàòîðû ñìåíû ñòðåññà: 
     * Fail     - ïðè îøèáêå(ÏÊÌ ïî æóêó);
     * Glitch   - ïðè ñðàáàòûâàíèè ãëè÷à; 
     * Kill     - ïðè óáèéñâå æóêà;
     */

    [Header("Ìîäèôèêàòîðû ñîáûòèé")]
    [Space]

    [Tooltip("Îøèáêà ïðè âûÿâëåíèè ãëè÷à")]
    [SerializeField] private float mod_Fail;

    [Tooltip("Ñðàáàòûâàíèå ýôôåêòà ãëè÷à")]
    [SerializeField] private float mod_Glitch;

    [Tooltip("Óáèéñòâî æóêà/ãëè÷à")]
    [SerializeField] private float mod_Kill;
    //PS. ñäåëàòü êîíñòàíòíûìè ñ êîíêðåìíûìè çíà÷åíèÿìè 

    private Vector3 targetPoint;
    Vector3 direction;
    float rotationAngle;

    Animator _animator;
    private void Awake()
    {
        //Çàïîìèíàíèå áàçîâîé ñêîðîñòè è ÕÏ
        base_speed = speed;
        base_clickToKill = clickToKill;

        //Ïîëó÷åíèå íóæíûõ êîìïîíåíòîâ
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
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
        if (isGlitch)
            StartCoroutine(Glitching());
    }

    private void Update()
    {
    }
    //Ìåòîä îòñëåæèâàíèÿ íàæàòèÿ ÏÊÌ èëè ËÊÌ ïî âðàãó
    public void OnPointerDown(PointerEventData eventData)
    {
        if (insideGameZone && currentClics < clickToKill)
        {
            //Îòñëåæèâàíèå ËÊÌ
            if (eventData.button == PointerEventData.InputButton.Left)
                LMBReact();

            //Îòñëåæèâàíèå ÏÊÌ
            if (eventData.button == PointerEventData.InputButton.Right)
                RMBReact();
        }
    }

    //  Ðåàêöèÿ íà ËÊÌ.
    private void LMBReact()
    {
        // Åñëè ãëè÷ - äîáàâëÿåì ñòðåññ(StressFactor * mod_Glitch) è âûçûâàåì GlichEffect;
        if (isGlitch)
        {
            _player.StressChange(stressFactor * mod_Glitch);
            SoundManagerScript.PlaySound("GlitchInit"); 
            playDeathAnim();           
            GlichEffect();
        }

        //Åñëè æóê  -çàñ÷èòûâàåì êëèêè äî clickToKill, ïîñëå óáèâàåì è ñíèæàåì ñòðåññ(StressFactor * mod_Kill);
        else
        {
            currentClics++;
            if (currentClics != clickToKill)
            {
                SoundManagerScript.PlaySound("ClickReaction");
            }
            if (currentClics == clickToKill)
            {
                _player.StressChange(-stressFactor * mod_Kill);
                SoundManagerScript.PlaySound("BugDeath");
                playDeathAnim();              
            }
        }    
    }

    // Ðåàêöèÿ íà ÏÊÌ.
    private void RMBReact()
    {
        //Åñëè ãëè÷ - óáèâàåì è ñíèæàåì ñòðåññ (StressFactor * mod_Kill);
        if (isGlitch)
        {
            SoundManagerScript.PlaySound("GlitchDeath");
            playDeathAnim();          
            _player.StressChange(-stressFactor * mod_Kill);
        }
        //Åñëè æóê - äîáàâëÿåì ñòðåññ(StressFactor * mod_Fail) çà îøèáêó;
        else
            SoundManagerScript.PlaySound("BugRmb");
            _player.StressChange(stressFactor * mod_Fail);
    }

    /*  Ðåàêöèÿ íà âõîä â èãðîâóþ çîíó.
     *  Åñëè æóê  - äîáàâëÿåì StressFactor;
     *  Åñëè ãëè÷ - íè÷åãî;
     *  Âíå çàâèñèìîñòè æóê/áàã âêëþ÷àåì insideGameZone äëÿ îòñëåæèâàíèÿ ïîçèöèè íà ñöåíå;
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

    //Ìåòîä äëÿ ïîèñêà òî÷êè ïóòè
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

    private IEnumerator Glitching()
    {
        yield return new WaitForSeconds(glitchingCooldown);
        _animator.SetFloat("Glitching", 0.7f);
        yield return new WaitForSeconds(glitchingDuration);
        _animator.SetFloat("Glitching", 0);
        StartCoroutine(Glitching());
    }    
    private void playDeathAnim()
    {
        _rigidbody.velocity = Vector2.zero;
        _animator.SetBool("isDeath", true);
        _animator.SetBool("isGlitch", isGlitch);
    }
    //Ìåòîä äëÿ ñìåðòè æóêà/ãëè÷à
    private void Death()
    {
        Destroy(this.gameObject);
        _gameController.Enemies_Alive.Remove(this.gameObject);
    }

    //Ñëåäóþùèå 2 ìåòîäà (MovePattern è GlitchEffect) áóäóò óíèêàëüíû äëÿ êàæäîãî ïðîòèâíèêà, ïîòîìó âûíåñåíû â ñàìûé íèç, îòäåëüíî
    //Ìåòîä äëÿ ïåðåäâèæåíèÿ, ÷òîá ëåã÷å áûëî ñâÿçàòü ïåðåäâèæåíèå ñ àíèìàöèåé

    private IEnumerator MovePattern()
    {
        switch (enemyType)
        {
            case EnemyType.Crum:
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
                _rigidbody.velocity = Vector3.zero;
                FindNextTargetPoint();
                yield return new WaitForSeconds(1.5f);
                transform.Rotate(Vector3.forward, rotationAngle);
                _rigidbody.velocity = this.transform.up * speed;
                break;

            case EnemyType.Fly:
                //Çàòû÷êà
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
                _rigidbody.velocity = Vector3.zero;
                FindNextTargetPoint();
                yield return new WaitForSeconds(1.5f);
                transform.Rotate(Vector3.forward, rotationAngle);
                _rigidbody.velocity = this.transform.up * speed;
                break;

            case EnemyType.Wood_Louse:
                //Çàòû÷êà
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
                _rigidbody.velocity = Vector3.zero;
                FindNextTargetPoint();
                yield return new WaitForSeconds(1.5f);
                transform.Rotate(Vector3.forward, rotationAngle);
                _rigidbody.velocity = this.transform.up * speed;
                break;

            case EnemyType.Cockroach:
                //Çàòû÷êà
                yield return new WaitUntil(() => Vector3.Distance(this.transform.position, targetPoint) <= 0.5f);
                _rigidbody.velocity = Vector3.zero;
                FindNextTargetPoint();
                yield return new WaitForSeconds(1.5f);
                transform.Rotate(Vector3.forward, rotationAngle);
                _rigidbody.velocity = this.transform.up * speed;
                break;

        }
        StartCoroutine(MovePattern());
    }

    //Ýôôåêò ñðàáàòûâàíèÿ ãëè÷à
    public void GlichEffect()
    {
        switch (enemyType)
        {
            //Êëîï - ñïàóí 3 Êëîïîâ(æóêîâ) èç ïîçèöèè ãëè÷à ïðè êëèêå
            case EnemyType.Crum:
                for (int i = 0; i < glitchModify; i++)
                    _gameController.Spawn(_gameController.Enemies_Prefabs[0], transform.position, Quaternion.identity);
                break;

            //Ìóõà - óñêîðåíèå (speed*glitchModify) âñåõ æèâûõ ïðîòèâíèêîâ íà ñöåíå êîòîðûå íå áûëè óñêîðåíû ðàíåå
            case EnemyType.Fly:
                foreach (GameObject livingEnemy in _gameController.Enemies_Alive) 
                {
                    if (livingEnemy.GetComponent<Enemy>().speed == livingEnemy.GetComponent<Enemy>().base_speed) 
                    { 
                        livingEnemy.GetComponent<Enemy>().speed = livingEnemy.GetComponent<Enemy>().speed * glitchModify;
                    }
                }
                break;

            //Òàðàêàí - Îáëàñòü âîêðóã ãëè÷à çàòåìíÿåòñÿ íà (glitchDuration) ñåêêóíä
            case EnemyType.Cockroach:
                break;
                
            //Ìîêðèöà - óâåëè÷åíèå ÕÏ (clickToKill*glitchModify) âñåõ æèâûõ ïðîòèâíèêîâ íà ñöåíå êîòîðûå íå áûëè óñèëåíû ðàíåå

            case EnemyType.Wood_Louse:
                foreach (GameObject livingEnemy in _gameController.Enemies_Alive)
                {
                    if (livingEnemy.GetComponent<Enemy>().clickToKill == livingEnemy.GetComponent<Enemy>().base_clickToKill)
                    {
                        livingEnemy.GetComponent<Enemy>().clickToKill = livingEnemy.GetComponent<Enemy>().clickToKill * glitchModify;
                        livingEnemy.GetComponent<Animator>().SetFloat("Buffed", 0.5f);
                    }
                }
                break;
        }
    }
}

