using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Statistics stats;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddStats()
    {
        PlayerController.instance.stats.damage += stats.damage;
        PlayerController.instance.stats.attackSpeed += stats.attackSpeed;
        PlayerController.instance.stats.moveSpeed += stats.moveSpeed;
        PlayerController.instance.stats.damage += stats.attackRange;
        PlayerController.instance.stats.shotSpeed += stats.shotSpeed;
        PlayerController.instance.stats.criticalChance += stats.criticalChance;

        UIController.instance.UpdateStats();
    }
}