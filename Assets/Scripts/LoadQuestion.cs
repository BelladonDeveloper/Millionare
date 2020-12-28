using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class LoadQuestion : MonoBehaviour
{
    public GameObject filesListPan, filesContent, filePrefab;
    public static LoadQuestion instance;

    public Text question;
    public Text[] questionsArray;
    public GameObject restartGame;
    public GameObject logo;
    public GameObject answerButtonGroup;
    public GameObject applyButton;
    public GameObject[] answerButtons;
    public Sprite[] startAnswerSprites;
    public Sprite[] trueAnswerSprites;
    public Sprite[] falseAnswerSprites;

    public Image imFF;
    public Sprite disableSpriteFF;
    public Image imHP;
    public Sprite disableSpriteHP;
    public Image imCF;
    public Sprite disableSpriteCF;

    public RawImage selectTextFile;

    public Transform activeAnswer;

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
    private string[] questionArrays;

    private int amountLines;
    private int numberQuestion = 0;
    private int delayCheckAnswer = 2;
    private int[] listPastQuestions;
    private int trueAnswers = 0;
    private int folderIndex = 0;
    private int trueAnswer;
    private int numberOfChoise;

    private float upIndex = 0.30f;
    private float prizePosition;

    private bool isFiftyFifty = true;
    private bool isHelpPublic = true;
    private bool isCallFriend = true;
	private bool isGameOver = false;
    
    void Start()
    {
        prizePosition = activeAnswer.position.y;
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

        logo.SetActive(false);
        answerButtonGroup.SetActive(false);
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
        logo.SetActive(true);
        answerButtonGroup.SetActive(true);

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
            //LoadFileQuestion(liteQuestionList);

            numberQuestion = Random.Range(0, (amountLines / 5) - 1) * 5; // випадкове запитання

            PutQuestionToList();

            LoadQuestionAndAnswersToScreen();
        }
    }

    private void LoadFileQuestion(string file, int index)
    {
        string allQuestionList = file; //File.ReadAllText(file);
        if (index == 0)
            allQuestionList = allQuestionList.Substring(18);
        else
            allQuestionList = allQuestionList.Substring(17);

        string[] tempList = allQuestionList.Split('\n');

        eachLine = new List<string>();

        int lengthList = tempList.Length;
        if (index == 4)
            lengthList++;
        for (int i = 0; i < tempList.Length - 1; i++)
        {
            Debug.Log(tempList[i]);
            eachLine.AddRange(tempList[i].Split(',')); //"\n"[0])); // текст в листі порядково
        }
        foreach (var item in eachLine)
        {
            item.Replace('.', ',');
        }
        amountLines = eachLine.Count; // кількість рядків

        listPastQuestions = new int[amountLines / 5]; // створюємо список для використаних запитань
        for (int j = 0; j < amountLines / 5; j++)
        {
            listPastQuestions[j] = -1; // заповнюємо список -1, щоб не мати проблем з питанням в рядку 0
        }
    }

    private void UpdateScene()
    {
        if (trueAnswers == 14)
            Victory();
        else
        {
            UpPrize();

            SetRandomQuestion();

            PutQuestionToList();

            LoadQuestionAndAnswersToScreen();
        }
    }

    public void CheckAnswer(Text text)
    {
		if (isGameOver == false)
		{
			int trueAnswer = FindTrueAnswer();
			numberOfChoise = int.Parse(text.name.ToString());
			if (trueAnswer == numberOfChoise)
			{
				answerButtons[trueAnswer].GetComponent<Image>().sprite = trueAnswerSprites[trueAnswer];
				Invoke("UpdateScene", delayCheckAnswer);
			}
			else
			{
				answerButtons[numberOfChoise].GetComponent<Image>().sprite = trueAnswerSprites[trueAnswer];

				Invoke("GameOver", delayCheckAnswer);
			}
		}
		
        //timerCount = 61;
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
        if (trueAnswers < 14)
        {
            activeAnswer.position =
                new Vector3(activeAnswer.position.x, activeAnswer.position.y + upIndex, activeAnswer.position.z);

            trueAnswers++;
            Debug.Log(trueAnswers);

            if (trueAnswers == 3)
                if (questionArrays[1] != null)
                {
                    LoadFileQuestion(questionArrays[1], 1);
                }
            if (trueAnswers == 5)
                prizePosition = activeAnswer.position.y - upIndex;

            if (trueAnswers == 6)
                if (questionArrays[2] != null)
                {
                    LoadFileQuestion(questionArrays[2], 2);
                }
            if (trueAnswers == 9)
                if (questionArrays[3] != null)
                {
                    LoadFileQuestion(questionArrays[3], 3);
                }
            if (trueAnswers == 10)
                prizePosition = activeAnswer.position.y - upIndex;
            if (trueAnswers == 12)
                if (questionArrays[4] != null)
                {
                    LoadFileQuestion(questionArrays[4], 4);
                }
            if (trueAnswers == 13)
                prizePosition = activeAnswer.position.y - upIndex;
        }
        else
        {
            Victory();
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
        for(int i = 0; i < 4; i++)
        {
            answerButtons[i].GetComponent<Image>().sprite = startAnswerSprites[i];
        }
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
                } else break; // якщо запитання покладено, то виходимо з повторення, щоб не перевіряти решту списку
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
    
    private void Victory()
    {
        question.text = "Ви виграли 1 000 000";

        restartGame.SetActive(true);
    }

    private void GameOver()
    {
        answerButtons[trueAnswer].GetComponent<Image>().sprite = trueAnswerSprites[trueAnswer];

        answerButtons[numberOfChoise].GetComponent<Image>().sprite = falseAnswerSprites[numberOfChoise];

		isGameOver = true;
        Debug.Log("GameOver");
		
        switch (trueAnswers)
        {
            case int n when (n < 4):
                {
                    question.text = "Ви програли";
                    break;
                }
            case int n when (n > 4 && n < 10):
                {
                    question.text = "Ви виграли 1 000";
                    break;
                }
            case int n when (n > 9 && n < 15):
                {
                    question.text = "Ви виграли 32 000";
                    break;
                }
        }
        activeAnswer.position =
            new Vector3(activeAnswer.position.x, prizePosition, activeAnswer.position.z);

        restartGame.SetActive(true);
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

    public void SelectTable(string ID) //Deprecated
    {
        LoadFromSheets.googleSheetDocID = ID;
    }

    public void OnApplyClick()
    {
        applyButton.SetActive(false);

        System.Action<string> data = null;

        StartCoroutine(LoadFromSheets.DownloadData(data));

        string downloadData = PlayerPrefs.GetString("LastDataDownloaded", null);

        questionArrays = downloadData.Split('*');

        if (questionArrays[0] != null)
        {
            LoadFileQuestion(questionArrays[0], 0);
        }

        numberQuestion = Random.Range(0, (amountLines / 5) - 1) * 5; // випадкове запитання

        PutQuestionToList();

        LoadQuestionAndAnswersToScreen();
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
