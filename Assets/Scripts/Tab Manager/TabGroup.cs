using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<Tabs> _TabButtons = new();
    public List<Tab> _Tabs;

    private TabButton _selectedTab;

    public void Subscribe(TabButton button)
    {

        int index = button.transform.GetSiblingIndex();
        Tabs holder = new()
        {
            Button = button,
            Tab = _Tabs[index]
        };
        _TabButtons.Add(holder);
    }


    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
    }
    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        _selectedTab = button;
        ResetTabs();

        foreach (Tabs tab in _TabButtons)
        {
            if (tab.Button == button)
            {
                tab.Tab._Anim.SetTrigger("Selected");
                tab.Tab._Anim.ResetTrigger("Deselected");
            }
            else
            {
                tab.Tab._Anim.SetTrigger("Deselected");
                tab.Tab._Anim.ResetTrigger("Selected");
            }
                
        }
    }

    public void ResetTabs()
    {
        foreach (Tabs tab in _TabButtons)
        {
            if (_selectedTab != null && tab.Button == _selectedTab) { continue; }
        }
    }



    [System.Serializable]
    public class Tabs
    {
        public TabButton Button;
        public Tab Tab;
    }
}
