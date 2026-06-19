using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboard : MonoBehaviour
{
    [Header("UI Элементы")]
    [Tooltip("Все кнопки с буквами A-Z в массиве (строго по порядку: A, B, C... Z)")]
    public Button[] letterButtons;
    
    [Tooltip("Кнопка удаления (DEL)")]
    public Button delButton;
    
    [Tooltip("Кнопка подтверждения (ENTER)")]
    public Button enterButton;

    [Tooltip("Текстовое поле, где отображается вводимое имя (например 'A__')")]
    public Text inputDisplayText; // <-- ЭТО ПОЛЕ БЫЛО ЗАБЫТО!

    [Header("Визуал выделения")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int selectedIndex = 0;
    private string currentInput = "";
    private const int MAX_LENGTH = 3;

    public void Initialize()
    {
        currentInput = "";
        UpdateInputDisplay();
        SelectButton(0);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        // === НАВИГАЦИЯ ПО ВИРТУАЛЬНОЙ КЛАВИАТУРЕ ===
        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveSelection(1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveSelection(-1);
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveSelection(-9); // Переход на строку выше (сетка 3x9)
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveSelection(9);  // Переход на строку ниже

        // === ВВОД С ФИЗИЧЕСКОЙ КЛАВИАТУРЫ (быстрый способ) ===
        for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
        {
            if (Input.GetKeyDown(key))
            {
                string letter = key.ToString();
                if (currentInput.Length < MAX_LENGTH)
                {
                    currentInput += letter;
                    UpdateInputDisplay();
                }
                return;
            }
        }

        // Backspace - удалить последний символ
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (currentInput.Length > 0)
            {
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                UpdateInputDisplay();
            }
            return;
        }

        // Enter или Space - подтвердить
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedIndex == letterButtons.Length + 1)
            {
                OnEnterClick();
            }
            else if (currentInput.Length == MAX_LENGTH)
            {
                OnEnterClick();
            }
            return;
        }
    }

    void MoveSelection(int step)
    {
        int newIndex = selectedIndex + step;
        int maxIndex = letterButtons.Length + 1;
        
        if (newIndex < 0) newIndex = 0;
        if (newIndex > maxIndex) newIndex = maxIndex;
        
        if (newIndex != selectedIndex)
            SelectButton(newIndex);
    }

    void SelectButton(int index)
    {
        ResetButtonColor(selectedIndex);
        selectedIndex = index;
        SetButtonColor(selectedIndex, selectedColor);
    }

    void SetButtonColor(int index, Color color)
    {
        if (index < letterButtons.Length)
        {
            var img = letterButtons[index].GetComponent<Image>();
            if (img) img.color = color;
        }
        else if (index == letterButtons.Length && delButton != null)
        {
            var img = delButton.GetComponent<Image>();
            if (img) img.color = color;
        }
        else if (index == letterButtons.Length + 1 && enterButton != null)
        {
            var img = enterButton.GetComponent<Image>();
            if (img) img.color = color;
        }
    }

    void ResetButtonColor(int index)
    {
        SetButtonColor(index, normalColor);
    }

    public void OnLetterClick(int index)
    {
        if (index >= 0 && index < letterButtons.Length)
        {
            string letter = letterButtons[index].GetComponentInChildren<Text>().text;
            if (currentInput.Length < MAX_LENGTH)
            {
                currentInput += letter;
                UpdateInputDisplay();
            }
        }
    }

    public void OnDeleteClick()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateInputDisplay();
        }
    }

    public void OnEnterClick()
    {
        if (currentInput.Length == MAX_LENGTH)
        {
            HighScoreManager.Instance.SubmitName(currentInput);
        }
    }

    void UpdateInputDisplay()
    {
        if (inputDisplayText != null)
            inputDisplayText.text = currentInput.PadRight(MAX_LENGTH, '_');
    }
}