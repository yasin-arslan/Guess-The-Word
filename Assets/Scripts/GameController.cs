using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] TMP_InputField letterInputField;
    [SerializeField] TextMeshProUGUI charCount;
    [SerializeField] TextMeshProUGUI guessCountText;
    [SerializeField] GameObject letterPrefab;
    [SerializeField] Transform letterHolder;
    [SerializeField] TextMeshProUGUI guessInformationText;
    [SerializeField] TMP_InputField wordInputField;
    [SerializeField] Button guessButton;
    [SerializeField] Button wordGuessButton;
    string[] wordsArray;
    char[] splittedWordChars;
    List<string> solvedList = new List<string>();
    [SerializeField] List<TMP_Text> letterHolderList = new List<TMP_Text>();
    [SerializeField] TextMeshProUGUI fpsCounter;
    string filePath, fileName;
    public string currentWord;
    int maxRandomRange,randomIndex,charsInCurrentWord,guessCount;
    [SerializeField] Sprite[] spriteList;
    [SerializeField] Image image;
    [SerializeField] AudioSource correctSound;
    [SerializeField] AudioSource wrongSound;
    [SerializeField] AudioSource winSound;
    [SerializeField] AudioSource loseSound;
    [SerializeField] AudioMixer audioMixer;
    int currentIndex;
    void Start()
    {
        Time.timeScale = 1f;
        fileName = "Words.txt";
        filePath = Application.dataPath + "/StreamingAssets/" + fileName;
        Initialize();
        InvokeRepeating("getFps",0.1f,1);
        InvokeRepeating("changeBg",3.5f,3.5f);
    }
    void getFps(){
        int fps = (int)(1f/Time.unscaledDeltaTime);
        fpsCounter.text = fps.ToString();
    }
    void changeBg(){
        for (int i = 0; i < spriteList.Length - 1; i++)
        {
            if (spriteList[i] == image.sprite)
            {
                currentIndex = i;
            }
        }
        if(currentIndex >= spriteList.Length - 2){
            currentIndex = 0;
        }
        image.sprite = spriteList[currentIndex + 1];
    }
    void Initialize()
    {
        readFromFile();
        chooseWord();
        drawHolders();
        guessCount = charsInCurrentWord > 7 ? (charsInCurrentWord + 2) : (charsInCurrentWord + 1);
        updateGuessCounter(guessCount.ToString());
        wordInputField.gameObject.SetActive(false);
        wordGuessButton.gameObject.SetActive(false);
    }

    void readFromFile()
    {
        wordsArray = File.ReadAllLines(filePath);
    }
    void chooseWord()
    {
        maxRandomRange = wordsArray.Length;
        randomIndex = Random.Range(0, maxRandomRange);
        currentWord = wordsArray[randomIndex];
        splittedWordChars = currentWord.ToCharArray();
        charsInCurrentWord = splittedWordChars.Length;
        charCount.text = charsInCurrentWord.ToString();
        foreach (char letter in splittedWordChars)
        {
            solvedList.Add(letter.ToString());
        }
    }
    void updateGuessCounter(string guessCountNumber)
    {
        guessCountText.text = guessCountNumber;
    }
    void drawHolders()
    {
        for (int i = 0; i < solvedList.Count; i++)
        {
            GameObject tempObject = Instantiate(letterPrefab, letterHolder, false);
            letterHolderList.Add(tempObject.GetComponent<TMP_Text>());
        }
    }
    private IEnumerator fadeOutText(TextMeshProUGUI textToFadeOut, Color color)
    {
        float duration = 0.9f;
        float currentTime = 0f;
        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);
            textToFadeOut.color = new Color(color.r, color.g, color.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }
        yield break;
    }
    void updateGuessInformationText(string text, Color color)
    {
        guessInformationText.text = text;
        StartCoroutine(fadeOutText(guessInformationText, color));
    }
    void checkLetter(string letter)
    {
        for (int i = 0; i < solvedList.Count; i++)
        {
            if (solvedList[i] == letter)
            {
                updateGuessInformationText("Kelime bu karakteri içeriyor!", Color.green);
                letterHolderList[i].text = letter;
                correctSound.Play();
            }
        }
    }
    public void replayGame(){
        SceneManager.LoadScene("Game");
    }
    void wordGuessState()
    {
        letterInputField.gameObject.SetActive(false);
        guessButton.gameObject.SetActive(false);
        wordInputField.gameObject.SetActive(true);
        wordInputField.characterLimit = charsInCurrentWord;
        wordGuessButton.gameObject.SetActive(true);
    }
    public void winOrLoseMessage(){
        wordGuessButton.gameObject.SetActive(false);
        wordInputField.gameObject.SetActive(false);
        if(isPlayerWon() || isPlayerWonByLetterGuess()){
            updateGuessInformationText("KAZANDINIZ", Color.green);
            winSound.Play();
        }else
        {
            updateGuessInformationText("KAYBETTİNİZ", Color.red);
            loseSound.Play();
        }
        revealWord();
    }
    private void revealWord(){
        for (int i = 0; i < letterHolderList.Count; i++)
        {
           if(letterHolderList[i].text == "_"){
                letterHolderList[i].text = solvedList[i];
           }
        }
    }
    private bool isPlayerWon()
    {
        return wordInputField.text == currentWord ? true : false;
    }
    //Fix here
    private bool isPlayerWonByLetterGuess(){
        for (int i = 0; i < letterHolderList.Count; i++)
        {
            if(letterHolderList[i].text == "_"){
                return false;
            }
        }
        return true;
    }
    public void onPlayerGuess()
    {
        string playerGuessChar = letterInputField.text;
        if (string.IsNullOrEmpty(playerGuessChar))
        {
            updateGuessInformationText("Boş karakter giremezsiniz!",Color.red);
            StartCoroutine(fadeOutText(guessInformationText, Color.red));
        }
        else
        {
            guessCount--;
            updateGuessCounter(guessCount.ToString());
            if (guessCount == 0)
            {
                checkLetter(playerGuessChar);
                wordGuessState();
            }
            else
            {
                letterInputField.text = "";
                if (currentWord.ToLower().Contains(playerGuessChar))
                {
                    checkLetter(playerGuessChar);
                }
                else
                {
                    updateGuessInformationText("Kelime bu karakteri içermiyor!", Color.red);
                    wrongSound.Play();
                }
            }

        }
    }
    public void exit(){
        Application.Quit();
    }

}
