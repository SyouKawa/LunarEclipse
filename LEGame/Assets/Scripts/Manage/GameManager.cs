using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public GameObject player;
    public GameObject DebugInfoPanel;

    /// <summary>
    /// 获取当前玩家的朝向
    /// </summary>
    /// <returns>返回 > 0则朝右，返回 < 0则朝左</returns>
    public float GetPlayerDir()
    {
        return player.transform.localScale.x;
    }

    private void Awake()
    {
        //单例处理
        if (_instance != null)
        {
            Destroy(this.gameObject); return;
        }
        else
        {
            _instance = this;
        }
        //玩家筛查
        player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.LogWarning("警告：当前场景不存在Player.");
        }
        //周期管理
        DontDestroyOnLoad(gameObject);
        //Debug显示配置
        InstantiateDebugPanel();
    }

    /// <summary>
    /// Debug显示面板的初始化
    /// </summary>
    private void InstantiateDebugPanel()
    {
        DebugInfoPanel = new GameObject("DebugInfoPanel");
        DebugInfoPanel.transform.SetParent(this.transform);
        DebugInfoPanel.AddComponent<SEDebug>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q) && SceneManager.GetActiveScene().buildIndex!=0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
