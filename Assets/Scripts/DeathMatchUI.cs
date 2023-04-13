using System;
using UnityEngine;
using UnityEngine.UI;

public class DeathMatchUI : MonoBehaviour
{
    [SerializeField] private RectTransform imagePoint;
    [SerializeField] private GameObject textAmount;

    [SerializeField] private GameObject textAmountLeft, textAmountRight;

    [SerializeField] private Sprite arrowUp, arrowDown;

    public void ShowTextAmountLeft(double amount)
    {
        textAmountLeft.GetComponent<Text>().text = $"{amount}b";
        textAmountLeft.SetActive(true);
    }

    public void ShowTextAmountRight(double amount)
    {
        textAmountRight.GetComponent<Text>().text = $"{amount}b";
        textAmountRight.SetActive(true);
    }

    public void SetPoint(double amount, double startAmount, float minY, float maxY, bool win, bool start)
    {
        string calculatedAmountText = $"{amount}b\n";
        double percent = GetPercentageDifference(startAmount, amount);
        percent = Math.Round(percent, 1);
        calculatedAmountText += "<color=" + percentColor(amount, startAmount) + ">(" + percent + "%)</color>";

        textAmount.GetComponent<Text>().text = calculatedAmountText;

        float y = Mathf.Lerp(minY, maxY, (float)amount / 100);
        imagePoint.anchoredPosition = new Vector2(imagePoint.anchoredPosition.x, y);

        Image image = imagePoint.GetComponent<Image>();

        if (start)
        {
            image.color = Color.yellow;
        }
        else if (win)
        {
            image.color = Color.green;
            image.sprite = arrowUp;
        }
        else
        {
            image.color = Color.red;
            image.sprite = arrowDown;
        }
    }

    private string percentColor(double amount, double startAmount)
    {
        if (amount > startAmount)
            return "lime";
        else if (amount == startAmount)
            return "yellow";
        else
            return "red";
    }

    private double CalculatePercentageIncrease(double originalValue, double newValue)
    {
        double difference = newValue - originalValue;
        double percentageIncrease = (difference / originalValue) * 100f;
        return Mathf.Abs((float)percentageIncrease);
    }

    private double GetPercentageDifference(double a, double b)
    {
        if (a == b)
        {
            return 0.0;
        }
        else
        {
            double diff = Math.Abs(a - b);
            double avg = (a + b) / 2.0;
            return Mathf.Abs((float)(diff / avg) * 100);
        }
    }

    public void OnMouseOver()
    {
        textAmount.SetActive(true);
    }

    public void OnMouseExit()
    {
        textAmount.SetActive(false);
    }
}
