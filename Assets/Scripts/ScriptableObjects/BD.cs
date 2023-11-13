using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Euler;
using System.Linq;

#if UNITY_EDITOR 
using UnityEditor;
#endif

/// <summary>
/// Clase que representa la base de datos que contiene todos y cada uno de los usuarios con sus respectivos comentarios
/// </summary>
[CreateAssetMenu(menuName = "Managers/BD")]
public class BD : SuperScriptableObject
{
    [field: SerializeField]
    public Material materialForUsers;

    [field: SerializeField]
    public Streamer[] Streamers { get; private set; }

    [field: SerializeField]
    public Comment[] comments { get; private set; }

    public void OnDisable()
    {
        for (int i = 0; i < Streamers.Length; i++)
        {
            Streamers[i].inUse = false;
        }
    }

    public int SelectStreamer()
    {
        var index = -1;

        do
        {
            index = Random.Range(0, Streamers.Length);
        } while(Streamers[index].inUse);

        Streamers[index].inUse = true;

        return index;
    }


    /// <summary>
    /// Adaptar al nuevo sistema
    /// </summary>
    /// <param name="moralIndex"></param>
    /// <param name="moralRange"></param>
    /// <returns></returns>
    public Comment SelectComment(float moralIndex, float moralRange)
    {
        var aux = Mathf.Clamp(Random.Range(moralIndex - moralRange, moralIndex + moralRange), 0, 1);


        return comments[Random.Range(0, comments.Length)];
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

        List<Comment> comments = new List<Comment>();

        string debug = string.Empty;

        for (int i = 0; i < _txtToParse.DataOriginalOrder.Count; i++)
        {
            debug += _txtToParse.DataOriginalOrder[i].ToString() + "\n";

            comments.Add(MakeNewChild<Comment>(i.ToString(), (comment)=> comment.Initilize(_txtToParse.DataOriginalOrder[i])));
        }

        _txtToParse.DataOriginalOrder.Clear();

        debug += "\n";

        comments.Sort(Sort);

        this.comments = comments.ToArray();

        Debug.Log(debug);

        OnFinishSet?.Invoke(this);

        OnFinishSet = null;
    }

    [ContextMenu("Borrar tablas relacionadas")]
    void DeleteAll()
    {
        comments?.Delete();


    }

    int Sort(Comment x, Comment y)
    {
        return int.Parse(x.name) <= int.Parse(y.name) ? -1 : 1;
    }

#endif

    #endregion
}
