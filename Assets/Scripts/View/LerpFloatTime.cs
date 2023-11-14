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
        gameObject.SetActive(true);
        enabled = true;
        for (int i = 0; i < fillAmounts.Length; i++)
        {
            fillAmounts[i].Stop();
            fillAmounts[i].On();
        }
    }

    public void Off()
    {
        for (int i = 0; i < fillAmounts.Length; i++)
        {
            fillAmounts[i].Stop();
            fillAmounts[i].Off();
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < fillAmounts.Length; i++)
        {
            fillAmounts[i].OnValidate();
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

    [SerializeField]
    AnimationCurve animationCurve;

    bool enable;

    public void Init()
    {
        fadeOn.alphas += (alpha) =>  fill.Invoke(animationCurve.Evaluate(alpha));

        fadeOn.end += InternalFade;

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

    public void Stop()
    {
        fadeOn.Stop();
    }

    void InternalFade()
    {
        if (enable)
            onEndOn.Invoke();
        else
            onEndOff.Invoke();
    }

    public void OnValidate()
    {
        if (animationCurve == null)
        {
            animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
            return;
        }

        var keys = animationCurve.keys;

        keys[0].time = 0;

        keys[^1].time = 1;

        animationCurve.keys = keys;
    }
}

