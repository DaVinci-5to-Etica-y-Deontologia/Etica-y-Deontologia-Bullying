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
    public string Comentario { get; private set; }

    [field: SerializeField, TextArea(3, 6)]
    public string UnBan { get; private set; }

    [field: SerializeField]
    public int Views { get; private set; }

    [field: SerializeField]
    public int Damage { get; private set; }

    [field: SerializeField]
    public float Deley { get; private set; }

    /*
    [field: SerializeField]
    public User[] ToRespond { get; private set; }
    */



    /////////////////////////////////////////////////////////////////////////
    ///
    public new User Parent => (User)base.Parent;

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

        UnBan = param["Unban"];

        Comentario = param["Comentario"];

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        Parent.Parent.OnFinishSet += (bd) => 
        {
            /*
            List<User> users = new List<User>();

            foreach (var item in param["Condicion necesaria"].Split(","))
            {
                users.Add(bd[item]);
            }
            
            ToRespond = users.ToArray();
            */
        };
        /////////////////////////////////////////////////////////////////////////
        ///
        return this;
    }

#endif
}
