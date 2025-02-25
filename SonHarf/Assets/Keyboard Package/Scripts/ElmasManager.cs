using UnityEngine;
using TMPro; // TextMeshPro kullanacaksanız

public class ElmasManager : MonoBehaviour
{
    public static ElmasManager Instance; // Singleton pattern

    [SerializeField] private int elmas = 0;
    [SerializeField] private TMP_Text elmasText;  // Ana UI Text
    [SerializeField] private TMP_Text elmasText2; // Ekstra UI Text (aynı işlevi görecek)
    private const string DiamondKey = "PlayerDiamond";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadDiamond();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Başlangıçta animasyonsuz güncelle
        UpdateElmasText(true);
    }

    public void AddElmas(int amount)
    {
        int previousDiamond = elmas; // Önceki elmas değerini saklıyoruz
        elmas += amount;
        // Yumuşak geçiş için önceki değerden yeni değere animasyonlu güncelleme
        UpdateElmasText(false, previousDiamond);
        SaveDiamond();
    }

    public void SpendElmas(int amount)
    {
        int previousDiamond = elmas;
        elmas -= amount;
        UpdateElmasText(false, previousDiamond);
        SaveDiamond();
    }

    public int GetCurrentElmas()
    {
        return elmas;
    }

    /// <summary>
    /// Elmas textlerini günceller.
    /// </summary>
    /// <param name="instant">
    /// true ise değer hemen güncellenir,
    /// false ise önceki değerden yeni değere animasyonlu geçiş yapılır.
    /// </param>
    /// <param name="previousValue">Animasyona başlanacak önceki değer (instant false ise kullanılır).</param>
    private void UpdateElmasText(bool instant = false, int previousValue = 0)
    {
        float animationTime = 0.5f;

        // elmasText için güncelleme
        if (elmasText != null)
        {
            if (instant)
            {
                elmasText.text = elmas.ToString();
            }
            else
            {
                LeanTween.value(gameObject, previousValue, elmas, animationTime)
                    .setOnUpdate((float val) =>
                    {
                        elmasText.text = Mathf.RoundToInt(val).ToString();
                    });
            }
        }

        // elmasText2 için güncelleme, 1 saniye delay ile
        if (elmasText2 != null)
        {
            if (instant)
            {
                elmasText2.text = elmas.ToString();
            }
            else
            {
                LeanTween.value(gameObject, previousValue, elmas, animationTime)
                    .setDelay(1f)
                    .setOnUpdate((float val) =>
                    {
                        elmasText2.text = Mathf.RoundToInt(val).ToString();
                    });
            }
        }
    }

    private void SaveDiamond()
    {
        PlayerPrefs.SetInt(DiamondKey, elmas);
        PlayerPrefs.Save();
    }

    private void LoadDiamond()
    {
        elmas = PlayerPrefs.GetInt(DiamondKey, 0);
        UpdateElmasText(true);
    }
}
