using System;
using UnityEngine;

public class SingletonProvider<T> where T : class, new()
{
    private SingletonProvider(){}

    private static T _instance;
    private static readonly object _synclock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_synclock)
                {
                    if (_instance == null)
                    {
                        // 若T class具有私有构造函数,那么则无法使用SingletonProvider<T>来实例化new T();
                        _instance = new T();
                        //测试用，如果T类型创建了实例，则输出它的类型名称
                        Debug.Log("{0}：创建了单例对象" + typeof(T).Name);
                    }
                }
            }
            return _instance;
        }
        set { _instance = value; }
    }
}