using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float up;

    void CheckMove()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        Vector2 p = transform.position;
        p.x += moveX * speed * Time.deltaTime;
        transform.position = p;
        //改变朝向
        if (moveX != 0)
        {
            transform.localScale = new Vector3(moveX, 1,1);
        }
    }

    void CheckJump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            //TODO：待讨论
        }
    }

    void Update()
    {
        CheckMove();
        CheckJump();
    }
}
