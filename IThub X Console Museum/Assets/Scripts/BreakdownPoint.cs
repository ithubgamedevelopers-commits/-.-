using UnityEngine;

public class BreakdownPoint : MonoBehaviour
{
    [Header("Визуал")]
    public GameObject exclamationMark;

    public bool IsBroken { get; private set; }

    private void Awake()
    {
        if (exclamationMark != null)
            exclamationMark.SetActive(false);
        
        IsBroken = false;
    }

    public void Break()
    {
        IsBroken = true;
        if (exclamationMark != null)
            exclamationMark.SetActive(true);
        
        Debug.Log($"💥 Компьютер {gameObject.name} сломан!");
    }

    public void Fix()
    {
        IsBroken = false;
        if (exclamationMark != null)
            exclamationMark.SetActive(false);
        
        Debug.Log($" Компьютер {gameObject.name} починен!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsBroken || !other.CompareTag("Player"))
            return;

        Debug.Log($"🎯 Игрок вошёл в триггер {gameObject.name}");

        if (SkillCheckManager.Instance != null)
        {
            // Передаём ССЫЛКУ НА СЕБЯ, чтобы менеджер знал, кого чинить
            SkillCheckManager.Instance.StartCheck(this);
        }
        else
        {
            Debug.LogError("❌ SkillCheckManager.Instance = null!");
        }
    }
}