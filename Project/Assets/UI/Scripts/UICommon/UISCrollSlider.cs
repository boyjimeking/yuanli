using UnityEngine;
using System.Collections;

public class UISCrollSlider : MonoBehaviour
{
    //进度条
    public UISlider slider;
    //代表最大值
    public int maxValue;
    private int finalValue = 0;
    private int currentValue = 0;
    private int stepNum = 0;
    public int FinalValue
    {
        get
        {
            return finalValue;
        }
        set
        {
            finalValue = value;
            int number = Mathf.Abs(finalValue - currentValue);
            if (number <= 100)
                stepNum = number;
            else if (number <= 1000)
                stepNum = number / 100;
            else if (1000 < number)
                stepNum = number / 50 + 1;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (finalValue != currentValue)
        {
            if ((currentValue + stepNum) < finalValue)
                currentValue += stepNum;
            else
                currentValue = finalValue;
            slider.value = currentValue * 1.0f / maxValue;
        }
    }
}
