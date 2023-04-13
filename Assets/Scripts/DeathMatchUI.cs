using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DeathMatchUI : MonoBehaviour
{
    [SerializeField] private RectTransform imagePoint;
    [SerializeField] private GameObject textAmount;

    [SerializeField] private GameObject textAmountLeft, textAmountRight;

    [SerializeField] private Sprite arrowUp, arrowDown;

    private DeathMatch thisDeathMatch;

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

    //public void SetPoint(double amount, double startAmount, float minY, float maxY, bool win, bool start)
    //{
    //    string calculatedAmountText = $"{amount}b\n";
    //    double percent = GetPercentageDifference(startAmount, amount);
    //    percent = Math.Round(percent, 1);
    //    calculatedAmountText += "<color=" + percentColor(amount, startAmount) + ">(" + percent + "%)</color>";

    //    textAmount.GetComponent<Text>().text = calculatedAmountText;

    //    float y = Mathf.Lerp(minY, maxY, (float)amount / 100);
    //    imagePoint.anchoredPosition = new Vector2(imagePoint.anchoredPosition.x, y);

    //    Image image = imagePoint.GetComponent<Image>();

    //    if (start)
    //    {
    //        image.color = Color.yellow;
    //    }
    //    else if (win)
    //    {
    //        image.color = Color.green;
    //        image.sprite = arrowUp;
    //    }
    //    else
    //    {
    //        image.color = Color.red;
    //        image.sprite = arrowDown;
    //    }
    //}

    // This method sets the point on the graph for the given amount
    // It also updates the text and color of the point based on the start amount and current amount

    public void SetPoint(double amount, double startAmount, float minY, float maxY, bool win, bool start, DeathMatch dm)
    {
        // Store reference to DeathMatch object
        thisDeathMatch = dm;

        // Create a StringBuilder object to build the calculatedAmountText
        StringBuilder calculatedAmountText = new StringBuilder(16);

        // Append the amount and unit to calculatedAmountText
        calculatedAmountText.Append(amount.ToString("F1"));
        calculatedAmountText.AppendLine("b");

        // Calculate the percentage difference between startAmount and amount
        double percent = GetPercentageDifference(startAmount, amount);

        // Round the percent to one decimal place
        percent = Math.Round(percent, 1);

        // Append the percent with color to calculatedAmountText
        calculatedAmountText.AppendFormat("<color={0}>{1}%</color>", percentColor(amount, startAmount), percent);

        // Set the text of the textAmount object to calculatedAmountText
        textAmount.GetComponent<Text>().text = calculatedAmountText.ToString();

        // Calculate the y position of the imagePoint based on the amount and range of y values
        float y = Mathf.Lerp(minY, maxY, (float)amount / 100);

        // Set the anchored position of imagePoint to the calculated y value
        imagePoint.anchoredPosition = new Vector2(imagePoint.anchoredPosition.x, y);

        // Get a reference to the Image component of imagePoint
        Image image = imagePoint.GetComponent<Image>();

        // Set the color and sprite of the image based on whether it is the start point or a win/loss point
        if (start)
        {
            image.color = Color.yellow;
        }
        else
        {
            image.color = win ? Color.green : Color.red;
            image.sprite = win ? arrowUp : arrowDown;
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

    public void OnPointerClick()
    {
        if (thisDeathMatch != null)
            FindObjectOfType<GameManager>().EditDeathMatch(thisDeathMatch);
    }
}
