using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Euler;

/// <summary>
/// Clase destinada a representar todos lo que contiene un comentario
/// </summary>
public class Comment : SuperScriptableObject
{
    //////////////////////////////////////////////////////////////////////////
    ///Colocar aqui las variables para los comentarios
    [field: SerializeField, TextArea(3,6)]
    public string Text { get; private set; }

    [field: SerializeField, TextArea(3, 6)]
    public string UnBan { get; private set; }

    [field: SerializeField]
    public int Views { get; private set; }

    [field: SerializeField]
    public int Damage { get; private set; }

    [field: SerializeField]
    public float Deley { get; private set; }

    [field: SerializeField]
    public float MoralIndex { get; private set; }


    /////////////////////////////////////////////////////////////////////////
    ///
    public new BD Parent => (BD)base.Parent;

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }

    private void OnValidate()
    {
        if(Parent!=null)
        {
            bool destroy = true;
            for (int i = 0; i < Parent.comments.Length; i++)
            {
                if (Parent.comments[i]==this)
                    destroy = false;
            }

            if(!destroy)
            {
                return;
            }
        }

        DestroyImmediate(this,true);
    }

#if UNITY_EDITOR

    public Comment Initilize(PDO<string, string> param)
    {
        //////////////////////////////////////////////////////////////////////////
        ///Colocar aqui el seteo de variables para los comentarios, segun el orden de columna, la primera es el ID
        if(int.TryParse(param["Views"], out var aux0))
            Views = aux0;

        if (int.TryParse(param["Damage"], out var aux3))
            Damage = aux3;

        if (float.TryParse(param["Deley"], out var aux2))
            Deley = aux2;

        if (float.TryParse(param["Indice Moral"], out var aux5))
            MoralIndex = aux5;

        Text = param["Comentario"];

        return this;
    }

#endif
}
