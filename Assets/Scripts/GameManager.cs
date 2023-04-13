using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField, Tooltip("(b) for example, 0.8 is 0.8 billion")]
    private double startingAmount;

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

    private Camera mainCamera;

    private DeathMatch editingDeathMatch = null;

    [SerializeField]
    private GameObject settingsEditDeathMatch;
    [SerializeField]
    private InputField editDmInputField;

    [SerializeField] private GameObject graphScale;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnHideScaleToggleChanged(bool toggle)
    {
        graphScale.SetActive(!toggle);
    }

    public void ApplyStartingAmount()
    {
        if (double.TryParse(startingAmountInputField.text, out startingAmount))
        {
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

    // This method is used to update the DeathMatch graph with the current state of the matches list
    private void UpdateGraph()
    {
        // Clear the graph by destroying all child objects in the horizontalDeathmatchHolder game object
        for (int i = horizontalDeathmatchHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(horizontalDeathmatchHolder.GetChild(i).gameObject);
        }

        // Instantiate a starting point for the graph and set its properties
        GameObject startingPoint = Instantiate(deathmatchPrefab, horizontalDeathmatchHolder);
        startingPoint.GetComponent<DeathMatchUI>().SetPoint(startingAmount, startingAmount, -450, 450, false, true, null);
        startingPoint.GetComponent<DeathMatchUI>().ShowTextAmountLeft(startingAmount);

        // Declare and initialize variables to store the current amount, number of wins, and number of losses
        double currentAmount = startingAmount;
        int wins = 0;
        int losses = 0;

        // Loop through each DeathMatch object in the matches list
        for (int i = 0; i < matches.Count; i++)
        {
            // Update the current amount based on the win/loss and amount of the current DeathMatch object
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

            // Instantiate a new DeathMatchUI object for the current DeathMatch object and set its properties
            GameObject newMatch = Instantiate(deathmatchPrefab, horizontalDeathmatchHolder);
            newMatch.GetComponent<DeathMatchUI>().SetPoint(currentAmount, startingAmount, -450, 450, matches[i].win, false, matches[i]);

            // If the current DeathMatch object is the last one in the matches list, show the amount on the right side of the graph
            if (i == matches.Count - 1)
                newMatch.GetComponent<DeathMatchUI>().ShowTextAmountRight(currentAmount);
        }

        // Set the text of the wins and losses UI elements
        textWins.text = $"W: {wins}";
        textLosses.text = $"L: {losses}";

        // Call the RenderLineAtEndOfFrame coroutine to render the line connecting the points on the graph
        StartCoroutine(RenderLineAtEndOfFrame());
    }

    public void OnSpreadSliderValueChanged(float value)
    {
        horizontalDeathmatchHolder.GetComponent<HorizontalLayoutGroup>().spacing = value;
        StartCoroutine(RenderLineAtEndOfFrame());
    }

    [SerializeField] 
    private LineRenderer lineRenderer;

    public void OnHorizontalScrollChanged()
    {
        StartCoroutine(RenderLineAtEndOfFrame());
    }

    public void OnRenderLineToggle(bool value)
    {
        lineRenderer.enabled = value;
        StartCoroutine(RenderLineAtEndOfFrame());
    }

    IEnumerator RenderLineAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        RenderLine();
    }

    // This method is used to render a line connecting the points on the DeathMatch graph
    private void RenderLine()
    {
        // Check if the lineRenderer is enabled, if not return
        if (!lineRenderer.enabled) return;

        // Get the number of child objects in the horizontalDeathmatchHolder game object
        int childCount = horizontalDeathmatchHolder.childCount;

        // Declare and initialize an array of Vector3 to store the points on the graph
        Vector3[] points = new Vector3[childCount];

        // Loop through each child object in the horizontalDeathmatchHolder game object
        for (int i = 0; i < childCount; i++)
        {
            // Get the Transform component of the current child object
            Transform child = horizontalDeathmatchHolder.GetChild(i);

            // Get the position of the child object in world space using the mainCamera
            Vector3 point = mainCamera.ScreenToWorldPoint(new Vector3(child.GetChild(0).GetComponent<RectTransform>().position.x, child.GetChild(0).GetComponent<RectTransform>().position.y, mainCamera.nearClipPlane));

            // Add the point to the points array
            points[i] = point;
        }

        // Set the number of positions in the lineRenderer to the number of child objects in the horizontalDeathmatchHolder game object
        lineRenderer.positionCount = childCount;

        // Set the positions of the lineRenderer to the points array
        lineRenderer.SetPositions(points);
    }

    #region EDIT
    // This method is used to edit a DeathMatch object
    public void EditDeathMatch(DeathMatch dm)
    {
        // Check if the DeathMatch object being edited is not the same as the current editingDeathMatch
        if (dm != editingDeathMatch)
        {
            // Set the editingDeathMatch to the current DeathMatch object being edited
            editingDeathMatch = dm;

            // Set the text of the editDmInputField based on whether the DeathMatch object has won or lost
            // If the DeathMatch object has won, the text will be an empty string
            // If the DeathMatch object has lost, the text will be a negative number followed by the amount lost
            editDmInputField.text = (dm.win ? "" : "-") + dm.amount;

            // Activate the settingsEditDeathMatch game object
            settingsEditDeathMatch.SetActive(true);
        }
        else
        {
            // Set the editingDeathMatch to null and deactivate the settingsEditDeathMatch game object
            editingDeathMatch = null;
            settingsEditDeathMatch.SetActive(false);
        }
    }

    // This method is called when the Edit Apply button is clicked
    public void Button_EditApply()
    {
        // Declare and initialize a variable to store the amount entered in the editDmInputField
        double amount = 0;

        // Try to parse the text in the editDmInputField as a double
        if (double.TryParse(editDmInputField.text, out amount))
        {
            // Set the amount of the editingDeathMatch object to the absolute value of the parsed amount
            editingDeathMatch.amount = Mathf.Abs((float)amount);

            // Check if the parsed amount is negative
            if (amount < 0)
                editingDeathMatch.win = false; // If it is negative, set win to false (i.e. the DeathMatch object lost)
            else
                editingDeathMatch.win = true; // If it is not negative, set win to true (i.e. the DeathMatch object won)

            // Call the UpdateGraph method to update the graph with the changes made to the DeathMatch object
            UpdateGraph();

            // Deactivate the settingsEditDeathMatch game object and set editingDeathMatch to null
            settingsEditDeathMatch.SetActive(false);
            editingDeathMatch = null;
        }
    }

    public void Button_EditDelete()
    {
        matches.Remove(editingDeathMatch);
        editingDeathMatch = null;
        settingsEditDeathMatch.SetActive(false);
        UpdateGraph();
    }

    #endregion

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