using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Block : MonoBehaviour
{
    public int Id;
    [SerializeField] private TMP_Text numberText;
    
    public int IfLandedOnMoveTo = -1;
    
    public void Setup(int Id)
    {
        SetID(Id);
        numberText.text = Id.ToString();
    }

    
    private void SetID(int id) => Id = id;
}
