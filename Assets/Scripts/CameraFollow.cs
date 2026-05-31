using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 0f, -10f);
    public float smoothSpeed = 0.125f;

    [Header("Bounds")]
    public PolygonCollider2D boundsCollider;  // Перетащите сюда объект с PolygonCollider2D

    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;
    private Bounds bounds;

    void Start()
    {
        if (boundsCollider != null)
        {
            // Получаем границы коллайдера
            bounds = boundsCollider.bounds;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        desiredPosition = target.position + offset;

        // Ограничиваем камеру внутри коллайдера
        if (boundsCollider != null)
        {
            // Получаем размеры камеры
            Camera cam = Camera.main;
            float cameraHeight = cam.orthographicSize * 2f;
            float cameraWidth = cameraHeight * cam.aspect;

            // Ограничиваем позицию, чтобы камера не выходила за границы
            float minX = bounds.min.x + cameraWidth / 2f;
            float maxX = bounds.max.x - cameraWidth / 2f;
            float minY = bounds.min.y + cameraHeight / 2f;
            float maxY = bounds.max.y - cameraHeight / 2f;

            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    // Обновляем границы, если коллайдер меняется
    void OnDrawGizmosSelected()
    {
        if (boundsCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(boundsCollider.bounds.center, boundsCollider.bounds.size);
        }
    }
}
