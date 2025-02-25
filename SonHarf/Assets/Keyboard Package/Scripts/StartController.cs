using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StartController : MonoBehaviour
{
    [SerializeField] private Button startButton; // Inspector'dan atayacağınız buton

    void Start()
    {
        // Butonun tıklama olayına dinleyici ekle
        startButton.onClick.AddListener(StartAllScripts);
    }

    void StartAllScripts()
    {
        // Sahnedeki TÜM IStartable scriptlerini bul
        var allStartables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IStartable>();
        
        // Her birini başlat
        foreach (var script in allStartables)
        {
            script.Begin();
        }

    }
}