using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] TextMeshProUGUI inputTextBox;
    [SerializeField] TextMeshProUGUI outputTextBox;
    [SerializeField] TextMeshProUGUI newWordTextBox;

    private Dictionary<char, List<string>> wordDictionary;

    private void Start()
    {
        Instance = this;
        outputTextBox.text = "";
        inputTextBox.text = "";
        newWordTextBox.text = "";

        wordDictionary = new Dictionary<char, List<string>>();

        // Dosyalardan kelimeleri y�kle
        LoadWordsFromFiles();
    }

    private void LoadWordsFromFiles()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "{0}.list");

        for (char c = 'a'; c <= 'z'; c++)
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

    public void SubmitWord()
    {
        string inputWord = inputTextBox.text.ToLower();
        inputTextBox.text = "";

        if (inputWord.Length > 0)
        {
            char lastChar = inputWord[inputWord.Length - 1];

            // �lk olarak, kullan�c�n�n girdi�i kelimenin mevcut olup olmad���n� kontrol edelim.
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
                newWordTextBox.text = "Bu kelime listede yok.";
                return;
            }

            // E�er kelime ge�erli ise devam et
            if (wordDictionary.ContainsKey(lastChar))
            {
                List<string> possibleWords = wordDictionary[lastChar];
                if (possibleWords.Count > 0)
                {
                    // Yeni kelimeyi se� ve g�ster
                    string newWord = possibleWords[Random.Range(0, possibleWords.Count)];
                    newWordTextBox.text = newWord;
                }
                else
                {
                    newWordTextBox.text = "Listede yok, tekrar deneyiniz .";
                }
            }
            else
            {
                newWordTextBox.text = "Listede yok, tekrar deneyiniz.";
            }
        }
    }
}