using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadQuestion : MonoBehaviour
{
    public GameObject filesListPan, filesContent, filePrefab;
    public static LoadQuestion instance;
    public Text question;
    public Text[] questionsArray;
    public ToggleGroup toggleGroup;
    public Toggle[] toggles;
    public Image imFF;
    public Sprite disableSpriteFF;
    public Image imHP;
    public Sprite disableSpriteHP;
    public Image imCF;
    public Sprite disableSpriteCF;
    public Image[] prize;
    public Sprite[] spritePrizes;
    public RawImage selectTextFile;

    /*
    public Text timerText;
    public Image timerImage;
    private float timerCount = 60;*/

    private GameObject[] instancedObjects;
    private DirectoryInfo[] files;
    private string liteQuestionList;
    private string middleQuestionList;
    private string hardQuestionList;
    private string folderPath;
    private List<string> eachLine;
    private int amountLines;
    private int numberQuestion = 0;
    private int[] listPastQuestions;
    private bool isFiftyFifty = true;
    private bool isHelpPublic = true;
    private bool isCallFriend = true;
    private int numberOfPrize = 0;
    private int folderIndex = 0;

    void Start()
    {
        imFF.gameObject.SetActive(false);
        imHP.gameObject.SetActive(false);
        imCF.gameObject.SetActive(false);
        toggleGroup.gameObject.SetActive(false);
    }

    private void Awake()
    {
        instance = this;
    }

    public void FiftyFifty()
    {
        if (isFiftyFifty)
        {
            imFF.sprite = disableSpriteFF;

            int i = FindTrueAnswer();
            int j = 4;
            do
            {
                j = (int)Random.Range(0, 4);
                if (j == i)
                {
                    do
                    {
                        j = (int)Random.Range(0, 4); // обираємо випадкову неправильну відповідь
                    } while (j == i);
                }
            } while (j == 4);


            for (int k = 0; k < 4; k++) // очищаємо відповіді на екрані
            {
                if (k == j || k == i) continue; // якщо натрапили на правильну або обрану неправильну, то не очищати
                questionsArray[k].text = " ";
            }

            isFiftyFifty = false;
        }
    }

    public void HelpPublic()
    {
        if (isHelpPublic)
        {
            imHP.sprite = disableSpriteHP;
            isHelpPublic = false;
        }
    }

    public void CallFriend()
    {
        if (isCallFriend)
        {
            imCF.sprite = disableSpriteCF;
            isCallFriend = false;
        }
    }
        
    public void SelectDirectoryPath()
    {
        // відкриваємо меню для вибору теки з питаннями
        DirectoryInfo directoryInfo = new DirectoryInfo("Questions");
        files = directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories);

        filesListPan.SetActive(true);
        instancedObjects = new GameObject[files.Length];
        for (int i = 0; i < files.Length; i++)
        {
            FileScript file = Instantiate(filePrefab, filesContent.transform).GetComponent<FileScript>();
            file.fileNameText.text = files[i].Name;
            file.index = i;
            instancedObjects[i] = file.gameObject;
        }
    }

    public void SelectFolder(int folderIndex)
    {
        folderPath = files[folderIndex].FullName;

        filesListPan.SetActive(false);
        selectTextFile.gameObject.SetActive(false);
        toggleGroup.gameObject.SetActive(true);
        imFF.gameObject.SetActive(true);
        imHP.gameObject.SetActive(true);
        imCF.gameObject.SetActive(true);

        foreach (GameObject obj in instancedObjects)
            Destroy(obj);

        string[] filesTXT = Directory.GetFiles(folderPath);

        foreach (string file in filesTXT)
        {
            Debug.Log(file);
            if (file.Contains("lite"))
                liteQuestionList = file;
            if (file.Contains("middle"))
                middleQuestionList = file;
            if (file.Contains("hard"))
                hardQuestionList = file;
        }

        if (liteQuestionList != null)
        {
            LoadFileQuestion(liteQuestionList);

            numberQuestion = Random.Range(0, (amountLines / 5) - 1) * 5; // випадкове запитання

            PutQuestionToList();

            LoadQuestionAndAnswersToScreen();
        }
    }

    private void LoadFileQuestion(string file)
    {
        string allQuestionList = File.ReadAllText(file);

        eachLine = new List<string>();
        eachLine.AddRange(
                    allQuestionList.Split("\n"[0])); // текст в листі порядково

        amountLines = eachLine.Count; // кількість рядків

        listPastQuestions = new int[amountLines / 5]; // створюємо список для використаних запитань
        for (int j = 0; j < amountLines / 5; j++)
        {
            listPastQuestions[j] = -1; // заповнюємо список -1, щоб не мати проблем з питанням в рядку 0
        }
    }

    private void UpdateScene()
    {
        UpPrize();

        SetRandomQuestion();

        PutQuestionToList();

        LoadQuestionAndAnswersToScreen();
    }

    private void CheckAnswer(Toggle toggle)
    {
        //timerCount = 61;

        if (toggle.isOn)
        {
            int i = FindTrueAnswer();

            if (toggle == toggles[i])
            {
                UpdateScene();
            }
            else GameOver();

            toggle.isOn = false;
        }
    }

    private int FindTrueAnswer()
    {
        int i = 0;
        foreach (Text text in questionsArray)
        {
            if (eachLine[numberQuestion + 1].Equals(text.text))
            {
                Debug.Log("true " + i); // визначаємо правильну відповідь
                break;
            }
            else i++;
        }
        return i;
    }

    private void UpPrize()
    {
        if (numberOfPrize < 14)
        {
            prize[numberOfPrize].sprite = spritePrizes[numberOfPrize + 15];
            prize[numberOfPrize + 1].sprite = spritePrizes[numberOfPrize + 1];
            numberOfPrize++;

            if (numberOfPrize == 5)
                if (middleQuestionList != null)
                    LoadFileQuestion(middleQuestionList);

            if (numberOfPrize == 10)
                if (hardQuestionList != null)
                    LoadFileQuestion(hardQuestionList);
        }
        else
        {
            question.text = "Ви виграли";
            questionsArray[0].text = "";
            questionsArray[1].text = "";
            questionsArray[2].text = "";
            questionsArray[3].text = "";
            SceneManager.LoadScene(1);
        }
    }

    private void LoadQuestionAndAnswersToScreen()
    {
        question.text = eachLine[numberQuestion];
        int[] array = RandomIntArray(4);
        questionsArray[0].text = eachLine[numberQuestion + array[0]];
        questionsArray[1].text = eachLine[numberQuestion + array[1]];
        questionsArray[2].text = eachLine[numberQuestion + array[2]];
        questionsArray[3].text = eachLine[numberQuestion + array[3]];
    }

    private void SetRandomQuestion()
    {
        bool isQuestionInList = false;
        do
        {
            isQuestionInList = false;
            numberQuestion = Random.Range(0, amountLines / 5) * 5; // навмисно не уникаємо останнього числа поза масивом, щоб Random.Range міг видати останнє число у списку
            if (numberQuestion == amountLines / 5) // якщо Random.Range все ж видав число поза масивом, 
                numberQuestion = Random.Range(0, amountLines / 5) * 5; //то вигадуємо його ще раз.

            for (int i = 0; i < amountLines / 5; i++)
            {
                if (listPastQuestions[i] == numberQuestion) // якщо питання вже є у списку,
                {
                    isQuestionInList = true; // то почнемо повторення do-while знову.
                    break;
                }
                else isQuestionInList = false;
            }
        } while (isQuestionInList);
    }

    private void PutQuestionToList()
    {
        for (int i = 0; i < amountLines / 5; i++)
        {
            if (listPastQuestions[i] == -1) // якщо є вільне місце у списку
            {
                listPastQuestions[i] = numberQuestion; // то покласти номер питання у список

                if (i == (amountLines / 5) - 1) // якщо список повний
                {
                    Debug.Log("Запитання закінчились");
                    for (int j = 0; j < amountLines / 5; j++)
                    {
                        listPastQuestions[j] = -1; // то очищаємо список використаних запитань
                    }
                } else break; // якщо запитання покладено, то виходимо зі повторення, щоб не перевіряти решту списку
            }
        }
    }

    private int[] RandomIntArray(int count)
    {
            int[] array = new int[count];
            int k = 0;

            for (int n = count - 1; n > -1;)
            {
                bool loop = true;
                do
                {
                    k = (int)Random.Range(1, count + 1); // до count додано один, тому що Random.Range рідко видає останнє число, це тупить програму

                    int c = 0;
                    for ( ; c < count; c++)
                    {
                        if (array[c] == k)
                        {
                            c = 0;
                            break;
                        }
						if (k > count) break; // виловлюємо найбільше число, котре призведе до запиту неіснуючого місця в масиві
                    }
                    if (c == count)
                    {
                        array[n] = k;
                        loop = false;
                    }
                    else 
					{
                        k = (int)Random.Range(1, count);
					}

                } while (loop);

                --n;
            }
			
			// призначено для масиву розміром 4
			//array[0] = 10 - (array[1] + array[2] + array[3]); // це милиця для прискорення призначення останнього числа в масиві
			// для більших массивів варто визначити суму можливих неповторюваних чисел та відняти всі призначені числа

            return array;
    }
    
    private void GameOver()
    {
        Debug.Log("GameOver");
        question.text = "Ви програли";
        questionsArray[0].text = "";
        questionsArray[1].text = "";
        questionsArray[2].text = "";
        questionsArray[3].text = "";
        SceneManager.LoadScene(1);
    }

    void OnEnable()
    {
        //Register Toggle Events
        toggles[0].onValueChanged.AddListener(delegate { CheckAnswer(toggles[0]); });
        toggles[1].onValueChanged.AddListener(delegate { CheckAnswer(toggles[1]); });
        toggles[2].onValueChanged.AddListener(delegate { CheckAnswer(toggles[2]); });
        toggles[3].onValueChanged.AddListener(delegate { CheckAnswer(toggles[3]); });
    }

    void OnDisable()
    {
        //Un-Register Toggle Events
        toggles[0].onValueChanged.RemoveAllListeners();
        toggles[1].onValueChanged.RemoveAllListeners();
        toggles[2].onValueChanged.RemoveAllListeners();
        toggles[3].onValueChanged.RemoveAllListeners();
    }

    public void QuitGame()
    {
        // save any game data here
#if UNITY_EDITOR
         // Application.Quit() does not work in the editor so
         // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
         UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /*
    public void StartTimer()
    {
        imFF.gameObject.SetActive(true);
        imHP.gameObject.SetActive(true);
        imCF.gameObject.SetActive(true);
        toggleGroup.gameObject.SetActive(true);

        numberQuestion = Random.Range(0, (amountLines / 5) - 1) * 5; // випадкове запитання

        PutQuestionToList();

        LoadQuestionAndAnswersToScreen();

        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        while (timerCount > 0)
        {
            timerCount--;
            timerText.text = timerCount.ToString();
            timerImage.fillAmount = 0.0163f * timerCount;

            yield return new WaitForSeconds(1);
        }
        Debug.Log("Час вичерпано");

        GameOver();
    }*/
}
