using UnityEngine;

public class PacMove : MonoBehaviour
{
    [Header("Прочти первый комент")]
    [Header("Настройки скорости")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    
    // Переменная для отслеживания направления. 
    // false = смотрит влево (как ты и просил), true = вправо
    private bool facingRight = false; 

    private void Awake()
    {
        //Rigidbody сделает движения более плавными чем transform
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // сбрасыаем предидущее
        movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            movement.y = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            movement.y = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            movement.x = 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            movement.x = -1f;

        // убираю ускорение по диагонали
        if (movement.magnitude > 1f)
            movement.Normalize();

        // --- ЛОГИКА ПОВОРОТА СПРАЙТА ---
        // Если нажали D (x > 0), но персонаж смотрит влево (!facingRight) -> поворачиваем
        if (movement.x > 0 && !facingRight)
            Flip();
        // Если нажали A (x < 0), но персонаж смотрит вправо (facingRight) -> поворачиваем
        else if (movement.x < 0 && facingRight)
            Flip();
        // --------------------------------
    }

    private void FixedUpdate()
    {
        // Применяем движение к Rigidbody2D
        rb.velocity = movement * moveSpeed;
    }

    // Метод зеркального отражения
    private void Flip()
    {
        // Меняем булево значение на противоположное
        facingRight = !facingRight;
        
        // Берем текущий масштаб, инвертируем ось X и применяем обратно
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}