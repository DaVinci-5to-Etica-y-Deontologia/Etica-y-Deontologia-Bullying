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

    protected abstract bool ExecutePopUp { get; }

    public override void MyAwake(PopUpManager popUpManager)
    {
        base.MyAwake(popUpManager);
        eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato += InternalPopUp;
        eventManager.events.SearchOrCreate<EventParam<CommentData>>("leavecomment").delegato += OnLeaveComment;
    }

    protected virtual void PopUp(CommentView commentView)
    {
        comment = commentView.commentData;

        callsManager.DestroyAll();

        foreach (Transform item in placeToCreate)
        {
            Destroy(item.gameObject);
        }

        var aux = Instantiate(commentView, placeToCreate.position, Quaternion.identity, placeToCreate);

        aux.button.interactable = false;

        onActive.Invoke();

        userName.text = $"Usuario: {comment.textName}";

        textToShow.text = "Posibles acciones:";
    }

    protected void Execute()
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
        if (comment != null && comment == commentData)
        {
            callsManager.DestroyAll();
            textToShow.text = "Comentario eliminado/viejo\n" + "No hay posibles acciones".RichTextColor(commentDestroy);
        }
    }
}

