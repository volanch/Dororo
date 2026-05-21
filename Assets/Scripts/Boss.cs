using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    public int bullet_count = 5;
    public GameObject BulletPrefab, LaserSunPrefab;

    public Player Player;
    
    
    public Transform[] movePoints;
    private int currentPointIndex = 0;
    private float speed = 2f;
    
    void Start()
    {
        StartCoroutine(BossBehaviorRoutine());
    }

    void FixedUpdate()
    {
        if (movePoints.Length > 0)
            transform.position = Vector3.MoveTowards(transform.position, 
                movePoints[currentPointIndex].position, 
                speed * Time.fixedDeltaTime);
        
    }

    IEnumerator BossBehaviorRoutine()
    {
        while (true) {
            currentPointIndex = Random.Range(0, movePoints.Length);
            speed = Random.Range(1f, 3f);

            int action = Random.Range(0, 3); 
            
            switch (action) {
                case 0: case 1: PerformAttack2(); break;
                case 2: break;
            }

            yield return new WaitForSeconds(3f);
        }
    }

    void PerformAttack1()
    {
        if (!BulletPrefab) return;

        for (int i = 0; i < bullet_count; i++) {
            
            Vector2 randomCirclePoint = Random.insideUnitCircle * 3;
            Vector3 spawnPosition = transform.position + new Vector3(randomCirclePoint.x, randomCirclePoint.y, 0f);
            Instantiate(BulletPrefab, spawnPosition, Quaternion.identity);
        }
    }

    void PerformAttack2()
    {
        if (!LaserSunPrefab) return;
        
        Vector3 spawnPosition = Player.transform.position + new Vector3(0, 4, 0f);
        Instantiate(LaserSunPrefab, spawnPosition, Quaternion.identity);
    }
    
    
    
    
    
    
    
    //tamer llllllllllllegendddddddd
}