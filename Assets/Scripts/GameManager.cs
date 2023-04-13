using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("(b) for example, 0.8 is 0.8 billion")]
    private double startingAmount;

    //[SerializeField, Tooltip("Transform that holds the list of deathmatches in the settings UI")] 
    //private Transform verticalDeathmatchHolder;

    [SerializeField, Tooltip("Transform that holds the list of deathmatches in the main graph UI")] 
    private Transform horizontalDeathmatchHolder;

    [SerializeField, Tooltip("Deathmatch prefab for the horizontal list")] 
    private GameObject deathmatchPrefab;

    [SerializeField, Tooltip("StartingAmount inputfield")] 
    private InputField startingAmountInputField;

    [SerializeField, Tooltip("AddDM inputfield")] 
    private InputField addDeathmatchInputField;

    [SerializeField, Tooltip("Win and Loss text")] 
    private Text textWins, textLosses;


    private List<DeathMatch> matches = new List<DeathMatch>();

    public void ApplyStartingAmount()
    {
        double amount = 0;
        if (double.TryParse(startingAmountInputField.text, out amount))
        {
            startingAmount = amount;
            UpdateGraph();
        }
    }

    public void AddDM(bool win)
    {
        double amount = 0;
        if (double.TryParse(addDeathmatchInputField.text, out amount))
        {
            DeathMatch dm = new DeathMatch(amount, "", win);
            matches.Add(dm);
            UpdateGraph();
        }
    }

    private void UpdateGraph()
    {
        // Clear graph
        for (int i = horizontalDeathmatchHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(horizontalDeathmatchHolder.GetChild(i).gameObject);
        }

        // Starting point (starting amount)
        GameObject startingPoint = Instantiate(deathmatchPrefab, horizontalDeathmatchHolder);
        startingPoint.GetComponent<DeathMatchUI>().SetPoint(startingAmount, startingAmount, -450, 450, false, true);
        startingPoint.GetComponent<DeathMatchUI>().ShowTextAmountLeft(startingAmount);

        double currentAmount = startingAmount;
        int wins = 0;
        int losses = 0;

        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].win)
            {
                currentAmount += matches[i].amount;
                wins++;
            }
            else
            {
                currentAmount -= matches[i].amount;
                losses++;
            }

            GameObject newMatch = Instantiate(deathmatchPrefab, horizontalDeathmatchHolder);
            newMatch.GetComponent<DeathMatchUI>().SetPoint(currentAmount, startingAmount, -450, 450, matches[i].win, false);
            if (i == matches.Count - 1)
                newMatch.GetComponent<DeathMatchUI>().ShowTextAmountRight(currentAmount);
        }

        textWins.text = $"W: {wins}";
        textLosses.text = $"L: {losses}";
    }

    public void OnSpreadSliderValueChanged(float value)
    {
        horizontalDeathmatchHolder.GetComponent<HorizontalLayoutGroup>().spacing = value;
    }
}

public class DeathMatch
{
    public double amount;
    public string opponentName;
    public bool win;

    public DeathMatch(double amount, string opponentName, bool win)
    {
        this.amount = amount;
        this.opponentName = opponentName;
        this.win = win;
    }
}