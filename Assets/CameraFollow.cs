using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Цель")]
    public Transform target; // игрок

    [Header("Плавность")]
    public float smoothSpeed = 5f;

    [Header("Границы уровня")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private Camera cam;
    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        cam = GetComponent<Camera>();
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.orthographicSize * cam.aspect;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Плавно следуем за игроком
        Vector3 desiredPos = new Vector3(target.position.x, target.position.y, transform.position.z);
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // Зажимаем в границы
        smoothedPos.x = Mathf.Clamp(smoothedPos.x, minX + camHalfWidth, maxX - camHalfWidth);
        smoothedPos.y = Mathf.Clamp(smoothedPos.y, minY + camHalfHeight, maxY - camHalfHeight);

        transform.position = smoothedPos;
    }
}