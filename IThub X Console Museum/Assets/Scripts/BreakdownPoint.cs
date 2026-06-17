using UnityEngine;

public class BreakdownPoint : MonoBehaviour
{
    [Header("Визуал")]
    [SerializeField] private GameObject exclamationMark; // Спрайт "!"

    public bool IsBroken { get; private set; }

    private void Awake()
    {
        if (exclamationMark != null)
            exclamationMark.SetActive(false);
    }

    // Вызывается Менеджером для создания поломки
    public void Break()
    {
        IsBroken = true;
        if (exclamationMark != null) 
            exclamationMark.SetActive(true);
    }

    // Вызывается при входе игрока в триггер
    public void Fix()
    {
        IsBroken = false;
        if (exclamationMark != null) 
            exclamationMark.SetActive(false);
            
        Debug.Log("Точка починена!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Если точка уже исправна или зашел не игрок — игнорируем
        if (!IsBroken || !other.CompareTag("Player")) 
            return;

        // Игрок зашел в сломанную точку -> чиним её
        Fix();
    }
}