using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button _jokerButton;
    [SerializeField] private GameObject _jokerImage;

    void Start()
    {
        EnableJokerButton();
    
        _jokerButton.onClick.AddListener(() => 
        {
            if (JokerManager.Instance != null)
            {
                switch(JokerManager.Instance.GetCurrentJoker())
                {
                    case JokerType.RevealWord:
                    case JokerType.RevealWord2:   // Yeni eklenenler
                    case JokerType.RevealWord3:   // Yeni eklenenler
                        if (GameManager.Instance.playerTurnCount >= 2)
                        {
                            JokerKelimePanelController.Instance.ShowPanel();
                            DisableJokerButton();
                        }
                        break;
                    
                    case JokerType.DoubleScore:
                    case JokerType.DoubleScore2:
                    case JokerType.DoubleScore3:
                        JokerManager.Instance.ActivateDoubleScore();
                        DisableJokerButton();
                        break;
                        
                    case JokerType.FreezeTime:
                    case JokerType.FreezeTime2:
                    case JokerType.FreezeTime3:
                        JokerManager.Instance.ActivateFreezeTime();
                        DisableJokerButton();
                        break;
                }
            }
        });
    }
    public void EnableJokerButton()
    {
        _jokerButton.interactable = true;
    }

    public void DisableJokerButton()
    {
        _jokerButton.interactable = false;
        _jokerImage.SetActive(true);
    }
}