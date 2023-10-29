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
        Tabs holder = new()
        {
            Button = button,
            Tab = button.tabToLoad
        };
        _TabButtons.Add(holder);
    }


    public void OnTabEnter(TabButton button)
    {
    }
    public void OnTabExit(TabButton button)
    {
    }

    public void OnTabSelected(TabButton button)
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
        public TabButton Button;
        public Tab Tab;
    }
}
