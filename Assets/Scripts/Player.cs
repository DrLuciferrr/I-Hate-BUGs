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

    //����� ��������� �������(��� ����� ��� � ����) + ����������� ���������� � StressMether
    public void StressChange(float stressFactor) 
    {
        stress += stressFactor;
        if (stress < 0)
            stress = 0;

        stressMether.value = stress;
        stressValue.text = stressMether.value.ToString();

        //������� ���������
        if (stress >= 100)
            GameOver();
    }



    //�����, ���������� ��� ��������� ���������
    private void GameOver()
    { 
        
    }

  
}
