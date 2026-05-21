using System.Collections;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public Transform[] movePoints;
    void Start()
    {
        StartCoroutine(Cprpt());
    }

    private float speedom = 0;

    IEnumerator Cprpt()
    {
        yield return new WaitForSeconds(1);
        speedom = 1;
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    void Update()
    {
        transform.Translate(Vector3.down * (6 * speedom* Time.deltaTime));

        for (var i = 0; i < movePoints.Length; i++) {
            var point = movePoints[i];
            
            point.transform.Translate(Vector3.left * ((-2 + i) * speedom* 2 * Time.deltaTime));
        }
    }
}
