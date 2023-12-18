using System.Collections;
using UnityEngine;

public class FloorSpikes : MonoBehaviour
{
    public bool areRetractable;
    private bool canDamage;
    private float hiddenTime = 1.5f, shownTime = 1.5f;
    private float hiddenShownDeltaTime = 0.15f;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        canDamage = true;

        if (areRetractable)
        {
            StartCoroutine(HideAndShow());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (canDamage)
            {
                PlayerHealthController.instance.TakeDamage(1);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (canDamage)
            {
                PlayerHealthController.instance.TakeDamage(1);
            }
        }
    }

    public IEnumerator HideAndShow()
    {
        anim.Play("Retractable_Spikes_Hide");
        canDamage = false;

        for (float i = 0f; i < hiddenTime; i += hiddenShownDeltaTime)
        {
            yield return new WaitForSeconds(hiddenShownDeltaTime);
        }

        canDamage = true;
        anim.Play("Retractable_Spikes_Show");

        for (float i = 0f; i < shownTime; i += hiddenShownDeltaTime)
        {
            yield return new WaitForSeconds(hiddenShownDeltaTime);
        }

        StartCoroutine(HideAndShow());
    }
}
