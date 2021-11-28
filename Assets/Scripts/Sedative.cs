using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Sedative : MonoBehaviour
{
    [SerializeField] private Player _player;

    [SerializeField] private float stressAffect;

    private Button _button;
    private Text _text;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Click);

        _text = GetComponentInChildren<Text>();
    }

    void Click()
    {
        _player.StressChange(-stressAffect);
    }
}
