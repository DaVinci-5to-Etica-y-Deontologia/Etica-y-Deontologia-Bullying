using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NicoNicoComment : MonoBehaviour
{
    public TextMeshProUGUI commentText;
    private float speed;
    private int screenWidth;

    void Start()
    {
        commentText = GetComponent<TextMeshProUGUI>();
        screenWidth = Screen.width;
    }
    void Update()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition += new Vector2(-speed * Time.deltaTime, 0);

        if (rectTransform.anchoredPosition.x < -screenWidth * 1.5f)
        {
            Destroy(gameObject);
        }
    }

    public void SetUniqueCharacteristics(float speeds, Color color, int size){
        commentText.color = color;
        speed = speeds;
        commentText.fontSize = size;
    }
}
