using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransitioner : MonoBehaviour
{
    public Direction direction;

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
            switch (direction)
            {
                case Direction.up:
                    PlayerController.instance.transform.position += new Vector3(0f, 3f, 0f);
                    break;

                case Direction.right:
                    PlayerController.instance.transform.position += new Vector3(3f, 0f, 0f);
                    break;

                case Direction.down:
                    PlayerController.instance.transform.position += new Vector3(0f, -3f, 0f);
                    break;

                case Direction.left:
                    PlayerController.instance.transform.position += new Vector3(-3f, 0f, 0f);
                    break;
            }
        }
    }
}
