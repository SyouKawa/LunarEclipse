using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SEDebug : MonoBehaviour
{
    [Header("Debug显示配置")]
    //Debug显示字体的大小
    public int fontSize = 20;
    //Debug信息框的显示边距(默认10,从左上顶角计算)
    public int marginTop = 25;
    public int marginLeft = 25;
    public int padding = 30;

    //Debug信息框的宽度和高度
    public int boxWidth = 400;
    public int boxHeight = 350;
    //Debug Button大小设置
    public int btnWidth = 150;
    public int btnHeight = 50;
    //Debug Label大小设置
    public int lblWidth = 250;
    public int lblHeight = 50;

    [Header("Debug信息配置")]
    public string msgStake;
    public bool isShowingInfo;
    public string levelInput;

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
        //变更将使用的GUI组件的字体大小
        GUIStyle[] styles =  new GUIStyle[5];
        styles[0] = GUI.skin.GetStyle("label");
        styles[2] = GUI.skin.GetStyle("button");
        for(int i=0;styles[i]!=null;i++)
        {
            styles[i].fontSize = fontSize;
        }

        //左侧面板背景配置
        GUI.Box(new Rect(marginLeft,marginTop,boxWidth,boxHeight),"信息显示");
        
        if(GUI.Button(new Rect(marginLeft + padding,marginTop + padding*2,btnWidth,btnHeight),"刷新场景物体到管理器"))
        {
            FreshObjects();
        }
        
        if(player != null)
        {
            velocity = player.curVelocity;
        }
        else
        {
            Debug.LogWarning("切换场景后，请点击“刷新场景物体到管理器”按钮，防止Player物体为空");
        }
        GUI.Label(new Rect(marginLeft + padding,marginTop + padding*4,lblWidth,lblHeight),"当前速度："+ velocity.ToString());
        GUI.Label(new Rect(marginLeft + padding,marginTop + padding*4 +lblHeight,lblWidth,lblHeight),"跳跃速度（上升）"+ player.jumpInitSpeed.ToString());
        GUI.Label(new Rect(marginLeft + padding,marginTop + padding*4 +lblHeight*2,lblWidth,lblHeight),"下落速度（下坠）"+ player.jumpDownSpeed.ToString());
        GUI.Label(new Rect(marginLeft + padding,marginTop + padding*4 + lblHeight *3,lblWidth,lblHeight),"最大跳跃高度"+ player.MaxHeight.ToString());

        // 右侧面板背景配置
        GUI.Box(new Rect(Screen.width-boxWidth,marginTop,boxWidth,boxHeight), "加载关卡");

        GUI.Label(new Rect(Screen.width-boxWidth+padding,marginTop+padding,lblWidth,lblHeight),"前往关卡：");
        levelInput = GUI.TextField(new Rect(Screen.width-boxWidth+150,marginTop+padding,btnHeight,btnHeight),levelInput);
        
        // 创建第一个按钮。如果按下此按钮，则会执行 Application.Loadlevel (1)
        if(GUI.Button(new Rect(Screen.width-boxWidth+250,marginTop + padding,btnWidth,btnHeight), "确认"))
        {
            try
            {
                SceneManager.LoadScene(int.Parse(levelInput));
            }
            catch(System.NullReferenceException nullE)
            {
                Debug.LogWarning("未找到当前level序号，请检查ProjectSetting的关卡配置");
            }
            catch(SystemException nulle)
            {
                Debug.LogWarning("未找到当前level序号，请检查ProjectSetting的关卡配置");
            }
            finally
            {
                Debug.LogError("无法加载关卡，请按错误提示解决场景加载问题");
            }
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
