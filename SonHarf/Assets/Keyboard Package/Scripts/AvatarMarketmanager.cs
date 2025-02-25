using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static LeanTween;

public class AvatarMarketManager : MonoBehaviour
{
    [Header("Swipe Ayarları")]
    public RectTransform contentRectTransform; // Content objesinin RectTransform'u
    public RectTransform swipeArea; // Swipe işleminin geçerli olduğu alan
    public RectTransform scrollBarTransform;

    [Header("Market Ayarları")]
    public Image marketAvatarImage; // Marketteki avatar Image kutusu

    [Header("Avatar Görüntülenecek Yerler")]
    public Image[] targetAvatarImages;

    [Header("Avatar Butonları")]
    public AvatarButton[] allAvatarButtons; // Inspector'dan tüm butonları sürükle bırak

    

    public static AvatarMarketManager Instance;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private float swipeThreshold = 50f; // Swipe algılama eşiği
    private float smoothTime = 0.1f; // Geçiş süresi
    private Vector2 targetPosition; // Hedef pozisyon
    private Vector2 velocity = Vector2.zero; // SmoothDamp için hız değişkeni

    private int currentIndex = 0; // Şu anki Image indeksi (0'dan başlar)
    private float imageWidth = 676f; // Her bir Image'ın genişliği (posX değeri)
    private int maxIndex = 2; // Maksimum indeks
    private const string SelectedAvatarKey = "SelectedAvatarID";

    // Seçili olan butonu tutacak referans (varsayılan boyut 200x200)
    private RectTransform lastSelectedButton;
    private Vector2 defaultButtonSize = new Vector2(200f, 200f);
    private Vector2 selectedButtonSize = new Vector2(235.2f, 238.2f);

    private Vector2 scrollBarTargetPosition; // Scroll bar'ın hedef pozisyonu
    private Vector2 scrollBarVelocity = Vector2.zero;
    private float scrollBarMinX = -6776f; // Scroll bar'ın minimum X değeri
    private float scrollBarMaxX = 6776f;  // Scroll bar'ın maksimum X değeri

    private void Start()
    {
        // Başlangıçta Content'in pozisyonunu sıfırla
        targetPosition = contentRectTransform.anchoredPosition;
        if (scrollBarTransform != null)
        {
            // Scroll bar başlangıç pozisyonunu -6776 olarak ayarla
            scrollBarTargetPosition = new Vector2(scrollBarMinX, scrollBarTransform.anchoredPosition.y);
            scrollBarTransform.anchoredPosition = scrollBarTargetPosition;
        }
         LoadSelectedAvatar();
    }
    private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
    }
    else
    {
        Destroy(gameObject);
    }
}

    private void Update()
    {
        // Smooth bir şekilde Content'in pozisyonunu güncelle
        contentRectTransform.anchoredPosition = Vector2.SmoothDamp(
            contentRectTransform.anchoredPosition,
            targetPosition,
            ref velocity,
            smoothTime
        );
        if (scrollBarTransform != null)
        {
            scrollBarTransform.anchoredPosition = Vector2.SmoothDamp(
                scrollBarTransform.anchoredPosition,
                scrollBarTargetPosition,
                ref scrollBarVelocity,
                smoothTime
            );
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startTouchPosition = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        endTouchPosition = eventData.position;
        float swipeDistance = endTouchPosition.x - startTouchPosition.x;

        if (Mathf.Abs(swipeDistance) > swipeThreshold)
        {
            if (swipeDistance > 0)
                MoveToPreviousImage();
            else
                MoveToNextImage();
        }
    }

    private void MoveToNextImage()
    {
        if (currentIndex < maxIndex)
        {
            currentIndex++;
            targetPosition = new Vector2(-imageWidth * currentIndex, contentRectTransform.anchoredPosition.y);
            UpdateScrollBarTarget();
        }
    }

    private void MoveToPreviousImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            targetPosition = new Vector2(-imageWidth * currentIndex, contentRectTransform.anchoredPosition.y);
            UpdateScrollBarTarget();
        }
    }

    private void UpdateScrollBarTarget()
    {
        if (scrollBarTransform != null)
        {
            // currentIndex, maxIndex aralığında 0-1 oranına çevrilir.
            float t = (float)currentIndex / maxIndex;
            // Bu oran, scroll bar'ın -6776 ile +6776 arasında Lerp edilmesini sağlar.
            float targetX = Mathf.Lerp(scrollBarMinX, scrollBarMaxX, t);
            scrollBarTargetPosition = new Vector2(targetX, scrollBarTransform.anchoredPosition.y);
        }
    }

    // Marketteki avatarı güncelleyen metot
    public void SetMarketAvatar(Sprite newAvatarSprite)
{
    // Market görselini güncelle
    if(marketAvatarImage != null)
        marketAvatarImage.sprite = newAvatarSprite;

    // Tüm hedef image'lara atama yap
    foreach(Image img in targetAvatarImages)
    {
        if(img != null)
            img.sprite = newAvatarSprite;
        else
            Debug.LogWarning("Hedef image'lardan biri boş!");
    }
}
    public void OnAvatarButtonClicked(RectTransform clickedButton)
{
    // Eğer aynı butona tekrar tıklandıysa hiçbir şey yapma
    if (lastSelectedButton == clickedButton)
        return;

    // Önceki butonu küçült (animasyonlu)
    if (lastSelectedButton != null)
    {
        LeanTween.size(lastSelectedButton, defaultButtonSize, 0.3f)
            .setEase(LeanTweenType.easeOutBack);
    }

    // Yeni butonu büyüt (animasyonlu)
    LeanTween.size(clickedButton, selectedButtonSize, 0.3f)
        .setEase(LeanTweenType.easeOutBack);

    // Seçili buton referansını güncelle
    lastSelectedButton = clickedButton;
}
public void SelectAvatar(int id, Sprite sprite, RectTransform buttonTransform, JokerType jokerType, float jokerDuration)
{
    OnAvatarButtonClicked(buttonTransform);
    SetMarketAvatar(sprite);
    
    // ID, Joker Tipi ve Süresini Kaydet
    PlayerPrefs.SetInt("SelectedAvatarID", id);
    PlayerPrefs.SetInt("SelectedJokerType", (int)jokerType); // Joker tipini int olarak kaydet
    PlayerPrefs.SetFloat("SelectedJokerDuration", jokerDuration); // Süreyi kaydet
    PlayerPrefs.Save();

    // JokerManager'a bildir
    JokerManager.Instance.SetCurrentJoker(jokerType, jokerDuration);
}

private void LoadSelectedAvatar()
{
    int savedID = PlayerPrefs.GetInt("SelectedAvatarID", -1);
    int savedJokerType = PlayerPrefs.GetInt("SelectedJokerType", 0); // Varsayılan: None
    float savedJokerDuration = PlayerPrefs.GetFloat("SelectedJokerDuration", 0f);

    if (savedID != -1)
    {
        foreach (Transform page in contentRectTransform)
        {
            foreach (Transform button in page)
            {
                AvatarButton btn = button.GetComponent<AvatarButton>();
                if (btn != null && btn.buttonID == savedID)
                {
                    OnAvatarButtonClicked(button.GetComponent<RectTransform>());
                    SetMarketAvatar(btn.avatarSprite);
                    
                    // Kaydedilen Joker bilgilerini yükle
                    JokerManager.Instance.SetCurrentJoker((JokerType)savedJokerType, savedJokerDuration);
                    break;
                }
            }
        }
    }
}

}