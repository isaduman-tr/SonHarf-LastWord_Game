using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] TextMeshProUGUI inputTextBox;
    [SerializeField] TextMeshProUGUI outputTextBox;
    [SerializeField] TextMeshProUGUI newWordTextBox;
    [SerializeField] private BubbleAnimationManager bubbleAnimationManager;


    [SerializeField] Transform scrollViewContent;         
    [SerializeField] Transform opponentScrollViewContent; 
    [SerializeField] GameObject textPrefab;
    [SerializeField] GameObject bubblePrefab; 
    [SerializeField] GameObject opponentBubblePrefab;

    private Dictionary<char, List<string>> wordDictionary;
    private int playerWordCount = 0; 
    private int opponentWordCount = 0;

    private void Start()
    {
        Instance = this;
        outputTextBox.text = "";
        inputTextBox.text = "";
        newWordTextBox.text = "";

        wordDictionary = new Dictionary<char, List<string>>();

        LoadWordsFromFiles();
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

    public void SubmitWord()
{
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
        PuanManager.Instance.AddPlayerScore(inputWord.Length);

        char lastChar = inputWord[inputWord.Length - 1]; // input son harf

        if (wordDictionary.ContainsKey(lastChar))
        {
            List<string> possibleWords = wordDictionary[lastChar];
            if (possibleWords.Count > 0)
            {
                string newOpponentWord = possibleWords[Random.Range(0, possibleWords.Count)];
                newWordTextBox.text = newOpponentWord;

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


    private IEnumerator DelayedAddWordToScrollView(Transform content, WordCounter wordCounter, string word, GameObject bubblePrefab)
    {
        yield return new WaitForSeconds(0.5f);
        AddWordToScrollView(content, ref wordCounter.Count, word, bubblePrefab);

    }

    private IEnumerator AddOpponentWordWithRandomDelay(string word)
    {
        float randomDelay = Random.Range(1f, 5f); // random zaman cevap
        yield return new WaitForSeconds(randomDelay);
        AddWordToScrollView(opponentScrollViewContent, ref opponentWordCount, word, opponentBubblePrefab);

        PuanManager.Instance.AddOpponentScore(word.Length);
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
                float additionalWidth = (wordLength - 2) * 100f;
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

        // content pos duruma gore silelim
        if (wordCount > 9)
        {
            RectTransform contentRect = content.GetComponent<RectTransform>();
            Vector2 newPosition = contentRect.anchoredPosition;
            newPosition.y += 100f;
            contentRect.anchoredPosition = newPosition;
        }
    }
}

