using UnityEngine;
using TMPro;

public class BubbleAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator bubbleAnimator;
    [SerializeField] private TextMeshProUGUI bubbleText;
    [SerializeField] private RectTransform bubbleImage;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Bubble aktif, kelime aktif ve animasyonu aktif.
    /// </summary>
    /// <param name="word">Bubble içerisine yazilacak kelime</param>
    public void PlayBubbleAnimation(string word)
    {
        gameObject.SetActive(true);

        if (bubbleText != null)
        {
            bubbleText.text = word;
        }

        AdjustBubbleSize(word);

        if (bubbleAnimator != null)
        {
            bubbleAnimator.SetTrigger("Play");
        }
    }

    /// <summary>
    /// Bubble'in genişligi.
    /// </summary>
    /// <param name="word">Bubble içerisine yazılan kelime</param>
    private void AdjustBubbleSize(string word)
    {
        if (bubbleImage != null)
        {
            int wordLength = word.Length;

            if (wordLength > 2)
            {
                float additionalWidth = (wordLength - 2) * 100f;
                bubbleImage.sizeDelta = new Vector2(550f + additionalWidth, bubbleImage.sizeDelta.y);
            }
            else
            {
                bubbleImage.sizeDelta = new Vector2(550f, bubbleImage.sizeDelta.y);
            }
        }
    }

    /// <summary>
    /// Animasyonun sonu function.
    /// </summary>
    public void OnAnimationEnd()
    {
        gameObject.SetActive(false);
    }
    public void ResetAnimation()
    {
        if (bubbleAnimator != null)
        {
            bubbleAnimator.ResetTrigger("Play"); // Trigger'ı sıfırla
            bubbleAnimator.Play("Idle", 0, 0f); // İlk state'e geri dön
        }

        gameObject.SetActive(false); // Bubble'ı kapat
    }
}

