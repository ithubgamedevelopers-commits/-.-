using UnityEngine;
public class PacMove : MonoBehaviour
{
    [Header("Прочти первый комент")]
    [Header("Настройки скорости")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;
        // для скрита надо: Rigidbody2D(без гравитации), любой из коллайдеров 
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
    }

    private void FixedUpdate()
    {
        // Применяем движение к Rigidbody2D
        rb.velocity = movement * moveSpeed;
    }
}