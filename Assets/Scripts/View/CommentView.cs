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
    Image perfil;

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
            _commentData = value;

            if (_commentData == null)
                return;

            perfil.sprite = _commentData.perfil;

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
        _commentData = null;
        this.SetActiveGameObject(false);
        Parent.Return(this);
    }

    public void OnClick()
    {
        _commentData.OnClick();
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



