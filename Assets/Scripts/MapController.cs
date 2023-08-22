using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public static MapController instance;

    public Transform target;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!UIController.instance.isBigMapActive)
            {
                UIController.instance.isBigMapActive = true;
                UIController.instance.map.SetActive(false);
                UIController.instance.bigMap.SetActive(true);
            }
            else
            {
                UIController.instance.isBigMapActive = false;
                UIController.instance.map.SetActive(true);
                UIController.instance.bigMap.SetActive(false);
            }
        }
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
