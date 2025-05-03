using UnityEngine;
using TMPro;

public class PuanManager : MonoBehaviour, IStartable, IStoppable
{
    public static PuanManager Instance;

    public delegate void ScoreUpdated();
    public event ScoreUpdated OnScoreUpdated;
    private int playerScore = 0;
    private int opponentScore = 0;
    private float _fixedMultiplier = 1f;
    public static bool jokerbutonunabastın =false;

    [SerializeField] public TextMeshProUGUI playerScoreTextBox;
    [SerializeField] public TextMeshProUGUI playerScoreTextBox2;
    [SerializeField] public TextMeshProUGUI playerScoreTextBox3;
    [SerializeField] TextMeshProUGUI opponentScoreTextBox;
    [SerializeField] private LevelSystem levelSystem;
    [SerializeField] private TextMeshProUGUI multiplierTextBox;

    [SerializeField] private TimerBar timerBar;

    private int currentDisplayedMultiplier = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (timerBar == null)
            timerBar = Object.FindFirstObjectByType<TimerBar>();

        UpdateMultiplierUI(1);
    }

    private void Update()
    {
        // Eğer sabit çarpan varsa, UI'yi sabit tut, hiç değiştirme
        if (_fixedMultiplier != 1f) return;

        if (timerBar != null)
        {
            float remainingTime = timerBar.GetRemainingTime();
            int m = GetMultiplier(remainingTime);
            if (m != currentDisplayedMultiplier)
            {
                currentDisplayedMultiplier = m;
                UpdateMultiplierUI(m);
            }
        }
    }

    public void Begin()
    {
        ResetScores();
        UpdatePlayerScoreUI();
        UpdateOpponentScoreUI();
        UpdateMultiplierUI(1);
    }

    public void AddPlayerScore(int points, float remainingTime)
    {
        int multiplier = _fixedMultiplier != 1f
                            ? Mathf.RoundToInt(_fixedMultiplier)
                            : GetMultiplier(remainingTime);

        UpdateMultiplierUI(multiplier);
        int totalPoints = Mathf.RoundToInt(points * multiplier);
        Debug.Log("Toplam Puan: " + totalPoints + " (Points: " + points + ", Multiplier: " + multiplier + ")");
        playerScore += totalPoints;
        UpdatePlayerScoreUI();
        OnScoreUpdated?.Invoke();
    }

    public void AddOpponentScore(int points, float remainingTime)
    {
        

        int multiplier = GetMultiplier(remainingTime);
        opponentScore += points * multiplier;
        UpdateOpponentScoreUI();
        OnScoreUpdated?.Invoke();
    }

    private int GetMultiplier(float remainingTime)
    {
        if (remainingTime >= 18f) return 5;
        else if (remainingTime >= 15f) return 4;
        else if (remainingTime >= 10f) return 3;
        else if (remainingTime >= 5f) return 2;
        else return 1;
    }

    public void UpdateMultiplierUI(int multiplier)
    {
        if (multiplierTextBox != null)
            multiplierTextBox.text = multiplier + "x";
    }

    private void UpdatePlayerScoreUI()
    {
        if (playerScoreTextBox != null)
            playerScoreTextBox.text = playerScore.ToString();

        if (playerScoreTextBox2 != null)
        {
            LeanTween.value(playerScoreTextBox2.gameObject, 0, playerScore, 1.5f)
                .setOnUpdate((float value) =>
                {
                    playerScoreTextBox2.text = "+" + Mathf.RoundToInt(value).ToString();
                })
                .setEase(LeanTweenType.easeOutQuad);
        }

        if (playerScoreTextBox3 != null)
        {
            LeanTween.value(playerScoreTextBox3.gameObject, 0, playerScore, 1.5f)
                .setOnUpdate((float value) =>
                {
                    playerScoreTextBox3.text = Mathf.RoundToInt(value).ToString();
                })
                .setEase(LeanTweenType.easeOutQuad);
        }
    }

    private void UpdateOpponentScoreUI()
    {
        if (opponentScoreTextBox != null)
            opponentScoreTextBox.text = opponentScore.ToString();
    }

    public int GetPlayerScore() => playerScore;
    public int GetOpponentScore() => opponentScore;

    public void ResetScores()
    {
        playerScore = 0;
        opponentScore = 0;
        UpdatePlayerScoreUI();
        UpdateOpponentScoreUI();
        UpdateMultiplierUI(1);
    }

    public void StopGame()
    {
        OnScoreUpdated = null;
        ResetScores();
        ResetMultiplier();
    }

    public void ApplyFixedMultiplier(float multiplier)
    {
        _fixedMultiplier = multiplier;
        currentDisplayedMultiplier = Mathf.RoundToInt(multiplier); // ✅ Joker UI'ı sabit kalır
        Debug.Log($"Sabit Çarpan Aktif: {multiplier}x");
        UpdateMultiplierUI(currentDisplayedMultiplier);
    }

    public void ResetMultiplier()
    {
        _fixedMultiplier = 1f;
        Debug.Log("Sabit Çarpan Sıfırlandı!");
        currentDisplayedMultiplier = -1;
    }

    public void JokerButton()
    {
        JokerType joker = JokerManager.Instance.GetCurrentJoker();
        float multiplier = 1f;
        float extraTime = 0f;

        // Joker türünü kontrol et
        switch (joker)
        {
            // DoubleScore türü jokerler için çarpan değeri belirle
            case JokerType.DoubleScore:
                multiplier = 8f;
                break;
            case JokerType.DoubleScore2:
                multiplier = 10f;
                break;
            case JokerType.DoubleScore3:
                multiplier = 15f;
                break;

            // FreezeTime türü jokerler için süre ekle
            case JokerType.FreezeTime:
                extraTime = 10f;
                break;
            case JokerType.FreezeTime2:
                extraTime = 15f;
                break;
            case JokerType.FreezeTime3:
                extraTime = 20f;
                break;

            // Geçerli joker türü yoksa uyarı ver
            default:
                Debug.LogWarning("Geçerli bir joker tipi bulunamadı.");
                return;
        }

        // Eğer DoubleScore jokeri aktifse, çarpan uygula
        if (joker == JokerType.DoubleScore || joker == JokerType.DoubleScore2 || joker == JokerType.DoubleScore3)
        {
            ApplyFixedMultiplier(multiplier);
        }

        // Eğer FreezeTime jokeri aktifse, ekstra zaman ekle
        if (joker == JokerType.FreezeTime || joker == JokerType.FreezeTime2 || joker == JokerType.FreezeTime3)
        {
            TimerBar timerBar = Object.FindFirstObjectByType<TimerBar>();
            if (timerBar != null)
            {
                timerBar.AddExtraTime(extraTime);
                Debug.Log($"FreezeTime aktif: {extraTime} saniye eklendi.");
            }
        }
    }
}
