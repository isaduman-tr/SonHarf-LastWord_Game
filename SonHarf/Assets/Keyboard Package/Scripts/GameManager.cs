using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour , IStartable , IStoppable
{
    public static GameManager Instance;
    [SerializeField] TextMeshProUGUI inputTextBox;
    [SerializeField] TextMeshProUGUI outputTextBox;
    [SerializeField] TextMeshProUGUI newWordTextBox;
    [SerializeField] private TimerBar timerBar;
    [SerializeField] private BubbleAnimationManager bubbleAnimationManager;
    [SerializeField] private GameObject barArrow;
    [SerializeField] private Image raycastTargetImage;
    [SerializeField] private GameObject jokerImage;


    [SerializeField] Transform scrollViewContent;         
    [SerializeField] Transform opponentScrollViewContent; 
    [SerializeField] GameObject textPrefab;
    [SerializeField] GameObject bubblePrefab; 
    [SerializeField] GameObject opponentBubblePrefab;
    [SerializeField] private Transform jokerWordContent;
    [SerializeField] private GameObject jokerWordButtonPrefab;
    private GameObject currentJokerButton = null;
    private List<GameObject> currentJokerButtons = new List<GameObject>();

    private Dictionary<char, List<string>> wordDictionary;
    private int playerWordCount = 0; 
    private int opponentWordCount = 0;
    private bool isPlayerTurn;
    public int playerTurnCount = 0;
    private string lastOpponentWord = "";

    private void Start()
    {
        Instance = this;
        outputTextBox.text = "";
        inputTextBox.text = "";
        newWordTextBox.text = "";

        wordDictionary = new Dictionary<char, List<string>>();

        LoadWordsFromFiles();
        timerBar.OnTimerEnd += OnTimerEnded;
    }

    public void Begin()
    {
        StartGameRandomly();
        ClearAllContent();
    }

        private void OnTimerEnded()
    {
        if (isPlayerTurn)
        {
            // Oyuncunun süresi bitti, sıra rakibe geçsin
            isPlayerTurn = false;
            timerBar.ResetTimer();
            timerBar.StartTimer();
            UpdateBarArrowRotation();
            HideJokerWithAnimation();

            if (lastOpponentWord != "")
            {
                char lastChar = lastOpponentWord[lastOpponentWord.Length - 1];
                StartCoroutine(StartOpponentTurnWithLastChar(lastChar));
            }
            else
            {
                // Rakip rastgele bir kelime ile başlar
                StartCoroutine(StartOpponentTurn());
            }
        
        }

    }

    private void LoadWordsFromFiles()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "{0}.list");

        for (char c = 'a'; c <= 'ş'; c++)
        {
            string fileName = string.Format(path, c);
            if (File.Exists(fileName))
            {
                string[] words = File.ReadAllLines(fileName);
                wordDictionary[c] = new List<string>(words);
            }
        }
    }

    public void DeleteLetter()
    {
        if (inputTextBox.text.Length != 0)
        {
            inputTextBox.text = inputTextBox.text.Remove(inputTextBox.text.Length - 1, 1);
        }
    }

    public void AddLetter(string letter)
    {
        inputTextBox.text = inputTextBox.text + letter;
    }

    public class WordCounter
    {
        public int Count;
    }

        public void StartGameRandomly()
    {
        timerBar.ResetTimer();
        // Rastgele bir sayı üret (0 veya 1)
        int randomStart = Random.Range(0, 2);

        if (randomStart == 0)
        {
            // Oyuncu başlar
            Debug.Log("Oyuncu başlıyor!");
            isPlayerTurn = true;
            playerTurnCount++; // EKLE
        Debug.Log("Player turn count: " + playerTurnCount); // EKLE
            timerBar.StartTimer();
        }
        else
        {
            // Rakip başlar
            Debug.Log("Rakip başlıyor!");
            isPlayerTurn = false;
            timerBar.StartTimer();
            StartCoroutine(StartOpponentTurn());
        }
        UpdateBarArrowRotation();

    }


    private IEnumerator StartOpponentTurn()
    {
        // Rakip için rastgele bir kelime seç
        char randomChar = (char)Random.Range('a', 'z' + 1); // Rastgele bir harf seç
        if (wordDictionary.ContainsKey(randomChar))
        {
            List<string> possibleWords = wordDictionary[randomChar];
            if (possibleWords.Count > 0)
            {
                string randomWord = possibleWords[Random.Range(0, possibleWords.Count)];
                newWordTextBox.text = randomWord;
                lastOpponentWord = randomWord;

                // Rakibin kelimesini ScrollView'e ekle
                yield return StartCoroutine(AddOpponentWordWithRandomDelay(randomWord));
            }
            else
            {
                outputTextBox.text = "Listede yok, tekrar deneyiniz.";
            }
        }
        else
        {
            outputTextBox.text = "Listede yok, tekrar deneyiniz.";
        }

        // Rakibin hamlesi bitti, sıra oyuncuda
        isPlayerTurn = true;
        timerBar.ResetTimer();
        UpdateBarArrowRotation();
        timerBar.StartTimer();
        GenerateJokerWordButton();
    }

        private void UpdateBarArrowRotation()
{
    // Turn değişiminde TimerBar'ı default 20 saniyeye ayarla.
    if (timerBar != null)
    {
        timerBar.SetDefaultTime(20f);
    }
    
    float targetRotation = isPlayerTurn ? 45f : -45f;
    LeanTween.rotateZ(barArrow.gameObject, targetRotation, 0.5f)
             .setEase(LeanTweenType.easeOutQuad)
             .setOnComplete(() => 
             {
                 // Raycast durumunu güncelle: oyuncu sırası ise dokunma kapalı, değilse açık.
                 raycastTargetImage.raycastTarget = !isPlayerTurn;
             });
}

    public void SubmitWord()
{
    RemoveJokerWordButton();
    if (!isPlayerTurn)
    {
        outputTextBox.text = "Şu anda rakibin sırası!";
        return;
        
    }

    string inputWord = inputTextBox.text.ToLower();
    inputTextBox.text = "";

    if (inputWord.Length > 0)
    {
        bool isWordInList = false;
        foreach (var list in wordDictionary.Values)
        {
            if (list.Contains(inputWord))
            {
                isWordInList = true;
                break;
            }
        }

        if (!isWordInList)
        {
            outputTextBox.text = "Kelime StreamingAssets'te bulunamadı!";
            return;
        }

        string newWord = newWordTextBox.text.ToLower();
        if (newWord.Length > 0)
        {
            char newWordLastChar = newWord[newWord.Length - 1]; // Son harf
            char inputWordFirstChar = inputWord[0]; // ilk harf

            // input ilk harf son harf uyumu
            if (inputWordFirstChar != newWordLastChar)
            {
                // uyum yoksa
                outputTextBox.text = "Kelimenin baş harfi '" + newWordLastChar + "' ile başlamalı!";
                inputTextBox.text = inputWord;
                return;
            }
        }

        WordCounter playerWordCounter = new WordCounter { Count = playerWordCount };
        StartCoroutine(DelayedAddWordToScrollView(scrollViewContent, playerWordCounter, inputWord, bubblePrefab));

        if (bubbleAnimationManager != null)
        {
            bubbleAnimationManager.PlayBubbleAnimation(inputWord);
        }
        float remainingTime = timerBar.GetRemainingTime();
        PuanManager.Instance.AddPlayerScore(inputWord.Length, remainingTime);

        char lastChar = inputWord[inputWord.Length - 1]; // input son harf

        if (wordDictionary.ContainsKey(lastChar))
        {
            List<string> possibleWords = wordDictionary[lastChar];
            if (possibleWords.Count > 0)
            {
                string newOpponentWord = possibleWords[Random.Range(0, possibleWords.Count)];
                newWordTextBox.text = newOpponentWord;
                lastOpponentWord = newOpponentWord;

                // Rakibin hamlesini başlat
                isPlayerTurn = false; // Rakibin sırası
                UpdateBarArrowRotation();
                timerBar.ResetTimer();
                timerBar.StartTimer();
                HideJokerWithAnimation();
                StartCoroutine(AddOpponentWordWithRandomDelay(newOpponentWord));
            }
            else
            {
                outputTextBox.text = "Listede yok, tekrar deneyiniz.";
            }
        }
        else
        {
            outputTextBox.text = "Listede yok, tekrar deneyiniz.";
        }

        playerWordCount = playerWordCounter.Count;
    }
}

private IEnumerator StartOpponentTurnWithLastChar(char lastChar)
    {
        if (wordDictionary.ContainsKey(lastChar))
        {
            List<string> possibleWords = wordDictionary[lastChar];
            if (possibleWords.Count > 0)
            {
                string randomWord = possibleWords[Random.Range(0, possibleWords.Count)];
                newWordTextBox.text = randomWord;
                lastOpponentWord = randomWord; // Rakibin son kelimesini güncelle

                // Rakibin kelimesini ScrollView'e ekle
                yield return StartCoroutine(AddOpponentWordWithRandomDelay(randomWord));
            }
            else
            {
                outputTextBox.text = "Listede yok, tekrar deneyiniz.";
            }
        }
        else
        {
            outputTextBox.text = "Listede yok, tekrar deneyiniz.";
        }

        // Rakibin hamlesi bitti, sıra oyuncuda
        isPlayerTurn = true;
        playerTurnCount++; // EKLE
    Debug.Log("Player turn count: " + playerTurnCount); // EKLE
        timerBar.ResetTimer(); // Zamanlayıcıyı sıfırla
        timerBar.StartTimer(); // Zamanlayıcıyı başlat
        UpdateBarArrowRotation();
        GenerateJokerWordButton();
    }

    private IEnumerator DelayedAddWordToScrollView(Transform content, WordCounter wordCounter, string word, GameObject bubblePrefab)
    {
        yield return new WaitForSeconds(0.5f);
        AddWordToScrollView(content, ref wordCounter.Count, word, bubblePrefab);

    }

    private IEnumerator AddOpponentWordWithRandomDelay(string word)
{
    float randomDelay = Random.Range(18f, 19f); // random zaman cevap
    yield return new WaitForSeconds(randomDelay);
    AddWordToScrollView(opponentScrollViewContent, ref opponentWordCount, word, opponentBubblePrefab);
    float remainingTime = timerBar.GetRemainingTime();

    PuanManager.Instance.AddOpponentScore(word.Length, remainingTime);
    // Rakibin hamlesi bitti, sıra oyuncuda
    isPlayerTurn = true; // Oyuncunun sırası
    playerTurnCount++; // EKLE
    Debug.Log("Player turn count: " + playerTurnCount); // EKLE
    timerBar.ResetTimer();
    timerBar.StartTimer(); // Zamanlayıcıyı yeniden başlat
    UpdateBarArrowRotation();
    GenerateJokerWordButton();
}


    private void AddWordToScrollView(Transform content, ref int wordCount, string word, GameObject bubblePrefab)
    {

        // Bubble prefab
        GameObject newBubble = Instantiate(bubblePrefab, content);

        // Prefab bubble Text
        TextMeshProUGUI textComponent = newBubble.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = word;
        }


        // harf image genislik
        RectTransform bubbleRect = newBubble.GetComponent<RectTransform>();
        if (bubbleRect != null)
        {
            int wordLength = word.Length;
            if (wordLength > 2)
            {
                float additionalWidth = (wordLength - 2) * 80f;
                bubbleRect.sizeDelta = new Vector2(bubbleRect.sizeDelta.x + additionalWidth, bubbleRect.sizeDelta.y);
            }

            // dalga anim
            Vector3 initialScale = new Vector3(0.2f, 0.2f, 1f);
            bubbleRect.localScale = initialScale;


            LeanTween.scale(bubbleRect, new Vector3(0.3f, 0.3f, 1f), 0.1f)
                     .setOnComplete(() =>
                     {
                         LeanTween.scale(bubbleRect, new Vector3(0.2f, 0.2f, 1f), 0.3f)
                                  .setEase(LeanTweenType.easeOutBounce);
                     });
        }


        wordCount++;

    }

    public void HideJokerWithAnimation()
    {
    if (!jokerImage.activeSelf) return; // Eğer zaten kapalıysa işlem yapma

    // Animator bileşenini kapat
    Animator animator = jokerImage.GetComponent<Animator>();
    if (animator != null) animator.enabled = false; // Animator'ü devre dışı bırak

    LeanTween.cancel(jokerImage); // Önce diğer animasyonları iptal et

    // Eğer bir UI objesiyse, RectTransform üzerinden işlem yap
    RectTransform jokerRect = jokerImage.GetComponent<RectTransform>();

    // LeanTween animasyonu başlat
    LeanTween.scale(jokerRect, Vector3.zero, 0.5f)
        .setEase(LeanTweenType.easeInOutQuad)
        .setIgnoreTimeScale(false)
        .setOnComplete(() => 
        {
            jokerImage.SetActive(false); // Tamamen kaybolunca kapat
            jokerImage.transform.localScale = Vector3.one; // Resetle
            if (animator != null) animator.enabled = true; // Animasyonu tekrar aç
        });
    }

    private void GenerateJokerWordButton()
    {
        RemoveJokerWordButtons(); // Eski butonları temizle
        
        if(string.IsNullOrEmpty(lastOpponentWord)) return;
        
        char lastChar = lastOpponentWord[lastOpponentWord.Length - 1];
        
        if(!wordDictionary.ContainsKey(lastChar)) return;

        // Joker tipine göre buton sayısını belirle
        int buttonCount = 1;
        if(JokerManager.Instance.GetCurrentJoker() == JokerType.RevealWord2) buttonCount = 2;
        else if(JokerManager.Instance.GetCurrentJoker() == JokerType.RevealWord3) buttonCount = 3;

        // Kelime listesinden rastgele X adet kelime seç
        List<string> selectedWords = new List<string>();
        List<string> possibleWords = wordDictionary[lastChar];
        
        for(int i = 0; i < buttonCount; i++)
        {
            if(possibleWords.Count == 0) break;
            
            int randomIndex = Random.Range(0, possibleWords.Count);
            selectedWords.Add(possibleWords[randomIndex]);
        }

        // Seçilen kelimeler için buton oluştur
        foreach(string word in selectedWords)
        {
            GameObject newButton = Instantiate(jokerWordButtonPrefab, jokerWordContent);
            currentJokerButtons.Add(newButton);

            TextMeshProUGUI btnText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = word;

            Button btn = newButton.GetComponent<Button>();
            btn.onClick.AddListener(() => UseJokerWord(word));
        }
    }

    private void RemoveJokerWordButtons()
    {
        foreach(GameObject btn in currentJokerButtons)
        {
            Destroy(btn);
        }
        currentJokerButtons.Clear();
    }
    
    private void RemoveJokerWordButton()
{
    if (currentJokerButton != null)
    {
        Destroy(currentJokerButton);
        currentJokerButton = null;
    }
}   

    public void UseJokerWord(string jokerWord)
{
    // Joker kelime kullanıldı, butonu kaldır
    RemoveJokerWordButton();

    // Button üzerindeki kelimeyi inputTextBox'a aktar
    inputTextBox.text = jokerWord;

    // Oyuncu kelime girişi metodunu çağır (normal SubmitWord() akışı uygulanır)
    SubmitWord();
}


    public void StopGame()
    {
        StopAllCoroutines();
        timerBar.StopTimer();
        isPlayerTurn = false;
        Debug.Log("Oyun durduruldu");
    }

     public void ClearAllContent()
    {
        // Player scroll view temizleme
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }
        
        // Opponent scroll view temizleme
        foreach (Transform child in opponentScrollViewContent)
        {
            Destroy(child.gameObject);
        }

        playerWordCount = 0;
        opponentWordCount = 0;
        lastOpponentWord = "";
        inputTextBox.text = "";
        newWordTextBox.text = "";
    }
}