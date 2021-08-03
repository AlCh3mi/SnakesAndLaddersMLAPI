using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Block : MonoBehaviour
{
    public int Id;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text numberText;
    
    public UnityEvent<int> IfLandedOnMoveTo;
    
    public void Setup(int Id, Color color)
    {
        SetID(Id);
        SetColour(color);
        numberText.text = Id.ToString();
    }

    private void SetColour(Color color) => spriteRenderer.color = color;
    
    private void SetID(int id) => Id = id;
}
