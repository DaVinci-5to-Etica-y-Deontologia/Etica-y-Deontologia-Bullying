using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FadeOnOff : Init
{
    public float durationAnim = 0.3f;

    public float durationWait = 0.1f;

    public event System.Action<float> alphas;
    public event System.Action end;

    [SerializeReference]
    public Timer timerOn;

    public bool unscaled = true;

    Vector2 fades;

    Timer fadeOn;    

    public bool fadeFinish => fadeOn.Chck;

    public void Init()
    {
        fadeOn = TimersManager.Create(() => fades.x, () => fades.y, durationAnim, Mathf.Lerp, alphas).AddToEnd(() => end?.Invoke()).SetUnscaled(unscaled).Stop();

        timerOn = TimersManager.Create(durationWait, () =>
        {
            fadeOn.Reset();

        }).SetUnscaled(unscaled).Stop();
    }

    public Timer FadeOn()
    {
        return SetFade(0, 1);
    }

    public Timer FadeOff()
    {
        return SetFade(1, 0);
    }

    public Timer SetFade(float init, float end)
    {
        alphas?.Invoke(init);
        fades.x = init;
        fades.y = end;

        timerOn.Reset();

        return timerOn;
    }

    public void Stop()
    {
        timerOn?.Stop();
        fadeOn?.Stop();
    }
}
