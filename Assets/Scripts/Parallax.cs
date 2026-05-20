using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f;
    private float spriteWidth;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x <= startPos.x - spriteWidth)
            transform.position = startPos;
    }
}