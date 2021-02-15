using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject player;
    void OnTriggerEnter2D(Collider2D other) {
		Debug.Log(other.name);
        Monster enemy = other.GetComponentInParent<Monster>();
        if(enemy !=null)
        {
            enemy.BeHit(player.GetComponent<Player>().Att);
        }
	}
}
