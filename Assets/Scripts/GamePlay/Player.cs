using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/Player")]
public class Player : ScriptableObject
{
    [Tooltip("true for moderator, false to Instigator")]
    public bool Moderator;

    [SerializeField, Range(10,30)]
    public int NumerOfCommentsToView = 20;
}
