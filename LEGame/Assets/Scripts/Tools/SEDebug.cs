using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SEDebug : MonoBehaviour
{
    [Header("Debug显示配置")]
    //Debug显示字体的大小
    public int fontSize = 20;
    //Debug信息框的显示边距(默认10,从左上顶角计算)
    public int marginTop = 25;
    public int marginLeft = 25;
    public int padding = 10;

    //Debug信息框的宽度和高度
    public int boxWidth = 400;
    public int boxHeight = 250;

    [Header("Debug信息配置")]
    public string msgStake;
    public bool isShowingInfo;

    protected Vector2 velocity;
    protected Player player;

    public void Start()
    {
        player = GameManager.Instance.player.GetComponent<Player>();
    }


    /// <summary>
    /// 仅用于显示当前部分数据和参数的显示框（只读信息，不可更改）
    /// </summary>
    public virtual void OnGUI()
    {
        //作用字体
        GUIStyle style = GUI.skin.GetStyle("label");
        style.fontSize = fontSize;

        //面板背景
        GUI.Box(new Rect(marginLeft,marginTop,boxWidth + padding,boxHeight + padding *2),"");
        
        if(GUI.Button(new Rect(marginLeft + padding,marginTop + padding*2,150,25),"刷新场景物体到管理器"))
        {
            FreshObjects();
        }

        GUI.Label(new Rect(marginLeft + padding,marginTop + padding*3,boxWidth,boxHeight),"InfoDisplay");
        
        if(player != null)
        {
            velocity = player.curVelocity;
        }
        else
        {
            Debug.LogWarning("切换场景后，请点击“刷新场景物体到管理器”按钮，防止Player物体为空");
        }
        GUI.Label(new Rect(marginLeft + padding,marginTop + padding*4,boxWidth,boxHeight),"当前速度："+ velocity.ToString());

        // 创建背景框
        GUI.Box(new Rect(Screen.width-200,10,200,100), "加载关卡");
    
        // 创建第一个按钮。如果按下此按钮，则会执行 Application.Loadlevel (1)
        if(GUI.Button(new Rect(Screen.width-200,10*3,150,25), "Level 1"))
        {
            LoadSceneByNum(1);
        }
    
        // 创建第二个按钮。
        if(GUI.Button(new Rect(Screen.width-200,10*6,150,25),"Level 2")) 
        {
            LoadSceneByNum(2);
        }
    }

    public void FreshObjects()
    {
        GameManager.Instance.player = GameObject.FindWithTag("Player");
            player = GameManager.Instance.player.GetComponent<Player>();
    }

    public void LoadSceneByNum(int index)
    {
        SceneManager.LoadScene(index);
        GameManager.Instance.player = GameObject.FindWithTag("Player");
    }

    public virtual void SetMessage(string newMessage)
    {
       
    }

    public virtual void AddMessage(string newMessage)
    {
        //如果已经在显示信息了，则清空并初始化
        if(isShowingInfo)
        {

        }
        //更新为新的信息
        msgStake += newMessage + "\n";
        //OhterDisplay
    }
}
