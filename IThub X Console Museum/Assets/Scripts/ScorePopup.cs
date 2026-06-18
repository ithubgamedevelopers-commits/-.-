using UnityEngine;
using UnityEngine.UI;

public class ScorePopup : MonoBehaviour
{
    [Header("Ссылки")]
    public Text textComponent;

    [Header("Настройки")]
    public float moveSpeed = 100f;
    public float lifeTime = 1.5f;

    private Vector3 targetPosition;
    private float currentLife;

    public void Setup(string message, Color color, Vector3 startPosition)
    {
        textComponent.text = message;
        textComponent.color = color;
        transform.position = startPosition;
        
        targetPosition = startPosition + new Vector3(0, 80f, 0);
        currentLife = lifeTime;
        
        gameObject.SetActive(true);
    }

    void Update()
    {
        currentLife -= Time.deltaTime;
        
        if (currentLife <= 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        Color c = textComponent.color;
        c.a = Mathf.Clamp01(currentLife / 0.5f);
        textComponent.color = c;
    }
}