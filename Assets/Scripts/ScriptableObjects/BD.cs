using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Euler;


#if UNITY_EDITOR 
using UnityEditor;
#endif

/// <summary>
/// Clase que representa la base de datos que contiene todos y cada uno de los usuarios con sus respectivos comentarios
/// </summary>
[CreateAssetMenu(menuName = "BD")]
public class BD : SuperScriptableObject
{
    [SerializeField]
    TextAsset _textAsset;

    [field: SerializeField]
    public User[] users { get; private set; }

    [field: SerializeField]
    public Comment[] comments { get; private set; }

    public int Length => users.Length;

    public Comment this[int index]
    {
        get
        {
            return comments[index];
        }
    }

    public User this [string name]
    {
        get
        {
            for (int i = 0; i < users.Length; i++)
            {
                if (users[i].name == name)
                    return users[i];
            }

            return null;
        }
    }


    #region NO TOCAR

#if UNITY_EDITOR

    public event System.Action<BD> OnFinishSet;

    [ContextMenu("Cargar el texto")]
    void LoadText()
    {
        DeleteAll();

        Parse _parse = new Parse(_textAsset);
        
        users = new User[_parse.DataUser.Count];

        List<Comment> comments = new List<Comment>();

        string debug = string.Empty;

        int index = 0;
        
        foreach (var user in _parse.DataUser)
        {
            List<PDO<string,string>> data = new List<PDO<string, string>>();

            debug += user + "\n";

            for (int i = _parse.DataOrdered.Count -1 ; i >= 0 ; i--)
            {
                debug += "\t" + _parse.DataOrdered[i].ToString() + "\n";

                if (_parse.DataOrdered[i][1] == user)
                {
                    data.Insert(0,_parse.DataOrdered[i]);
                    _parse.DataOrdered.RemoveAt(i);
                }                
            }

            users[index] = MakeNew<User>(user, (userClass) => userClass.Initilize(data)); //creo en disco el scriptable

            comments.AddRange(users[index].comments);

            index++;
        }

        comments.Sort(Sort);

        this.comments = comments.ToArray();

        Debug.Log(debug);

        OnFinishSet?.Invoke(this);

        OnFinishSet = null;
    }

    [ContextMenu("Borrar tablas relacionadas")]
    void DeleteAll()
    {
        users?.Delete();
    }

    int Sort(Comment x, Comment y)
    {
        return int.Parse(x.name) <= int.Parse(y.name) ? -1 : 1;
    }

#endif

    #endregion
}
