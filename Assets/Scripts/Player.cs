using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float baseStressLevel;

    [SerializeField] private float stress = 0f;
    [SerializeField] private Slider stressMether;
    [SerializeField] private Text stressValue;

    private void Awake()
    {
        StressChange(baseStressLevel);
    }

    //Метод изменения стресса(как вверх так и вниз) + отображение результата в StressMether
    public void StressChange(float stressFactor) 
    {
        stress += stressFactor;
        if (stress < 0)
            stress = 0;

        stressMether.value = stress;
        stressValue.text = stressMether.value.ToString();

        //Условие поражения
        if (stress >= 100)
            GameOver();
    }



    //Метод, вызываемый при успловиях поражения
    private void GameOver()
    { 
        
    }

  
}
