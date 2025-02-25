using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StopController : MonoBehaviour

{
    [SerializeField] private Button stopButton;
    [SerializeField] private WinLoseManager winLoseManager;
    [SerializeField] private BubbleAnimationManager bubbleAnimationManager;
    [SerializeField] private UIManager _uiManager;

    void Start()
    {
        stopButton.onClick.AddListener(StopAllSystems);
    }

    void StopAllSystems()
    {
        var allStoppables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IStoppable>();
        foreach (var system in allStoppables)
        {
            system.StopGame();
        }

        if (bubbleAnimationManager != null)
        {
            bubbleAnimationManager.ResetAnimation(); // Animasyonu durdur ve sıfırla
        }
        
        // Ekstra UI işlemleri
        winLoseManager.ShowPausePanel(); // Örnek metod
        _uiManager.EnableJokerButton();
    }
}
