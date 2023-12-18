using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public GameObject item;

    public bool isItem, isPickup;
    public int itemCost;

    public TMP_Text costText;

    // Start is called before the first frame update
    void Start()
    {
        if (isPickup)
        {
            item = Instantiate(item, transform.position, transform.rotation);
            item.GetComponent<CircleCollider2D>().enabled = false;
        }

        item.transform.position = transform.position;
        costText.text = $"{itemCost}$";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (LevelManager.instance.currentCoins >= itemCost)
            {
                LevelManager.instance.SpendCoins(itemCost);
                costText.enabled = false;
                if (isItem)
                {
                    StartCoroutine(PlayerController.instance.BuyItem(item, this));
                }
                if (isPickup)
                {
                    item.GetComponent<CircleCollider2D>().enabled = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}
