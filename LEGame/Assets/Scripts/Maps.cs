using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:不需要赋值", Justification = "<挂起>")]
public class Maps : MonoBehaviour
{

    public int height;//y
    public int width;//x

    public int MoveSpeed;

    public GameObject hexTile;

    private void SpawnMainMap()
    {
        for (int i = 0; i < width; i++)
        {
            float deltaHeight = 0;
            if (i % 2 == 0)
            {
                deltaHeight = 0;
            }
            else
            {
                deltaHeight = 0.433f;
            }

            float x = 0.75f * i; //长轴加半边长

                for (int j = 0;j< height;j++ )
            {
                //（√3/4）≈0.433f,六边形短轴长
                Vector3 pos = new Vector3(x, deltaHeight + (2 * j * 0.433f + 0.433f), 0);
                GameObject curTile = GameObject.Instantiate(hexTile, pos, Quaternion.identity);
                curTile.name = pos.ToString();
            }
        }
    }

    private void FillColorByButton()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 tempPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(tempPos, Vector3.forward);
            Ray2D ray2D = new Ray2D(tempPos, Vector3.forward);
            Debug.DrawRay(ray2D.origin, ray2D.direction, Color.blue,20);
            if (hit.collider != null)
            {
                GameObject cur = hit.collider.gameObject;
                Debug.Log(hit.collider.gameObject.name);
                if (Input.GetKey(KeyCode.B))
                {
                    cur.GetComponent<SpriteRenderer>().color = new Color(97/255f,221/255f,212/255f,1);
                }
                if (Input.GetKey(KeyCode.G))
                {
                    cur.GetComponent<SpriteRenderer>().color = new Color(108/255f, 202/255f, 77/255f, 1);
                }
            }
        }
    }

    private void CheckRollScreen()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector3 p = Camera.main.transform.position;
        p.x += moveX * MoveSpeed * Time.deltaTime;//摄像机方向和地图方向相反
        p.y += moveY * MoveSpeed * Time.deltaTime;
        p.z = Camera.main.transform.position.z;
        Camera.main.transform.position = p;
    }

    void Start()
    {
        SpawnMainMap();
    }

    
    void Update()
    {
        CheckRollScreen();
        FillColorByButton();
    }
}
