using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommentView : MonoBehaviour, IPoolElement<CommentView>
{
    [SerializeField]
    public Button button;

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
    Image BackGround;

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
            {
                _commentData.onDestroy -= Destroy;

                if(_commentData.user!=null)
                {
                    _commentData.user.onSuspectChange -= User_onSuspectChange;

                    _commentData.user.onMoralIndexChange -= User_onMoralIndexChange;
                }
            }
                

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

            _commentData.user.onSuspectChange += User_onSuspectChange;

            _commentData.user.onMoralIndexChange += User_onMoralIndexChange;

            User_onMoralIndexChange(_commentData.user.MoralIndex);

            User_onSuspectChange(_commentData.user.Suspect);
        }
    }

    private void User_onMoralIndexChange(float obj)
    {
        if (commentData.player.Moderator)
            return;

        obj *= 2;

        if (obj >= 1)
            BackGround.color = Color.Lerp(Color.yellow, Color.green, obj-1);
        else
            BackGround.color = Color.Lerp(Color.red, Color.yellow, obj);
    }

    private void User_onSuspectChange(int obj)
    {
        if (!commentData.player.Moderator)
            return;

        switch (obj)
        {
            case 0:
                BackGround.color = new Color(0,0,0,0.1f);
                break;

            case 1:
                BackGround.color = Color.green;
                break;

            case 2:
                BackGround.color = Color.yellow;
                break;

            case 3:
                BackGround.color = Color.red;
                break;
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
        Parent?.Return(this);
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
        if(commentData != null)
        {
            commentData = null;
        }
    }
}



