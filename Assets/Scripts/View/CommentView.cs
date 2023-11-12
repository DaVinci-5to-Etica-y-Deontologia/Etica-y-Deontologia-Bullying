using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommentView : MonoBehaviour, IPoolElement<CommentView>
{
    [SerializeField]
    TextMeshProUGUI textMesh;

    [SerializeField]
    Image cuerpo;

    [SerializeField]
    Image cabeza;

    [SerializeField]
    Image ojos;

    [SerializeField]
    Image boquitas;

    [SerializeField]
    Image accesorios;

    [SerializeField]
    Button button;

    [SerializeField]
    CommentData _commentData;

    public LinkedPool<CommentView> Parent { get; set; }

    public IPoolElement<CommentView> Next { get; set; }

    public bool inPool { get; set; }

    public CommentData commentData
    {
        get => _commentData;

        set
        {
            if(_commentData!=null)
                _commentData.onDestroy -= Destroy;

            if (value == null)
                return;

            _commentData = value;

            _commentData.user.SetCuerpo(cuerpo);

            _commentData.user.SetAccesorio(accesorios);

            _commentData.user.SetBoquita(boquitas);

            _commentData.user.SetCabeza(cabeza);

            _commentData.user.SetOjos(ojos);

            textMesh.text = _commentData.textComment;

            _commentData.onDestroy += Destroy;
        }
    }

    public CommentView Create()
    {
        var aux = StreamerManager.SpawnComment();
        aux.SetActiveGameObject(false);
        return aux;
    }

    public void Destroy()
    {
        commentData = null;
        this.SetActiveGameObject(false);
        Parent.Return(this);
    }

    public void OnClick()
    {
        commentData.eventManager.events.SearchOrCreate<EventParam<CommentView>>("onclickcomment").delegato.Invoke(this);
    }


    public void AnimRefresh()
    {
    }


    private void OnDestroy()
    {
        if(_commentData!=null)
            _commentData.onDestroy -= Destroy;
    }
}



