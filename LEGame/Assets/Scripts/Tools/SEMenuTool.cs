using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class SEMenuTool : MonoBehaviour
{
    [MenuItem("SEGame/Add GameManager")]
    public static void AddGameManagerToCurScene()
    {
        if(GameObject.FindWithTag("Manager") == null)
        {
            GameManager MgrOrigin = Resources.Load<GameManager>("Prefabs/GameManager");
            GameObject.Instantiate(MgrOrigin,Vector3.zero,Quaternion.identity);
        }
        else
        {
            Debug.Log("当前场景中已存在GameManager，请勿重复创建");
        }
    }

    [MenuItem("SEGame/Add Player")]
    public static void AddPlayerToCurScene()
    {
        if(GameObject.FindWithTag("Player") == null)
        {
            Player origin = Resources.Load<Player>("Prefabs/Player");
            GameObject.Instantiate(origin,Vector3.zero,Quaternion.identity);
        }
        else
        {
            Debug.Log("当前场景中已存在Player，请勿重复创建");
        }
    }

}
#endif