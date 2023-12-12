using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/Player")]
public class Player : ScriptableObject
{
    [field: Tooltip("true for moderator, false to Instigator")]
    public bool Moderator { get; set; }

    [field: SerializeField, Range(10,30)]
    public int NumerOfCommentsToView { get; set; } = 20;

    [field: SerializeField]
    public float InstigatorCooldownFirst { get; set; } = 3;

    [field: SerializeField]
    public float InstigatorCooldownSecond { get; set; } =5;

    [field: SerializeField]
    public float Multiply { get; set; } = 1;

    [field: SerializeField]
    public int ID;

    public void SetModerator(bool b) => Moderator = b;

    public void SetMultiply(int i) => Multiply = 0.5f + i * 0.25f;
}
