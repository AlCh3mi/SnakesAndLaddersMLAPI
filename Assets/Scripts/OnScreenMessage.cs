using TMPro;
using UnityEngine;

public class OnScreenMessage : MonoBehaviour
{
    [SerializeField] private float Duration;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text subText;

    private bool isInitialized = false;
    private bool fadeAway = true;

    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if(isInitialized && fadeAway)
            canvasGroup.alpha -= Time.deltaTime / Duration;
    }

    public void Display(string headerText, string subText = "", float displayDuration = 5f, bool fadeAway = true)
    {
        this.headerText.text = headerText;
        this.subText.text = subText;
        this.fadeAway = fadeAway;
        Duration = displayDuration;
        isInitialized = true;
        Destroy(gameObject, displayDuration > 0 ? displayDuration : default);
    }
}