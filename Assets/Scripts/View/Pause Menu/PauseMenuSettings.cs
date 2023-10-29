using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuSettings : MonoBehaviour
{
    [SerializeField, Header("FullScreen")] private Toggle _Toggle;
    [SerializeField, Header("Resolution")] private TMP_Dropdown _DropDownMenu;

    private Resolution[] _resolutions;
    private List<Resolution> _filteredResolutions = new();
    private RefreshRate _currentRefreshRate;
    private int _currentResolutionIndex = 0;


    void Start()
    {
        _resolutions = Screen.resolutions;

        _DropDownMenu.ClearOptions();
        _currentRefreshRate = Screen.currentResolution.refreshRateRatio;

        foreach (var item in _resolutions)
        {
            if (item.refreshRateRatio.value == _currentRefreshRate.value)
                _filteredResolutions.Add(item);
        }

        List<string> options = new();
        int index = 0;
        foreach (var item in _filteredResolutions)
        {
            string resOptions = item.width + "x" + item.height;
            options.Add(resOptions);

            if (item.width == Screen.width && item.height == Screen.height)
                _currentResolutionIndex = index;

            index++;
        }

        _DropDownMenu.AddOptions(options);
        _DropDownMenu.value = _currentResolutionIndex;
        _DropDownMenu.RefreshShownValue();


        _Toggle.isOn = Screen.fullScreen;



    }

    public void SetResolition(int resIndex)
    {
        Resolution res = _filteredResolutions[resIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool screenMode)
    {
        Screen.fullScreen = screenMode;
    }
}
