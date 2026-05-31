using UnityEngine;

// Эти строки гарантируют, что у персонажа будут нужные компоненты.
// Если их нет, Unity добавих их автоматически.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float jumpForce = 12f;
    public bool canDoubleJump = true;

    [Header("Ground Check Settings")]
    public Transform groundCheckPoint;        // Пустой объект у ног персонажа
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;             // Слой "Ground"

    // Компоненты
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    // Переменные для состояния
    private float moveInput;
    private bool isGrounded;
    private int extraJumps;

    void Start()
    {
        // Получаем компоненты
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        // Настраиваем Rigidbody2D для лучшей физики [citation:4]
        rb.freezeRotation = true;                                      // Чтобы персонаж не падал на бок
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Для быстрого движения
        rb.gravityScale = 3f;                                          // Сила гравитации
    }

    void Update()
    {
        // Получаем ввод с клавиатуры (A/D или стрелки). GetAxisRaw дает -1, 0 или 1 без плавности.
        moveInput = Input.GetAxisRaw("Horizontal");

        // 1. ПРОВЕРКА НА ЗЕМЛЕ:
        // Самый надежный способ - проверка круговым сканером у ног [citation:4]
        // Можно также использовать capsuleCollider.IsTouchingLayers(groundLayer) для простоты,
        // но метод с groundCheckPoint точнее.
        if (groundCheckPoint != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
        }
        else
        {
            // Если вы забыли создать groundCheckPoint, используем запасной метод через коллайдер
            isGrounded = capsuleCollider.IsTouchingLayers(groundLayer);
        }

        // Сбрасываем двойной прыжок, когда касаемся земли
        if (isGrounded)
        {
            extraJumps = canDoubleJump ? 1 : 0;
        }

        // 2. ПРЫЖОК:
        // Проверяем нажатие на кнопку прыжка (Space по умолчанию)
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                // Обычный прыжок с земли
                Jump();
            }
            else if (extraJumps > 0)
            {
                // Двойной прыжок в воздухе
                Jump();
                extraJumps--;
            }
        }

        // Опционально: "высота прыжка от нажатия кнопки"
        // Если отпустить кнопку прыжка раньше, персонаж не долетит до максимума [citation:2]
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    // Эта функция вызывается в FixedUpdate для работы с физикой [citation:4]
    void FixedUpdate()
    {
        // 3. ДВИЖЕНИЕ:
        // Применяем скорость. Используем linearVelocity (новое название в новых версиях Unity)
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Небольшое улучшение: плавное торможение в воздухе (раскомментируйте если нужно)
        // if (!isGrounded) {
        //     rb.linearVelocity = new Vector2(moveInput * moveSpeed * 0.9f, rb.linearVelocity.y);
        // }
    }

    private void Jump()
    {
        // Обнуляем вертикальную скорость для одинаковой высоты прыжка
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        // Прикладываем силу прыжка
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // Визуализация зоны проверки земли в редакторе (для удобства настройки)
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
