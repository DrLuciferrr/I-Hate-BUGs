using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject stressMether;
    [Space]
    public float baseStress;
    private float stress = 0f;


    private float stressToScale = 2.1f;
    private void Awake()
    {
        StressChange(baseStress);
    }

    //Метод изменения стресса(как вверх так и вниз) + отображение результата в StressMether
    public void StressChange(float stressFactor) 
    {
        stress += stressFactor;

        if (stress < 0)
            stress = 0;

        stressMether.transform.localScale = new Vector3 (4, stress * stressToScale, 1);

        //Условие поражения
        if (stress >= 100)
            GameOver();
    }


    //Метод, вызываемый при успловиях поражения
    private void GameOver()
    { 
        
    }

  
}
