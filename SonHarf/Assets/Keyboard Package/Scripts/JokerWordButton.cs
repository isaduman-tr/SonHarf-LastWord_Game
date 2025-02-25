using UnityEngine;
using UnityEngine.UI;

public class JokerWordButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if(JokerKelimePanelController.Instance != null)
        {
            JokerKelimePanelController.Instance.HidePanel();
        }
    }
}

