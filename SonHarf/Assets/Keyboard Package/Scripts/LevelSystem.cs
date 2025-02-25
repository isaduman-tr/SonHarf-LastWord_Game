using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI xpText; // XP değerini gösterecek Text
    [SerializeField] private TextMeshProUGUI xpText2; // İkinci XP Text'i
    [SerializeField] private TextMeshProUGUI xpText3; // Üçüncü XP Text'i
    [SerializeField] private TextMeshProUGUI xpText4;

    [SerializeField] private TextMeshProUGUI levelText; // Level değerini gösterecek Text
    [SerializeField] private TextMeshProUGUI levelText2; // İkinci Level Text'i
    [SerializeField] private TextMeshProUGUI levelText3; // Üçüncü Level Text'i
    [SerializeField] private TextMeshProUGUI levelText4;

    [SerializeField] private Slider levelBar; // Level bar (Slider)
    [SerializeField] private Slider levelBar2; // İkinci Level Bar
    [SerializeField] private Slider levelBar3; // Üçüncü Level Bar
    [SerializeField] private Slider levelBar4;

    [SerializeField] private TMP_Text nextLevelRewardText; // Inspector'a atayın
    [SerializeField] private TMP_Text nextLevelRewardText2;
    [SerializeField] private RectTransform targetDiamondImage;

    [SerializeField] private GameObject levelUpAnimObj1;
    [SerializeField] private GameObject levelUpAnimObj2;

    [Header("Elmas Ayarları")]
    [SerializeField] private int[] levelElmasMiktarlari = new int[25]; // Inspector'da 25 değer dolduracağız
    

    private int currentXP = 0; // Mevcut XP
    private int totalXP = 0; // Toplam XP (XPText'te gösterilecek)
    private int currentLevel = 1; // Mevcut Level
    private const int baseXPToNextLevel = 100; // Bir sonraki level için gereken temel XP

    private const string XPKey = "PlayerXP"; // Kayıt için anahtar
    private const string LevelKey = "PlayerLevel"; // Kayıt için anahtar
    private const string TotalXPKey = "TotalXP"; // Toplam XP için anahtar
    private const string NextDiamondRewardKey = "NextDiamondReward";

    public delegate void OnLevelUpDelegate(int newLevel);
    public static event OnLevelUpDelegate OnLevelUp;

    private void Start()
    {
        // Kayıtlı XP, Level ve Toplam XP değerlerini yükle
        LoadXP();
        UpdateUI();
    }

    // XP ekleme fonksiyonu
    public void AddXP(int xp)
    {
        currentXP += xp;
        totalXP += xp; // Toplam XP'yi güncelle

        // Level atlama kontrolü
        while (currentXP >= baseXPToNextLevel)
        {
            currentXP -= baseXPToNextLevel;
            LevelUp();
        }

        // UI'ı güncelle ve XP'yi kaydet
        UpdateUI();
        SaveXP();
    }

    // Level atlama fonksiyonu
    private void LevelUp()
    {
        currentLevel++;
        Debug.Log("Level Up! Current Level: " + currentLevel);
        
        int levelIndex = (currentLevel - 2) % levelElmasMiktarlari.Length;
        int kazanilanElmas = levelElmasMiktarlari[levelIndex];

        ElmasManager.Instance.AddElmas(kazanilanElmas);
        
        Debug.Log($"{currentLevel}. level! {kazanilanElmas} elmas kazandınız! (Index: {levelIndex})");

        OnLevelUp?.Invoke(currentLevel);
        AnimateTargetDiamondImage();
        ActivateLevelUpAnimations();

        // Tüm Level Bar'ları sıfırla ve animasyonla güncelle
        UpdateLevelBars();
        UpdateNextLevelRewardPref();
    }
    [ContextMenu("Varsayılan Değerleri Doldur")]

    private void UpdateNextLevelRewardText()
{
    if (nextLevelRewardText != null)
    {
        // currentLevel seviyesindeyken, bu leveli geçince kazanılacak elmas miktarı:
        int diamondReward = levelElmasMiktarlari[(currentLevel - 1) % levelElmasMiktarlari.Length];
        // Mesajı istediğiniz formatta ayarlayın:
        nextLevelRewardText.text = diamondReward.ToString();
        PlayerPrefs.SetInt(NextDiamondRewardKey, diamondReward);
        PlayerPrefs.Save();
    }

    if (nextLevelRewardText2 != null)
        {
            int diamondReward = levelElmasMiktarlari[(currentLevel - 1) % levelElmasMiktarlari.Length];
            LeanTween.value(nextLevelRewardText2.gameObject, 0, diamondReward, 1f)
                .setOnUpdate((float val) =>
                {
                    nextLevelRewardText2.text = Mathf.RoundToInt(val).ToString();
                });
        }
}

private void ActivateLevelUpAnimations()
{
    // Objeleri aktif yap
    if (levelUpAnimObj1 != null)
        levelUpAnimObj1.SetActive(true);
    if (levelUpAnimObj2 != null)
        levelUpAnimObj2.SetActive(true);

    // levelUpAnimObj1'i 1 saniye sonra pasif yap
    if (levelUpAnimObj1 != null)
    {
        LeanTween.delayedCall(levelUpAnimObj1, 1f, () =>
        {
            levelUpAnimObj1.SetActive(false);
        });
    }

    // levelUpAnimObj2'yi 2.5 saniye sonra pasif yap
    if (levelUpAnimObj2 != null)
    {
        LeanTween.delayedCall(levelUpAnimObj2, 2.5f, () =>
        {
            levelUpAnimObj2.SetActive(false);
        });
    }
}

private void AnimateTargetDiamondImage()
    {
        if (targetDiamondImage != null)
        {
            // 0.5 saniyede Y ekseninde 90°'ye dön
            LeanTween.rotateY(targetDiamondImage.gameObject, 90f, 0.5f)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() =>
                {
                    // 1 saniye bekle, sonra tekrar 0°'ye döndür
                    LeanTween.delayedCall(targetDiamondImage.gameObject, 1f, () =>
                    {
                        LeanTween.rotateY(targetDiamondImage.gameObject, 0f, 0.5f)
                            .setEase(LeanTweenType.easeInOutQuad);
                    });
                });
        }
    }


    

    // UI'ı güncelle
    private void UpdateUI()
    {
        // XPText'leri güncelle (Toplam XP'yi göster)
        UpdateXPText(xpText);
        UpdateXPText(xpText2);
        UpdateXPText(xpText3);
        UpdateXPText(xpText4);
        



        // LevelText'leri güncelle
        UpdateLevelText(levelText);
        UpdateLevelText(levelText2);
        UpdateLevelText(levelText3);
        UpdateLevelText(levelText4);

        // LevelBar'ları güncelle
        UpdateLevelBar(levelBar);
        UpdateLevelBar(levelBar2);
        UpdateLevelBar(levelBar3);
        UpdateLevelBar(levelBar4);

        UpdateNextLevelRewardText();
    }

    // XP Text'ini güncelle
    private void UpdateXPText(TextMeshProUGUI xpText)
    {
        if (xpText != null)
        {
            int displayedXP = int.Parse(xpText.text.Replace("XP: ", ""));
            LeanTween.value(displayedXP, totalXP, 0.5f).setOnUpdate((float val) =>
            {
                xpText.text = Mathf.RoundToInt(val).ToString();
            });
        }
    }

    // Level Text'ini güncelle
    private void UpdateLevelText(TextMeshProUGUI levelText)
    {
        if (levelText != null)
            levelText.text = currentLevel.ToString();
    }

    // Level Bar'ını güncelle
    private void UpdateLevelBar(Slider levelBar)
    {
        if (levelBar != null)
        {
            levelBar.maxValue = baseXPToNextLevel;
            float startValue = (currentXP == 0) ? 0 : levelBar.value;
            LeanTween.value(levelBar.gameObject, startValue, currentXP, 0.5f).setOnUpdate((float val) =>
            {
                levelBar.value = val;
            });
        }
    }

    // Level Bar'ları sıfırla ve animasyonla güncelle
    private void UpdateLevelBars()
    {
        if (levelBar != null)
        {
            LeanTween.value(levelBar.gameObject, levelBar.value, levelBar.maxValue, 0.3f)
                .setOnUpdate((float val) => { levelBar.value = val; })
                .setOnComplete(() =>
                {
                    levelBar.value = 0;
                    UpdateUI();
                });
        }

        if (levelBar2 != null)
        {
            LeanTween.value(levelBar2.gameObject, levelBar2.value, levelBar2.maxValue, 0.3f)
                .setOnUpdate((float val) => { levelBar2.value = val; })
                .setOnComplete(() =>
                {
                    levelBar2.value = 0;
                    UpdateUI();
                });
        }

        if (levelBar3 != null)
        {
            LeanTween.value(levelBar3.gameObject, levelBar3.value, levelBar3.maxValue, 0.3f)
                .setOnUpdate((float val) => { levelBar3.value = val; })
                .setOnComplete(() =>
                {
                    levelBar3.value = 0;
                    UpdateUI();
                });
        }

        if (levelBar4 != null)
        {
            LeanTween.value(levelBar4.gameObject, levelBar4.value, levelBar4.maxValue, 0.3f)
                .setOnUpdate((float val) => { levelBar4.value = val; })
                .setOnComplete(() =>
                {
                    levelBar4.value = 0;
                    UpdateUI();
                });
        }
    }

    // XP, Level ve Toplam XP'yi kaydet
    private void SaveXP()
    {
        PlayerPrefs.SetInt(XPKey, currentXP);
        PlayerPrefs.SetInt(LevelKey, currentLevel);
        PlayerPrefs.SetInt(TotalXPKey, totalXP);
        PlayerPrefs.Save();
    }

    // XP, Level ve Toplam XP'yi yükle
    private void LoadXP()
    {
        currentXP = PlayerPrefs.GetInt(XPKey, 0);
        currentLevel = PlayerPrefs.GetInt(LevelKey, 1);
        totalXP = PlayerPrefs.GetInt(TotalXPKey, 0);
        UpdateUI();
    }

    private void UpdateNextLevelRewardPref()
    {
        int diamondReward = levelElmasMiktarlari[(currentLevel - 1) % levelElmasMiktarlari.Length];
        PlayerPrefs.SetInt(NextDiamondRewardKey, diamondReward);
        PlayerPrefs.Save();
    }


}