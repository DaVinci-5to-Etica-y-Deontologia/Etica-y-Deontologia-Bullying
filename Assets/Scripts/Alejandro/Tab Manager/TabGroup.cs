using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<Tabs> _TabButtons;
    public List<Tab> _Tabs;

    private TabButton _selectedTab;

    public void Subscribe(TabButton button)
    {
        if (_TabButtons == null)
            _TabButtons = new();

        int index = button.transform.GetSiblingIndex();
        Tabs holder = new()
        {
            Button = button,
            Tab = _Tabs[index]
        };
        holder.Tab.gameObject.SetActive(false);
        _TabButtons.Add(holder);
    }


    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (_selectedTab == null || button != _selectedTab)
            PlayAnim(button, "enter");
    }
    public void OnTabExit(TabButton button)
    {
        ResetTabs();
        PlayAnim(button, "exit");
    }

    public void OnTabSelected(TabButton button)
    {
        _selectedTab = button;
        ResetTabs();
        PlayAnim(button, "selected");
        foreach (Tabs tab in _TabButtons)
        {
            if (tab.Button == button)
                tab.Tab.gameObject.SetActive(true);
            else
                tab.Tab.gameObject.SetActive(false);
        }
    }

    public void ResetTabs()
    {
        foreach (Tabs tab in _TabButtons)
        {
            if (_selectedTab != null && tab.Button == _selectedTab) { continue; }
            PlayAnim(tab.Button, "reset");
        }
    }


    public void PlayAnim(TabButton button, string anim)
    {
        button._Anim.ResetTrigger(anim);
        button._Anim.SetTrigger(anim);
    }

    [System.Serializable]
    public class Tabs
    {
        public TabButton Button;
        public Tab Tab;
    }
}
