using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.instance.GetCoins(coinValue);
            Destroy(gameObject);
        }
    }
}
