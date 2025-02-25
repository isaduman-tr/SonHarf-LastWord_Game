using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarBuyingManager : MonoBehaviour
{
    public static AvatarBuyingManager Instance;

    [System.Serializable]
    public class AvatarData
    {
        public int buttonID;                   // 0'dan 26'ya kadar benzersiz ID
        public JokerType jokerType;            // Avatarın enum değeri (örneğin, DoubleScore, FreezeTime, vb.)
        public Button avatarSelectButton;      // Avatarı seçmek için kullanılan buton (sadece fiyat bilgisini gösterir)
        public TMP_Text buttonPriceText;       // Avatar UI'sinde gösterilecek fiyat ya da "Owned" yazısı
        public Sprite avatarSprite;  // İlgili avatarın görseli
        public float jokerDuration = 20f;  // Varsayılan joker süresi

    }

    // Inspector'de 27 elemanlık olarak atayabileceğiniz avatar verileri
    public AvatarData[] allAvatars;
    
    // Seçili avatarın fiyat bilgisinin gösterileceği global UI text
    public Button globalPurchaseButton;
    // Global satın alma butonu üzerindeki text (varsa)
    public TMP_Text globalPurchaseButtonText;

    // Şu anda seçili olan avatar
    private AvatarData selectedAvatar;

    public TMP_Text avatarFeatureText;
    


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeAvatars();
        globalPurchaseButton.onClick.AddListener(OnGlobalPurchaseButtonClicked);
        LoadSelectedAvatar();
    }

    /// <summary>
    /// Tüm avatarlar için, fiyat hesaplanır ve UI öğeleri (buttonPriceText) güncellenir.
    /// Her avatarın seçim butonuna tıklama eventi eklenir.
    /// </summary>
    private void InitializeAvatars()
    {
        foreach (var avatar in allAvatars)
        {
            int price = GetPriceFromJokerType(avatar.jokerType);
            bool purchased = PlayerPrefs.GetInt("Avatar_" + avatar.buttonID, 0) == 1;

            ColorBlock colors = avatar.avatarSelectButton.colors;

            if (purchased)
            {
                avatar.buttonPriceText.text = "Owned";
                // Satın alınmışsa butonun normal rengi beyaz (FFFFFF) olacak.
                colors.normalColor = Color.white;
            }
            else
            {
                avatar.buttonPriceText.text = price.ToString();
                // Satın alınmamışsa butonun normal rengi 353535 olacak.
                colors.normalColor = new Color32(0x35, 0x35, 0x35, 255);
            }
            avatar.avatarSelectButton.colors = colors;

            // Listener eklemeden önce eski listener'ları temizleyelim.
            avatar.avatarSelectButton.onClick.RemoveAllListeners();
            AvatarData currentAvatar = avatar; // Closure sorunu olmaması için yerel kopya.
            avatar.avatarSelectButton.onClick.AddListener(() => OnAvatarSelectButtonClicked(currentAvatar));
        }
    }

    /// <summary>
    /// JokerType değerine göre avatarın fiyatını belirler.
    /// Fiyatlandırma mantığını ihtiyacınıza göre değiştirebilirsiniz.
    /// </summary>
    private int GetPriceFromJokerType(JokerType type)
    {
        switch (type)
        {
            case JokerType.None: return 0;
            case JokerType.DoubleScore: return 0;
            case JokerType.DoubleScore2: return 150;
            case JokerType.DoubleScore3: return 200;
            case JokerType.FreezeTime: return 120;
            case JokerType.FreezeTime2: return 170;
            case JokerType.FreezeTime3: return 220;
            case JokerType.RevealWord: return 130;
            case JokerType.RevealWord2: return 180;
            case JokerType.RevealWord3: return 230;
            default: return 0;
        }
    }

    /// <summary>
    /// Avatar seçim butonuna tıklandığında, global UI (seçili avatar fiyat display ve global satın alma butonu) güncellenir.
    /// </summary>
    private void OnAvatarSelectButtonClicked(AvatarData avatar)
    {
        selectedAvatar = avatar;
        int price = GetPriceFromJokerType(avatar.jokerType);
        bool purchased = PlayerPrefs.GetInt("Avatar_" + avatar.buttonID, 0) == 1;
        
        

        if (avatarFeatureText != null)
    {
        switch (avatar.jokerType)
        {
            case JokerType.DoubleScore:
                avatarFeatureText.text = "Puanınızı tek tur için 8 katına çıkararak oyunda size büyük avantaj sağlayan bu avatar, her hamlenizde fark yaratır.";
                break;
            case JokerType.DoubleScore2:
                avatarFeatureText.text = "Puanınızı tek tur için 10 katına çıkaran bu avatar, hamlelerinize ekstra güç katarak sizi oyunun lideri konumuna taşır.";
                break;
            case JokerType.DoubleScore3:
                avatarFeatureText.text = "Puanınızı tek tur için 15 katına çıkaran bu avatar, yüksek skorlar elde etmeniz için mükemmel bir destek sunar.";
                break;
            case JokerType.FreezeTime:
                avatarFeatureText.text = "Tur sürenize tek seferlik 20 saniye ekleyerek size ekstra düşünme zamanı kazandıran bu avatar, stratejinizin anahtarıdır.";
                break;
            case JokerType.FreezeTime2:
                avatarFeatureText.text = "Tur sürenizi 25 saniye uzatan bu avatar, riskleri minimize ederken daha rahat kararlar almanızı sağlar.";
                break;
            case JokerType.FreezeTime3:
                avatarFeatureText.text = "Tur Sürenizi 30 saniye artıran bu avatar, size plan yapma ve hamlelerinizi optimize etme fırsatı sunar.";
                break;
            case JokerType.RevealWord:
                avatarFeatureText.text = "Zorlandığınız anlarda 1 kelime jokeri sunarak sizi kurtaran bu avatar, kritik hamlelerinizde yanınızda.";
                break;
            case JokerType.RevealWord2:
                avatarFeatureText.text = "2 kelime jokeri ile hatalarınızı telafi etmenizi kolaylaştıran bu avatar, stratejinize ekstra destek katar.";
                break;
            case JokerType.RevealWord3:
                avatarFeatureText.text = "3 kelime jokeri sağlayarak maksimum esneklik sunan bu avatar, en zorlu durumlarda bile sizi güçlü kılar.";
                break;
            default:
                avatarFeatureText.text = "";
                break;
        }
    }
        if (purchased)
    {
        globalPurchaseButton.interactable = false;
        if(globalPurchaseButtonText != null)
            globalPurchaseButtonText.text = "Owned";

        // Eğer avatar satın alınmışsa, direkt olarak avatarı seç
        AvatarMarketManager.Instance.SelectAvatar(
            avatar.buttonID,
            avatar.avatarSprite,
            avatar.avatarSelectButton.GetComponent<RectTransform>(),
            avatar.jokerType,
            avatar.jokerDuration
        );
    }

        else
        {
            globalPurchaseButton.interactable = true;
            if(globalPurchaseButtonText != null)
                globalPurchaseButtonText.text = price.ToString();
        }

        Debug.Log("Avatar seçildi: ID " + avatar.buttonID + ", Fiyat: " + price);
    }

    /// <summary>
    /// Global satın alma butonuna tıklandığında, seçili avatar için satın alma işlemi gerçekleştirilir.
    /// Elmas miktarı kontrol edilir; yeterliyse avatar satın alınır, UI güncellenir ve kayıt yapılır.
    /// </summary>
    private void OnGlobalPurchaseButtonClicked()
    {
        if(selectedAvatar == null)
        {
            Debug.Log("Avatar seçilmedi.");
            return;
        }
        
        int price = GetPriceFromJokerType(selectedAvatar.jokerType);
        int currentDiamonds = ElmasManager.Instance.GetCurrentElmas();
        bool purchased = PlayerPrefs.GetInt("Avatar_" + selectedAvatar.buttonID, 0) == 1;

        if (purchased)
        {
            Debug.Log("Bu avatar zaten satın alınmış.");
            return;
        }

        if (currentDiamonds >= price)
    {
        // Yeterli elmas varsa satın alma gerçekleşir.
        ElmasManager.Instance.SpendElmas(price);

        // Satın alma bilgisini kaydet
        PlayerPrefs.SetInt("Avatar_" + selectedAvatar.buttonID, 1);
        PlayerPrefs.Save();

        // UI güncellemeleri
        selectedAvatar.buttonPriceText.text = "Owned";
        globalPurchaseButton.interactable = false;
        if(globalPurchaseButtonText != null)
            globalPurchaseButtonText.text = "Owned";
        


        // Buton rengini beyaz yap
        ColorBlock colors = selectedAvatar.avatarSelectButton.colors;
        colors.normalColor = Color.white;
        selectedAvatar.avatarSelectButton.colors = colors;

        Debug.Log("Avatar satın alındı: ID " + selectedAvatar.buttonID + ", Fiyat: " + price);

        // Satın alma tamamlandığında avatarı otomatik seç
        AvatarMarketManager.Instance.SelectAvatar(
            selectedAvatar.buttonID,
            selectedAvatar.avatarSprite,
            selectedAvatar.avatarSelectButton.GetComponent<RectTransform>(),
            selectedAvatar.jokerType,
            selectedAvatar.jokerDuration
        );
    }
        else
        {
            Debug.Log("Yeterli elmas yok! Gerekli: " + price + ", Elmasınız: " + currentDiamonds);
        }
    }

    private void LoadSelectedAvatar()
{
    int savedID = PlayerPrefs.GetInt("SelectedAvatarID", -1);
    if (savedID != -1)
    {
        foreach (var avatar in allAvatars)
        {
            if (avatar.buttonID == savedID)
            {
                OnAvatarSelectButtonClicked(avatar);
                break;
            }
        }
    }
}
}



