using UnityEngine;
using UnityEngine.UI;

public class NicknameItem : MonoBehaviour
{
    private const float Y_OFFSET = 2.5f;
    
    private Transform _owner;

    private Text _nameText;

    public void SetOwner(Transform owner)
    {
        _owner = owner;

        _nameText = GetComponent<Text>();
    }
    
    public void UpdateNickname(string newNick)
    {
        _nameText.text = newNick;
    }
    
    public void UpdatePosition()
    {
        transform.position = _owner.position + Vector3.up * Y_OFFSET;
    }
}
