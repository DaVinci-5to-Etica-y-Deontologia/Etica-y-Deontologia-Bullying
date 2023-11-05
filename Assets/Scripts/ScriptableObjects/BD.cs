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

    public IEnumerable<User> EnableAllLazyness()
    {
        foreach (var user in users)
        {
            user.Enable = true;

            yield return user;
        }   
    }

    public IEnumerable<User> ResetAllLazyness()
    {
        foreach (var user in EnableAllLazyness())
        {
            user.Ban = false;

            yield return user;
        }
    }

    public void ResetAll()
    {
        foreach (var item in ResetAllLazyness())
        {
        }
    }


    #region NO TOCAR

#if UNITY_EDITOR

    [SerializeField]
    Parse _txtToParse;

    public event System.Action<BD> OnFinishSet;

    [ContextMenu("Cargar el texto")]
    void LoadText()
    {
        DeleteAll();

        _txtToParse.Execute();

        Dictionary<string,List<PDO<string, string>>> _users = new();

        List<Comment> comments = new List<Comment>();

        string debug = string.Empty;

        int index = 0;

        //separo en users
        for (int i = 0; i < _txtToParse.DataOriginalOrder.Count; i++)
        {
            if (_users.TryGetValue(_txtToParse.DataOriginalOrder[i][1], out var aux))
                aux.Add(_txtToParse.DataOriginalOrder[i]);
            else
            {
                _users.Add(_txtToParse.DataOriginalOrder[i][1], new List<PDO<string, string>>() { _txtToParse.DataOriginalOrder[i] });
            }

            debug += _txtToParse.DataOriginalOrder[i].ToString() + "\n";
        }

        _txtToParse.DataOriginalOrder.Clear();

        debug += "\n";

        //agrego en los scriptables
        foreach (var user in _users)
        {
            users[index] = MakeNew<User>(user.Key, (userClass) => userClass.Initilize(user.Value)); //creo en disco el scriptable

            debug += users[index] + "\n";

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
