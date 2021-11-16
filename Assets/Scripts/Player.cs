using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private float stress = 0f;
    [SerializeField] private Slider stressMether;
    [SerializeField] private Text stressValue;

    //Метод изменения стресса(как вверх так и вниз) + отображение результата в StressMether
    public void StressChange(float stressFactor) 
    {
        stress += stressFactor;
        if (stress < 0)
            stress = 0;
        stressValue.text = stressMether.value.ToString();
        stressMether.value = stress;
    }

    private void GameOver()
    { 
        
    }
}
