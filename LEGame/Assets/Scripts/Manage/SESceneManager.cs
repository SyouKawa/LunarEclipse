using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class SESceneManager : MonoBehaviour
{
    public int sceneNum;

    public void JumpTo(int index) {
        sceneNum = index;
        SceneManager.LoadScene(sceneNum);
    }

    public int GetCurLevelIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}
