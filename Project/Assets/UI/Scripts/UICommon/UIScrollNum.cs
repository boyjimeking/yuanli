using UnityEngine;
using System.Collections;

public class UIScrollNum : MonoBehaviour
{
    public UILabel txtNum;
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
            int number = finalValue - currentValue;
            if (Mathf.Abs(number) <= 100)
                stepNum = number;
            else if (Mathf.Abs(number) <= 1000)
                stepNum = number / 100;
            else if (1000 < Mathf.Abs(number))
                stepNum = number / 50;
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
            txtNum.text = currentValue.ToString();
        }
    }
}
