using UnityEngine;
public class PacMove : MonoBehaviour
{
    [Header("Прочти первый комент")]
    [Header("Настройки скорости")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 movement;
        // для скрита надо: Rigidbody2D(без гравитации), любой из коллайдеров, тег Player 
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>(); //для плавности
            sr = GetComponent<SpriteRenderer>(); //для поворота
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
        
        // Отзеркаливание спрайта при движении влево/вправо
        if (movement.x > 0)
            sr.flipX = false;
        else if (movement.x < 0)
            sr.flipX = true;
    }

    private void FixedUpdate()
    {
        // Применяем движение к Rigidbody2D
        rb.velocity = movement * moveSpeed;
    }
}