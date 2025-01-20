using UnityEngine;
using TMPro;

public class PuanManager : MonoBehaviour
{
    public static PuanManager Instance;

    private int playerScore = 0;
    private int opponentScore = 0;

    [SerializeField] TextMeshProUGUI playerScoreTextBox;
    [SerializeField] TextMeshProUGUI opponentScoreTextBox;

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

    public void AddPlayerScore(int points)
    {
        playerScore += points;
        UpdatePlayerScoreUI();
    }

    public void AddOpponentScore(int points)
    {
        opponentScore += points;
        UpdateOpponentScoreUI();
    }

    private void UpdatePlayerScoreUI()
    {
        if (playerScoreTextBox != null)
        {
            playerScoreTextBox.text = "Oyuncu Puanı: " + playerScore;
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
}
