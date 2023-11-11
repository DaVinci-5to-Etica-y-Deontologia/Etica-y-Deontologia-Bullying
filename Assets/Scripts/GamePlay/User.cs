using Euler;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

/// <summary>
/// Clase destinada a ser un user
/// </summary>
[System.Serializable]
public class UserParent : IDirection
{
    static LinkedPool<CommentData> poolCommentData = new LinkedPool<CommentData>(new CommentData());

    public int ID;

    [field: SerializeField]
    public DataPic<CommentData> comments { get; private set; }

    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public float MoralIndex { get; set; }

    [field: SerializeField]
    public float MoralRange { get; set; }

    [field: SerializeField]
    public bool Enable { get; set; } = true;

    [field: SerializeField]
    public Color colorText { get; set; } = Random.ColorHSV();

    public event System.Action<CommentData> onCreateComment;

    public event System.Action<CommentData> onLeaveComment;

    public BD database => stream.dataBase;

    public EventManager eventManager => stream.eventManager;
    public string textIP => $"{stream.textIP}.{ID}";

    public float CoolDown { get=>_coolDown.current; set=> _coolDown.Set(value); }

    public (CommentData value, int ID, int index) this[int ID]
    {
        get
        {
            return comments.GetTByID(ID);
        }
    }

    int _Admonition
    {
        get => _admonition;
        set
        {
            if (_admonition + value <= 2)
            {
                _admonition += value;
            }
            else 
                Destroy();
        }
    }

    int _admonition;

    Timer _coolDown;

    StreamerData stream;


    #region Moderator

    public void Admonition(int index)
    {
        _Admonition++;
        CoolDown = 30;
        RemoveComment(index);
    }

    public void Ban()
    {
        for (int i = comments.Count - 1; i >= 0; i--)
        {
            RemoveComment(i);
        }

        Destroy();
    }

    #endregion

    #region Instigator
    
        //algo a futuro

    #endregion

    #region rpc
    public void AddComment(string json)
    {
        var newCommentData = poolCommentData.Obtain().Self;

        JsonUtility.FromJsonOverwrite(json, newCommentData);

        newCommentData.Init(stream.ID, ID);

        var auxPic = new Internal.Pictionary<int, CommentData>(newCommentData.ID, newCommentData);

        comments.Add(auxPic);

        onCreateComment?.Invoke(newCommentData);
    }

    public void RemoveComment(int index)
    {
        CommentData comment = comments.GetTByIndex(index).value;

        onLeaveComment?.Invoke(comment);

        comments.RemoveAt(index);

        comment.Destroy();

        if(!Enable && comments.Count==0)
            stream.users.Remove(ID);
    }

    #endregion
   
    public void Aplicate(int views, int damage ,string textIP)
    {
        //Debug.Log($"Aplicar el danio: {commentView.comment.Damage} ganancia de viewers: {commentView.comment.Views}");

        stream.Users(views);

        stream.Life.current += damage;

        DataRpc.Create(Actions.RemoveComment, textIP);
    }


    public void Destroy()
    {
        _coolDown.Stop();
        Enable = false;
        //stream.users.Remove(ID);
    }

    public void CreateComment()
    {
        System.Action lambda = () => 
        {
            if (!Enable)
                return;

            var aux = database.SelectComment(MoralIndex, MoralRange);

            var newCommentData = poolCommentData.Obtain().Self;

            newCommentData.Create(comments.lastID + 1, aux);

            CoolDown = newCommentData.Delay;

            DataRpc.Create(Actions.AddComment, textIP, newCommentData);

            newCommentData.Destroy();
        };

        StreamerManager.eventQueue.Enqueue(lambda);
    }

    public override string ToString()
    {
        return Name + " " + comments.Count;
    }

    public void Init(StreamerData stream)
    {
        this.stream = stream;

        comments = new();

        _coolDown = TimersManager.Create(Random.Range(10, 15), CreateComment);

        colorText=colorText.ChangeAlphaCopy(1);
    }

    public UserParent(int id)
    {
        this.ID = id;

        int rng = Random.Range(5,8);

        string chars = "abcdefghijklmnñopqrstuvwxyz";

        for (int i = 0; i < rng; i++)
        {
            Name += chars[Random.Range(0, chars.Length)];
        }
    }
}


public class User : UserParent
{
    static protected Sprite[] cuerpos;
    static protected Sprite[] cabezas;
    static protected Sprite[] ojos;
    static protected Sprite[] accesorios;
    static protected Sprite[] boquitas;

    [System.Serializable]
    public struct DataIcon
    {
        public int index;
        public Color r;
        public Color g;
        public Color b;

        public void Set(int max)
        {
            index = Random.Range(0, max);

            r = Random.ColorHSV();

            g = Random.ColorHSV();

            b = Random.ColorHSV();
        }

        public void SetImage(UnityEngine.UI.Image image, Material material)
        {
            image.material = new Material(material);

            image.material.SetColor("_r", r);

            image.material.SetColor("_g", g);

            image.material.SetColor("_b", b);
        }
    }

    [SerializeField]
    protected DataIcon cuerpo;

    [SerializeField]
    protected DataIcon cabeza;

    [SerializeField]
    protected DataIcon ojo;

    [SerializeField]
    protected DataIcon accesorio;

    [SerializeField]
    protected DataIcon boquita;

    public static IEnumerator LoadPerfilSprite()
    {
        cuerpos = Resources.LoadAll("Arte/Cuerpo", typeof(Sprite)).Select((r) => r as Sprite).ToArray();
        yield return null;
        cabezas = Resources.LoadAll("Arte/Cabeza", typeof(Sprite)).Select((r) => r as Sprite).ToArray();
        yield return null;
        ojos = Resources.LoadAll("Arte/Ojos", typeof(Sprite)).Select((r) => r as Sprite).ToArray();
        yield return null;
        accesorios = Resources.LoadAll("Arte/Accesorios", typeof(Sprite)).Select((r) => r as Sprite).ToArray();
        yield return null;
        boquitas = Resources.LoadAll("Arte/Boca", typeof(Sprite)).Select((r) => r as Sprite).ToArray();
        yield return null;
    }

    public void SetCuerpo(UnityEngine.UI.Image image)
    {
        image.sprite = cuerpos[cuerpo.index];

        cuerpo.SetImage(image, database.materialForUsers);
    }
    public void SetCabeza(UnityEngine.UI.Image image)
    {
        image.sprite = cabezas[cabeza.index];

        cabeza.SetImage(image, database.materialForUsers);
    }
    public void SetOjos(UnityEngine.UI.Image image)
    {
        image.sprite = ojos[ojo.index];

        ojo.SetImage(image, database.materialForUsers);
    }
    public void SetAccesorio(UnityEngine.UI.Image image)
    {
        image.sprite = accesorios[accesorio.index];

        accesorio.SetImage(image, database.materialForUsers);
    }
    public void SetBoquita(UnityEngine.UI.Image image)
    {
        image.sprite = boquitas[boquita.index];

        boquita.SetImage(image, database.materialForUsers);
    }

    public User(int id) : base(id)
    {
        ojo.Set(ojos.Length);
        boquita.Set(boquitas.Length);
        cabeza.Set(cabezas.Length);
        cuerpo.Set(cuerpos.Length);
        accesorio.Set(accesorios.Length);
    }
}