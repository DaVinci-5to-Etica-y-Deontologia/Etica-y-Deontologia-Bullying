using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TabGroup _TabGroup;

    public Animator _Anim { get; private set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _TabGroup.OnTabEnter(this);
    }

    public void OnPointerClick()
    {
        _TabGroup.OnTabSelected(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _TabGroup.OnTabExit(this);
    }

    void Start()
    {
        _Anim = GetComponent<Animator>();
        _TabGroup.Subscribe(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
