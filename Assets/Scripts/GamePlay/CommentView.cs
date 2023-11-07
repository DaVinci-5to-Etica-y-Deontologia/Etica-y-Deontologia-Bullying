using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommentView : MonoBehaviour
{
    public int ID;

    public User user;

    [field: SerializeField]
    public Comment comment { get; private set; }

    [SerializeField]
    EventManager eventManager;

    [SerializeField]
    TextMeshProUGUI textMesh;

    [SerializeField]
    Image perfil;

    [SerializeField]
    Button button;

    //rpc
    public void Init(int idStream,int idUser)
    {
        this.user = StreamerManager.instance.streamers.GetTByID(idStream)?.users.GetTByID(idUser);

        if (user == null)
        {
            StreamerManager.instance.streamers.GetTByID(idStream).LeaveComment(ID);
            return;
        }
            

        perfil.sprite = user.Perfil;

        textMesh.text = comment.Text.RichText("color", "#" + ColorUtility.ToHtmlStringRGBA(user.colorText));
    }

    public void Create(int id, Comment comment)
    {
        this.ID = id;

        this.comment = comment;

        JsonOverride(JsonUtility.ToJson(this));
    }

    public void Aplicate()
    {
        user.Aplicate(this);
    }

    public void OnClick()
    {
        eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato.Invoke(this);
    }

    public void AnimRefresh()
    {
    }

    //rpc
    void JsonOverride(string json)
    {
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
