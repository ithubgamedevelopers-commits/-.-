using UnityEngine;

public class PixelPerfectMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    [Tooltip("Скорость перемещения к целевой точке")]
    public float moveSpeed = 5f;

    [Tooltip("Слои, которые считаются препятствиями")]
    public LayerMask obstacleLayer;

    private Vector2 targetPosition;
    private bool isMoving = false;
    private BoxCollider2D boxCollider;

    // Кэшируем размер коллайдера для расчёта шага
    private float colliderWidth;
    private float colliderHeight;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            Debug.LogError("На объекте нет BoxCollider2D! Скрипт не будет работать корректно.");
            return;
        }

        // Рассчитываем размер шага на основе размера коллайдера
        // Это гарантирует, что персонаж будет двигаться ровно на свою ширину/высоту
        colliderWidth = boxCollider.size.x * Mathf.Abs(transform.localScale.x);
        colliderHeight = boxCollider.size.y * Mathf.Abs(transform.localScale.y);

        // Начальная позиция — это и есть цель
        targetPosition = transform.position;
    }

    void Update()
    {
        // Проверяем ввод только если персонаж не движется
        if (!isMoving)
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (input != Vector2.zero)
            {
                // Определяем размер шага в зависимости от направления
                // Для вертикального движения используем высоту коллайдера, для горизонтального - ширину
                float step = (Mathf.Abs(input.x) > 0) ? colliderWidth : colliderHeight;

                // Рассчитываем новую целевую позицию
                Vector2 newTarget = targetPosition + input * step;

                // Выполняем рейкаст для проверки возможности движения
                Vector2 direction = (newTarget - (Vector2)transform.position).normalized;
                float distance = Vector2.Distance(transform.position, newTarget);

                // Если на пути нет препятствий, начинаем движение
                if (!Physics2D.Raycast(transform.position, direction, distance, obstacleLayer))
                {
                    targetPosition = newTarget;
                    isMoving = true;
                }
            }
        }
        else
        {
            // Двигаем объект к цели
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Проверяем достижение цели с помощью квадрата расстояния для точности
            // 0.001f - это небольшая погрешность (эпсилон)
            if ((targetPosition - (Vector2)transform.position).sqrMagnitude < 0.001f)
            {
                transform.position = targetPosition; // Устанавливаем позицию точно, чтобы избежать дрейфа
                isMoving = false;
            }
        }
    }
}