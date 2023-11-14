using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{ 
    public EventManager eventManager;
    
    public EventCallsManager callsManager;

    public Player player;

    [SerializeField]
    PopUpElement[] popUpElements;

    public CommentView commentViewPrefab;

    protected void Awake()
    {
        popUpElements = GetComponentsInChildren<PopUpElement>(true);

        foreach (var item in popUpElements)
        {
            item.MyAwake(this);
        }
    }
}

public abstract class PopUpElement : MonoBehaviour
{
    public PopUpManager parent;

    protected EventManager eventManager => parent.eventManager;

    protected EventCallsManager callsManager=> parent.callsManager;

    protected CommentView commentViewPrefab => parent.commentViewPrefab;

    protected Player player => parent.player;

    public UnityEngine.Events.UnityEvent onActive;

    public UnityEngine.Events.UnityEvent onExecute;

    /// <summary>
    /// Funcion que ejecutara el popmanager en el awake, necesario para setear el eventmanager y el callsmanager
    /// </summary>
    /// <param name="popUpManager"></param>
    virtual public void MyAwake(PopUpManager popUpManager)
    {
        parent = popUpManager;
    }
}

public abstract class PopUpComment : PopUpElement
{
    [SerializeField]
    protected TMPro.TextMeshProUGUI userName;

    [SerializeField]
    protected TMPro.TextMeshProUGUI textToShow;

    [SerializeField]
    protected Transform placeToCreate;

    [SerializeField]
    protected Color commentDestroy;

    protected CommentData comment;

    protected User user;

    protected abstract bool ExecutePopUp { get; }

    public override void MyAwake(PopUpManager popUpManager)
    {
        base.MyAwake(popUpManager);
       
        eventManager.events.SearchOrCreate<EventParam<CommentData>>("createcomment").delegato += OnCreateComment;
        eventManager.events.SearchOrCreate<EventParam<CommentData>>("leavecomment").delegato += OnLeaveComment;

        eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato += InternalPopUp;
    }

    protected virtual void PopUp(CommentView commentView)
    {
        comment = commentView.commentData;

        user = comment.user;

        callsManager.DestroyAll();

        foreach (Transform item in placeToCreate)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in commentView.commentData.user.comments)
        {
            CreateView(item.Value);
        }

        onActive.Invoke();

        userName.text = $"Usuario: {comment.textName}";

        textToShow.text = "Posibles acciones:";
    }

    public void Execute()
    {
        onExecute.Invoke();
    }

    void InternalPopUp(CommentView commentView)
    {
        if (ExecutePopUp)
            PopUp(commentView);
    }

    void OnLeaveComment(CommentData commentData)
    {
        if (isActiveAndEnabled && user == commentData.user && !user.Enable && user.comments.Count == 0)
        {
            callsManager.DestroyAll();
            textToShow.text = "Se retiro el usuario y no quedan comentarios\n" + "No hay posibles acciones".RichTextColor(commentDestroy);
        }
    }

    void OnCreateComment(CommentData commentData)
    {
        if (isActiveAndEnabled && user == commentData.user)
            CreateView(commentData);
    }

    void CreateView(CommentData commentData)
    {
        var aux = Instantiate(commentViewPrefab, placeToCreate);

        aux.button.interactable = false;

        aux.commentData = commentData;
    }

}

