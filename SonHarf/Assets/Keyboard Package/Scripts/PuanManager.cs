using UnityEngine;
using TMPro;

public class PuanManager : MonoBehaviour , IStartable , IStoppable
{
    public static PuanManager Instance;

    public delegate void ScoreUpdated();
    public event ScoreUpdated OnScoreUpdated;
    private int playerScore = 0;
    private int opponentScore = 0;
    private float _fixedMultiplier = 1f;

    [SerializeField] public TextMeshProUGUI playerScoreTextBox;
    [SerializeField] public TextMeshProUGUI playerScoreTextBox2;
    [SerializeField] public TextMeshProUGUI playerScoreTextBox3;
    [SerializeField] TextMeshProUGUI opponentScoreTextBox;
    [SerializeField] private LevelSystem levelSystem;

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

    public void Begin()
    {
        ResetScores(); // Oyun başlangıcında puanları sıfırla
        UpdatePlayerScoreUI(); // UI'ı güncelle
        UpdateOpponentScoreUI();
    }

    public void AddPlayerScore(int points, float remainingTime)
    {
        // Eğer joker (double score) aktifse _fixedMultiplier 1'den farklı olur.
    int multiplier = _fixedMultiplier != 1f 
                        ? Mathf.RoundToInt(_fixedMultiplier)  // Joker aktif: multiplier = 8
                        : GetMultiplier(remainingTime);         // Normalde: zaman dilimine göre multiplier

    int totalPoints = Mathf.RoundToInt(points * multiplier);
    Debug.Log("Toplam Puan: " + totalPoints + " (Points: " + points + ", Multiplier: " + multiplier + ")");
    playerScore += totalPoints;
    UpdatePlayerScoreUI();
    OnScoreUpdated?.Invoke();
    }

    public void AddOpponentScore(int points, float remainingTime)
    {
        int multiplier = GetMultiplier(remainingTime); // Zaman dilimine göre kat sayıyı al
        opponentScore += points * multiplier; // Puanı katlayarak ekle
        UpdateOpponentScoreUI();
        OnScoreUpdated?.Invoke();
    }

    private int GetMultiplier(float remainingTime)
    {
        // Zaman dilimlerine göre kat sayıyı belirle
        if (remainingTime >= 18f)
        {
            return 5; // 20-18 saniye arası: 5x
        }
        else if (remainingTime >= 15f)
        {
            return 4; // 18-15 saniye arası: 4x
        }
        else if (remainingTime >= 10f)
        {
            return 3; // 15-10 saniye arası: 3x
        }
        else if (remainingTime >= 5f)
        {
            return 2; // 10-5 saniye arası: 2x
        }
        else
        {
            return 1; // 5-0 saniye arası: 1x
        }
    }

    private void UpdatePlayerScoreUI()
    {
        if (playerScoreTextBox != null)
        {
            playerScoreTextBox.text = "Oyuncu Puanı: " + playerScore;
        }

        if (playerScoreTextBox2 != null)
        {
            // LeanTween animasyonu ile puanı 0'dan başlatıp belirtilen değere doğru artır
            LeanTween.value(playerScoreTextBox2.gameObject, 0, playerScore, 1.5f)
                .setOnUpdate((float value) =>
                {
                    playerScoreTextBox2.text = "+" + Mathf.RoundToInt(value).ToString(); // Değeri yuvarlayarak göster
                })
                .setEase(LeanTweenType.easeOutQuad); // Easing tipi
        }

        if (playerScoreTextBox3 != null)
        {
            // LeanTween animasyonu ile puanı 0'dan başlatıp belirtilen değere doğru artır
            LeanTween.value(playerScoreTextBox3.gameObject, 0, playerScore, 1.5f)
                .setOnUpdate((float value) =>
                {
                    playerScoreTextBox3.text = Mathf.RoundToInt(value).ToString(); // Değeri yuvarlayarak göster
                })
                .setEase(LeanTweenType.easeOutQuad); // Easing tipi
        }
    }

    private void UpdateOpponentScoreUI()
    {
        if (opponentScoreTextBox != null)
        {
            opponentScoreTextBox.text = "Rakip Puanı: " + opponentScore;
        }
    }

    public int GetPlayerScore()
    {
        return playerScore;
    }

    public int GetOpponentScore()
    {
        return opponentScore;
    }

    public void ResetScores()
    {
        playerScore = 0;
        opponentScore = 0;
        UpdatePlayerScoreUI();
        UpdateOpponentScoreUI();
    }

    public void StopGame()
    {
        OnScoreUpdated = null; // Eventleri temizle
        ResetScores();
    }

    public void ApplyFixedMultiplier(float multiplier)
{
    _fixedMultiplier = multiplier;
    Debug.Log($"Sabit Çarpan Aktif: {multiplier}x");
}

public void ResetMultiplier()
{
    _fixedMultiplier = 1f;
    Debug.Log("Sabit Çarpan Sıfırlandı!");
}
}