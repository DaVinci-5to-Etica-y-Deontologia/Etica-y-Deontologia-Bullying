using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NicoNicoCommentManager : MonoBehaviour
{
    private delegate void doOnUpdate();
    private doOnUpdate spawnText = delegate {};
    public NicoNicoComment commentPrefab;
    public RectTransform commentParent;
    public float minSpeed, maxSpeed;
    public int minSize, maxSize;
    public float minTimeBetweenComment, maxTimeBetweenComment;
    public Color[] colors;
    public string[] comments;
    public void CreateComment(string text)
    {
        NicoNicoComment newComment = Instantiate(commentPrefab, commentParent);
        newComment.transform.position = new Vector3(Screen.width * 1.5f, Random.Range(0, Screen.height), 1);
        newComment.commentText.text = text;
        var a = colors[Random.Range(0, colors.Length)];
        newComment.SetUniqueCharacteristics(Random.Range(minSpeed, maxSpeed), a, Random.Range(minSize, maxSize));
    }

    private IEnumerator spawner(){
        spawnText = delegate{};
        CreateComment(comments[Random.Range(0, comments.Length)]);
        yield return new WaitForSeconds(Random.Range(minTimeBetweenComment, maxTimeBetweenComment));
        spawnText = delegate { StartCoroutine(spawner());};
    }
    void Start(){
        spawnText = delegate { StartCoroutine(spawner());};
        for(int i = 0; i> 60; i++){
            CreateComment(comments[Random.Range(0, comments.Length)]);
        }
    }
    void Update(){
        spawnText();
    }
}
