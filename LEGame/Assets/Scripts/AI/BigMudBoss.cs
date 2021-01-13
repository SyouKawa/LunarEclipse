using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMudBoss : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Status
    {
        Hide,
        StartAnim,
        Part1,
        Part2,
        Part3,
        FakeDeath,
        Die
    }
    public Status curState;
    private Fist Fist1;
    private Fist Fist2; 
    void Start()
    {
        curState = Status.Hide;
        Fist1 = transform.GetChild(1).GetComponent<Fist>();
        Fist2 = transform.GetChild(2).GetComponent<Fist>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
