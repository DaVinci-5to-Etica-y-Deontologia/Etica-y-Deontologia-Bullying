using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class TabButton : MonoBehaviour
{
    public TabGroup _TabGroup;

    public Animator _Anim { get; private set; }

    void Start()
    {
        Debug.Log("start");
        _Anim = GetComponent<Animator>();
        _TabGroup.Subscribe(this);
    }


    public void Clicked()
    {

        _TabGroup.OnTabSelected(this);
    }

}
