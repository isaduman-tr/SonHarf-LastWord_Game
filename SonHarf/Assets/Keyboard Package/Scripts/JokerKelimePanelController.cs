using UnityEngine;

public class JokerKelimePanelController : MonoBehaviour
{
    public static JokerKelimePanelController Instance;

    [Header("Panel Ayarları")]
    public RectTransform panelRectTransform; // Panelin RectTransform'u
    public float hidePosY = 580f; // Panelin başlangıç/yığılan pozisyonu
    public float showPosY = 778f; // Panelin açılınca ulaşacağı posY
    public float duration = 0.3f; // Animasyon süresi

    private void Awake()
    {
        // Singleton yapısı
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Paneli aç: posY'yi 580'den 778'e taşı
    public void ShowPanel()
    {
        if (panelRectTransform != null)
        {
            LeanTween.moveY(panelRectTransform, showPosY, duration).setEase(LeanTweenType.easeOutBack);
        }
    }

    // Paneli kapat: posY'yi 778'den 580'e geri taşı
    public void HidePanel()
    {
        if (panelRectTransform != null)
        {
            LeanTween.moveY(panelRectTransform, hidePosY, duration).setEase(LeanTweenType.easeOutBack);
        }
    }
}
