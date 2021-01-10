using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

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
    }


    void Start()
    {
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
