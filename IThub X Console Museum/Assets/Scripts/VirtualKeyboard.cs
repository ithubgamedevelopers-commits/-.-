using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VirtualKeyboard : MonoBehaviour
{
    public TextMeshProUGUI inputDisplay;
    public TextMeshProUGUI errorText;       
    public Button submitButton;      
    public Button backspaceButton;
    public GameObject keyPrefab; 
    public Transform keysContainer;

    private int currentScore;
    private string currentInput = "";
    private const int MAX_LENGTH = 5;

    void Start()
    {
        GenerateKeyboard();
        gameObject.SetActive(false);
    }

    public void Initialize(int score)
    {
        currentScore = score;
        currentInput = "";
        errorText.gameObject.SetActive(false);
        UpdateDisplay();
        gameObject.SetActive(true);
        
        // Автофокус на первую букву для навигации геймпадом
        if (keysContainer.childCount > 0)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(keysContainer.GetChild(0).gameObject);
    }

    private void GenerateKeyboard()
    {
        foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
        {
            GameObject btnObj = Instantiate(keyPrefab, keysContainer);
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => AddChar(c.ToString()));
            btnObj.GetComponentInChildren<TextMeshProUGUI>().text = c.ToString();
        }
    }

    public void AddChar(string c)
    {
        if (currentInput.Length < MAX_LENGTH)
        {
            currentInput += c;
            errorText.gameObject.SetActive(false);
            UpdateDisplay();
        }
    }

    public void Backspace()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            errorText.gameObject.SetActive(false);
            UpdateDisplay();
        }
    }

    public void Submit()
    {
        HighScoreManager.Instance.SaveRecord(currentInput, currentScore);
    }

    public void ShowError(string msg)
    {
        errorText.text = msg;
        errorText.gameObject.SetActive(true);
        currentInput = "";
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        inputDisplay.text = currentInput.PadRight(MAX_LENGTH, '_');
        submitButton.interactable = (currentInput.Length == MAX_LENGTH);
    }
}