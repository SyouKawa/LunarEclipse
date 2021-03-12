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
            GameObject player = transform.parent.gameObject;
            Vector2 dir =  enemy.transform.position - player.transform.position;
            dir = dir.normalized;
            //触发敌人被击
            enemy.backDir = dir;
            enemy.OnBeHit(new OtherData(player.GetComponent<Player>()));
        }
	}
}
