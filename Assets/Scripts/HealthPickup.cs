using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthController.instance.Heal(healAmount);
            
            Destroy(gameObject);
        }
    }
}
