using UnityEngine;

public class LerpFloatTime : MonoBehaviour
{
    public FillAmount[] fillAmounts;

    private void Awake()
    {
        for (int i = 0; i < fillAmounts.Length; i++)
        {
            fillAmounts[i].Init();
        }
    }

    private void OnEnable()
    {
        On();
    }

    public void On()
    {
        enabled = true;
        for (int i = 0; i < fillAmounts.Length; i++)
        {
            fillAmounts[i].On();
        }
    }

    public void Off()
    {
        for (int i = 0; i < fillAmounts.Length; i++)
        {
            fillAmounts[i].Off();
        }
    }
}

[System.Serializable]
public class FillAmount
{
    [SerializeField]
    FadeOnOff fadeOn;

    [SerializeField]
    bool onActive;

    [SerializeField]
    bool onDeactive;

    [SerializeField]
    public UnityEngine.Events.UnityEvent<float> fill;

    [SerializeField]
    public UnityEngine.Events.UnityEvent onEndOn;

    [SerializeField]
    public UnityEngine.Events.UnityEvent onEndOff;

    bool enable;

    public void Init()
    {
        fadeOn.alphas += (alpha) => fill.Invoke(alpha);

        fadeOn.end += InternalFade;

        fadeOn.unscaled = true;

        fadeOn.Init();
    }

    public void On()
    {
        if (onActive)
        {
            enable = true;
            fadeOn.FadeOn();
        }
            
    }

    public void Off()
    {
        if (onDeactive)
        {
            enable = false;
            fadeOn.FadeOff();
        }
    }

    void InternalFade()
    {
        if (enable)
            onEndOn.Invoke();
        else
            onEndOff.Invoke();
    }
}

