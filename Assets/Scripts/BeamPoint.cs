
using UnityEngine;
public class BeamPoint : MonoBehaviour
{
    public int damage = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            player.TakeDamage(damage);
            
            Destroy(gameObject); 
        }
    }
}
