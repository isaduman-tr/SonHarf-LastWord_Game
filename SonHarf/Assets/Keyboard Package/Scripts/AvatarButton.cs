// AvatarButton.cs
using UnityEngine;
using UnityEngine.UI;

public class AvatarButton : MonoBehaviour
{
    public int buttonID; // 0'dan 26'ya kadar unique ID
    public Sprite avatarSprite;
    public JokerType jokerType; // Inspector'dan "DoubleScore" seçilecek
    public float jokerDuration = 20f; // 20 saniye

    void Start()
    {
        // Buton tıklamasını bu script'e bağla
    }

    //private void OnButtonClicked()
//{
    // Joker bilgilerini parametre olarak ekleyin
    //AvatarMarketManager.Instance.SelectAvatar(
       // buttonID, 
       // avatarSprite, 
       // GetComponent<RectTransform>(),
       // jokerType, // Joker tipi
       // jokerDuration // Joker süresi
    //);
//}
    
}

