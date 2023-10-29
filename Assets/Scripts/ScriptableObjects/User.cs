using Euler;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase destinada a ser un user
/// </summary>
public class User : SuperScriptableObject
{
    #region NO TOCAR

    [field: SerializeField]
    public Comment[] comments { get; private set; }

    #endregion

    ////////////////////////////////////////////////////////
    //Colocar las variables para el usuario

    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public bool Ban { get; set; }

    [field: SerializeField]
    public bool Enable { get; set; }

    [field: SerializeField]
    public Sprite Perfil { get; set; }

    [field: SerializeField]
    public Color colorText { get; set; }


    //////////////////////////////////////////////////////

    public new BD Parent => (BD)base.Parent;


#if UNITY_EDITOR

    public User Initilize(List<PDO<string, string>> param)
    {
        //////////////////////////////////////////////////////////////////////////////////
        //Colocar aqui el seteo de las variables para el usuario

        Name = name;
        Enable = true;




        //Fin
        //////////////////////////////////////////////////////////////////////////////////

        //No tocar
        InternalSet(param);
        return this;
    }

    
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////
   
    #region NO TOCAR
    public override void DeleteThis()
    {
        comments?.Delete();

        base.DeleteThis();
    }

    /// <summary>
    /// recive una lista de todos los comentarios separados reglones, con sus parametros dentro del array
    /// </summary>
    /// <param name="param"></param>
    void InternalSet(List<PDO<string, string>> param)
    {
        comments = new Comment[param.Count];
        for (int i = 0; i < comments.Length; i++)
        {
            comments[i] = MakeNewChild<Comment>(param[i][0], (comments) => comments.Initilize(param[i]));
        }
    }
    #endregion

#endif

}
