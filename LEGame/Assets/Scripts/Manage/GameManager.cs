using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public GameObject player;

    //获取当前玩家的朝向
    public float GetPlayerDir()
    {
        return player.transform.localScale.x;
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject); return;
        }
        else
        {
            _instance = this;
        }

        player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.LogWarning("警告：当前场景不存在Player.");
        }
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q) && SceneManager.GetActiveScene().buildIndex!=0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
