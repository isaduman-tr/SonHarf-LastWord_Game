using UnityEngine;

public class WinLoseManager : MonoBehaviour , IStartable , IStoppable
{
    [SerializeField] private GameObject winPanel; // Kazanma paneli
    [SerializeField] private GameObject losePanel; // Kaybetme paneli
    [SerializeField] private GameObject chatPanel; // Chat paneli
    [SerializeField] private LevelSystem levelSystem;

    private int winScore = 50; // Kazanmak için gereken puan
    private bool isGameOver = false; // Oyunun bitip bitmediğini kontrol et

    public void Begin()
    {
        InitializePanels();
        SubscribeToEvents();
    }

    private void InitializePanels()
    {
        SetPanelAlpha(winPanel, 0);
        SetPanelAlpha(losePanel, 0);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        chatPanel.SetActive(true);
    }

    private void SubscribeToEvents()
    {
        if (PuanManager.Instance != null)
        {
            PuanManager.Instance.OnScoreUpdated += CheckForWin;
        }
    }


    private void OnDestroy()
    {
        // Event bağlantısını kaldır
        if (PuanManager.Instance != null)
        {
            PuanManager.Instance.OnScoreUpdated -= CheckForWin;
        }
    }

    private void CheckForWin()
    {
        if (isGameOver) return; // Oyun zaten bittiyse kontrol etme

        int playerScore = PuanManager.Instance.GetPlayerScore();
        int opponentScore = PuanManager.Instance.GetOpponentScore();

        if (playerScore >= winScore)
        {
            EndGame(true); // Oyuncu kazandı
        }
        else if (opponentScore >= winScore)
        {
            EndGame(false); // Rakip kazandı
        }
    }

    private void EndGame(bool isPlayerWinner)
{
    isGameOver = true; // Oyunun bittiğini işaretle

    // Chat panelini kapat
    if (chatPanel != null)
    {
        chatPanel.SetActive(false);
    }

    if (isPlayerWinner)
    {
        // Kazanma panelini göster (fade-in efekti ile)
        ShowPanelWithFade(winPanel);

        // PlayerWinXp olarak playerScoreTextBox'daki değeri al ve Debug.Log ile yazdır
        if (PuanManager.Instance != null && PuanManager.Instance.playerScoreTextBox != null)
        {
            string playerWinXpText = PuanManager.Instance.playerScoreTextBox.text; // TextMeshPro'daki metni al

            // Debug ile kontrol
            Debug.Log("PlayerWinXp Text: " + playerWinXpText);

            // String'den sadece sayısal kısmı ayıkla
            string numericPart = playerWinXpText.Replace("Oyuncu Puanı: ", "").Trim();

            // TryParse ile güvenli bir şekilde sayıya dönüştür
            if (int.TryParse(numericPart, out int playerWinXp))
            {
                Debug.Log("PlayerWinXp: " + playerWinXp); // Debug olarak yazdır

                // LevelSystem'e XP ekle
                if (levelSystem != null)
                {
                    levelSystem.AddXP(playerWinXp);
                }
            }
            else
            {
                Debug.LogError("PlayerWinXp değeri sayıya dönüştürülemedi: " + numericPart);
            }
        }
    }
    else
    {
        // Kaybetme panelini göster (fade-in efekti ile)
        ShowPanelWithFade(losePanel);
    }
}

    private void ShowPanelWithFade(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true); // Paneli aktif et
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0; // Başlangıçta tamamen şeffaf
                LeanTween.alphaCanvas(canvasGroup, 1, 0.3f).setEase(LeanTweenType.easeInOutQuad); // Animasyonu başlat
            }
        }
    }

    private void SetPanelAlpha(GameObject panel, float alpha)
    {
        if (panel != null)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
        }
    }

    public void RestartGame()
    {
        // Panelleri gizle ve alpha değerini sıfırla
        SetPanelAlpha(winPanel, 0);
        SetPanelAlpha(losePanel, 0);

        // Panelleri pasif hale getir
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        // Chat panelini tekrar aç
        if (chatPanel != null)
        {
            chatPanel.SetActive(true);
        }

        isGameOver = false; // Oyunun yeniden başladığını işaretle

        // Puanları sıfırla
        PuanManager.Instance.ResetScores();

        // Oyunu yeniden başlat (GameManager üzerinden)
        GameManager.Instance.StartGameRandomly();
    }

    public void ResetGame()
    {
        isGameOver = false;
        SetPanelAlpha(winPanel, 0);
        SetPanelAlpha(losePanel, 0);
    }

    public void StopGame()
    {
        GameManager.Instance.ClearAllContent();
        ResetGame();
        chatPanel.SetActive(false);
    }
    public void ShowPausePanel()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }
}