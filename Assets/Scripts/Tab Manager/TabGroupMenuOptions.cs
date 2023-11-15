using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupMenuOptions : MonoBehaviour
{
    public List<Tabs> _TabButtons = new();
    public List<Tab> _Tabs;

    private TabButtonMenuOptions _selectedTab;

    public void Subscribe(TabButtonMenuOptions button)
    {
        Tabs holder = new()
        {
            Button = button,
            Tab = button.tabToLoad
        };
        _TabButtons.Add(holder);
    }


    public void OnTabEnter(TabButtonMenuOptions button)
    {
    }
    public void OnTabExit(TabButtonMenuOptions button)
    {
    }

    public void OnTabSelected(TabButtonMenuOptions button)
    {
        _selectedTab = button;

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

    [System.Serializable]
    public class Tabs
    {
        public TabButtonMenuOptions Button;
        public Tab Tab;
    }
}
