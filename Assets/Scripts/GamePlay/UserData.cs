using Euler;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

/// <summary>
/// Clase destinada a ser un user
/// </summary>
[System.Serializable]
public class UserParent : DataElement<UserParent>
{
    static LinkedPool<CommentData> poolCommentData = new LinkedPool<CommentData>(new CommentData());

    [field: SerializeField]
    public DataPic<CommentData> comments { get; private set; } = new();

    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField]
    public float MoralIndex { get; private set; }

    [field: SerializeField]
    public float MoralRange { get; private set; }



    [field: SerializeField]
    public int Suspect { get; set; } = 0;

    [field: SerializeField]
    public Color colorText { get; set; } = Random.ColorHSV();

    public event System.Action<CommentData> onCreateComment;

    public event System.Action<CommentData> onLeaveComment;

    public event System.Action<float> onMoralRangeChange;

    public event System.Action<float> onMoralIndexChange;

    public event System.Action<int> onSuspectChange;


    public override string textIP => $"{stream.textIP}.{ID}";



    public float CoolDown { get=>_coolDownToComment.current; set=> _coolDownToComment.Set(value); }

    protected override IDataElement parent => stream;

    public (CommentData value, int ID, int index) this[int ID]
    {
        get
        {
            return comments.GetTByID(ID);
        }
    }


    [SerializeField]
    float _moralIndex;

    [SerializeField]
    float _newMoralIndex;

    [SerializeField]
    float _moralRange;

    [SerializeField]
    float _newMoralRange;

    Timer _coolDownToComment;

    Timer _coolDownAdmonition;

    Timer _moralIndexCooldown;

    Timer _moralRangeCooldown;

    StreamerData stream;

    #region suscription

    public static void Ban(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.User.Ban();
    }
    public static void Admonition(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.User.Admonition(srch.comment.index);
    }
    public static void Picantear(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.User.Picantear();
    }
    public static void ChangeMoral(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.User.ChangeMoral();
    }
    public static void SuspectChange(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.User.SuspectChange(jsonData);
    }
    public static void AddComment(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.User.AddComment(jsonData);
    }
    public static void RemoveComment(string jsonData, StreamerManager.SearchResult srch)
    {
        srch.User.RemoveComment(srch.comment.index);
    }



    #endregion


    #region Moderator

    public void Admonition(int index)
    {
        CoolDown = 30;

        if (!_coolDownAdmonition.Chck)
            Destroy();

        RemoveComment(index);
    }

    public void Ban()
    {
        Destroy();

        for (int i = comments.Count - 1; i >= 0; i--)
        {
            RemoveComment(i);
        }        
    }

    public void SuspectChange(string index)
    {
        Suspect = int.Parse(index);

        onSuspectChange?.Invoke(Suspect);
    }

    #endregion

    #region Instigator
    
    /// <summary>
    /// Baja el rango moral a la mitad y disminuye el indice en un tercio
    /// </summary>
    public void ChangeMoral()
    {
        _moralIndexCooldown.Reset();

        _moralRangeCooldown.Reset();

        _newMoralRange /= 2;

        _newMoralIndex -= 1f / 3;
    }

    /// <summary>
    /// Publica un comentario en base a un indice 0.5 menor, y luego baja su indice un 0.25
    /// </summary>
    public void Picantear()
    {
        _moralIndexCooldown.Reset();

        _newMoralIndex -= 0.5f;

        //para publicar un comentario picante

        if(IsServer)
            _coolDownToComment.current = 0;

        _newMoralIndex += 0.25f;
    }
    #endregion

    #region rpc
    public void AddComment(string jsonCommentData)
    {
        var newCommentData = poolCommentData.Obtain();

        JsonUtility.FromJsonOverwrite(jsonCommentData, newCommentData);

        newCommentData.Parent = poolCommentData;

        newCommentData.Init(stream.ID, ID);

        if (IsServer)
            CoolDown = newCommentData.Delay;

        comments.Add(newCommentData.CreatePic());

        onCreateComment?.Invoke(newCommentData);
    }

    public void RemoveComment(int index)
    {
        CommentData comment = comments.GetTByIndex(index).value;

        comments.RemoveAt(index);        

        onLeaveComment?.Invoke(comment);

        comment.Destroy();

        if (!Enable && comments.Count == 0)
            stream.users.Remove(ID);
    }

    #endregion
   
    public void Aplicate(int views, float damage ,string textIP)
    {
        Debug.Log($"Aplicar el danio: {damage} ganancia de viewers: {views}");
        if (stream.ShowEnd)
            return;

        stream.Users(views);

        stream.Life.current += damage;

        DataRpc.Create(Actions.RemoveComment, textIP);
    }

    public void Stop()
    {
        _coolDownToComment?.Stop();
    }

    public void Destroy()
    {
        Enable = false;
        stream.Viewers.current--;
        Stop();
    }

    public void CreateComment()
    {
        if (!IsServer && !Enable)
            return;

        System.Action lambda = () => 
        {
            if (!Enable)
                return;

            var auxFlyweight = dataBase.SelectComment(MoralIndex, MoralRange);

            var newCommentData = poolCommentData.Obtain();

            newCommentData.Create(comments.Prepare(), auxFlyweight);

            DataRpc.Create(Actions.AddComment, textIP, newCommentData);

            newCommentData.Destroy();
        };

        StreamerManager.eventQueue.Enqueue(lambda);
    }

    public override string ToString()
    {
        return Name + " " + comments.Count;
    }

    

    void _moralIndexCooldown_onChange(IGetPercentage arg1, float arg2)
    {
        onMoralIndexChange?.Invoke(MoralIndex);
    }

    void _moralRangeCooldown_onChange(IGetPercentage arg1, float arg2)
    {
        onMoralRangeChange?.Invoke(MoralRange);
    }

    public void Init(StreamerData stream)
    {
        this.stream = stream;

        _coolDownAdmonition = TimersManager.Create(15).SetInitCurrent(0).SetMultiply(player.multiply);

        _moralIndexCooldown = TimersManager.Create<float>(() => _newMoralIndex, _moralIndex, 30, Mathf.Lerp, (s) => MoralIndex = s).SetMultiply(player.multiply).Stop();

        _moralRangeCooldown = TimersManager.Create<float>(() => _newMoralRange, _moralRange, 30, Mathf.Lerp, (s) => MoralRange = s).SetMultiply(player.multiply).Stop();

        _moralRangeCooldown.onChange += _moralRangeCooldown_onChange;

        _moralIndexCooldown.onChange += _moralIndexCooldown_onChange;
    }

    public void Create(StreamerData stream)
    {
        Init(stream);

        stream.Viewers.current++;

        colorText = colorText.ChangeAlphaCopy(1);

        if (IsServer)
            _coolDownToComment = TimersManager.Create(Random.Range(10, 15), CreateComment).SetMultiply(player.multiply);
    }

    public UserParent(int id)
    {
        this.ID = id;

        int rng = Random.Range(5,8);

        MoralIndex = Random.Range(0, 1f);

        _moralIndex = MoralIndex;

        MoralRange = Random.Range(0, 0.5f);

        _moralRange = MoralRange;

        Enable = true;

        string chars = "abcdefghijklmnñopqrstuvwxyz";

        for (int i = 0; i < rng; i++)
        {
            Name += chars[Random.Range(0, chars.Length)];
        }
    }
}

[System.Serializable]
public class UserData : UserParent
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

            r = Random.ColorHSV(0, 1, 0, 1, 0.3f, 1);

            g = Random.ColorHSV(0, 1, 0, 1, 0.3f, 1);

            b = Random.ColorHSV(0, 1, 0, 1, 0.3f, 1);
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

        cuerpo.SetImage(image, dataBase.materialForUsers);
    }
    public void SetCabeza(UnityEngine.UI.Image image)
    {
        image.sprite = cabezas[cabeza.index];

        cabeza.SetImage(image, dataBase.materialForUsers);
    }
    public void SetOjos(UnityEngine.UI.Image image)
    {
        image.sprite = ojos[ojo.index];

        ojo.SetImage(image, dataBase.materialForUsers);
    }
    public void SetAccesorio(UnityEngine.UI.Image image)
    {
        image.sprite = accesorios[accesorio.index];

        accesorio.SetImage(image, dataBase.materialForUsers);
    }
    public void SetBoquita(UnityEngine.UI.Image image)
    {
        image.sprite = boquitas[boquita.index];

        boquita.SetImage(image, dataBase.materialForUsers);
    }

    public UserData(int id) : base(id)
    {
        ojo.Set(ojos.Length);
        boquita.Set(boquitas.Length);
        cabeza.Set(cabezas.Length);
        cuerpo.Set(cuerpos.Length);
        accesorio.Set(accesorios.Length);
    }
}