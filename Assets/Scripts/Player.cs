using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float stress = 0f;
    [SerializeField] private Slider stressMether;

    //����� ��������� �������(��� ����� ��� � ����) + ����������� ���������� � StressMether
    public void StressChange(float stressFactor) 
    {
        stress += stressFactor;
        if (stress < 0)
            stress = 0;
        stressMether.value = stress;
    }

}
