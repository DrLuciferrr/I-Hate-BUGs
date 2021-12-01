using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Sedative : MonoBehaviour
{
    public SedativeType type;

    [SerializeField] private Player _player;
    [SerializeField] private GameController _gameController;
    [SerializeField] private Text _text;

    public int count;
    [SerializeField] private float stressAffect;
    [SerializeField] private int duration;

    

    public enum SedativeType
    {
        Chocolate_Bar,
        Tea,
        Auto_Fix
    }

    private void Awake()
    {
        _text.text = count.ToString();
    }
    private IEnumerator SedativeEffect()
    {

        switch (type)
        {
            case SedativeType.Chocolate_Bar:
                SoundManagerScript.PlaySound("UseChocolate");
                _player.StressChange(-stressAffect);
                yield break;

            case SedativeType.Tea:
                SoundManagerScript.PlaySound("UseTea");
                for (int i = 0; i <= duration; i++)
                {
                    _player.StressChange(-stressAffect);
                    yield return new WaitForSeconds(1);
                }
                yield break;

            case SedativeType.Auto_Fix:
                SoundManagerScript.PlaySound("UseAutoTest");
                yield break;
        }
    }

    private void OnMouseDown()
    {
        if (count > 0) 
        {
            StartCoroutine(SedativeEffect());
            count--;
            _text.text = count.ToString();
        }
        if (count == 0)
            Destroy(this.gameObject);
    }
}
