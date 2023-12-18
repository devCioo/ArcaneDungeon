using System.Collections;
using UnityEngine;

public class ItemPedestal : MonoBehaviour
{
    public GameObject item;

    // Start is called before the first frame update
    void Start()
    {
        item.transform.position = transform.position + new Vector3(0f, 1f, 0f);
        StartCoroutine(AnimatePedestalItem());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (item != null)
            {
                StopAllCoroutines();
                StartCoroutine(PlayerController.instance.CollectItem(item));
                item = null;
            }
        }
    }

    private IEnumerator AnimatePedestalItem()
    {
        for (float i = 0f; i < 0.5f; i += 0.01f)
        {
            item.transform.position += new Vector3(0f, 0.01f, 0f);
            yield return new WaitForSeconds(0.01f);
        }
        for (float i = 0f; i < 0.4f; i += 0.01f)
        {
            item.transform.position += new Vector3(0f, -0.0125f, 0f);
            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine(AnimatePedestalItem());
    }
}
