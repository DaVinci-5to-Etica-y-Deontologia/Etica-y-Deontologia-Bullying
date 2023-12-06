using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Euler;
using System.Linq;

#if UNITY_EDITOR 
using UnityEditor;
#endif

/// <summary>
/// Clase que representa la base de datos que contiene los flyweight
/// </summary>
[CreateAssetMenu(menuName = "Managers/BD")]
public class BD : SuperScriptableObject
{
    [field: SerializeField]
    public Material materialForUsers;

    [field: SerializeField]
    public Tutos[] Tutos { get; private set; }

    [field: SerializeField]
    public UsernameGenerator usernameGenerator { get; private set; }

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
    /// <returns>id del comentario seleccionado</returns>
    public int SelectComment(float moralIndex, float moralRange)
    {
        var min = Mathf.Clamp((moralIndex - moralRange), 0, 1);

        var max = Mathf.Clamp((moralIndex + moralRange), 0, 1);

        int minIndex = -1;

        int maxIndex = comments.Length-1;

        for (int i = 0; i < comments.Length; i++)
        {
            if (comments[i].MoralIndex < min)
                minIndex = i;
            else if (comments[i].MoralIndex < max)
                maxIndex = i;
            else
                break;
        }

        return Random.Range(minIndex + 1, maxIndex + 1);
    }

    #region NO TOCAR

#if UNITY_EDITOR

    [SerializeField]
    Parse _txtToParse;

    public event System.Action<BD> OnFinishSet;

    [ContextMenu("cargar los nombres de usuario")]
    void LoadNames()
    {
        usernameGenerator.Execute();
    }

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
        return x.MoralIndex < y.MoralIndex ? -1 : (x.MoralIndex > y.MoralIndex) ? 1 : 0;
    }

#endif

    #endregion
}

[System.Serializable]
public struct Tutos
{
    public Sprite sprite;

    public CompleteText texts;
}

[System.Serializable]
public class CompleteText
{
    [System.Serializable]
    public class TextWithSize
    {
        public float size = 30;
        [TextArea(3, 6)]
        public string text;

        public Color color = Color.white;

        public override string ToString()
        {
            return text.RichText("size", size.ToString()).RichTextColor(color);
        }
    }

    public TextWithSize[] texts;

    public string separator = " ";

    public override string ToString()
    {
        return texts.Aggregate(string.Empty, (ret, text) => ret += text.ToString() + separator);
    }
}
