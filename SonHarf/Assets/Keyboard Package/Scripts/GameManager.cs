using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] TextMeshProUGUI inputTextBox;
    [SerializeField] TextMeshProUGUI outputTextBox;
    [SerializeField] TextMeshProUGUI newWordTextBox; // Yeni kelimeyi yazd�rmak i�in

    private Dictionary<char, List<string>> wordDictionary;

    private void Start()
    {
        Instance = this;
        outputTextBox.text = "";
        inputTextBox.text = "";
        newWordTextBox.text = "";

        // Kelime listesini olu�turun
        wordDictionary = new Dictionary<char, List<string>>()
        {
            { 'a', new List<string> { "araba", "ayva", "armut" } },
            { 'b', new List<string> { "bal�k", "biber", "bardak" } },
            { 'k', new List<string> { "kedi", "kaplumba�a", "kale" } },
            // Di�er harfler i�in benzer listeler ekleyin
        };
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

            if (wordDictionary.ContainsKey(lastChar))
            {
                List<string> possibleWords = wordDictionary[lastChar];
                string newWord = possibleWords[Random.Range(0, possibleWords.Count)];
                newWordTextBox.text = newWord; // Yeni kelimeyi bu bile�ene yazd�r
            }
            else
            {
                newWordTextBox.text = "Bu harfle ba�layan kelime bulunamad�.";
            }
        }
    }
}