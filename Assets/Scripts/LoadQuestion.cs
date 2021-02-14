using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class LoadQuestion : MonoBehaviour
{
    public static LoadQuestion instance;

    public Text question;
    public Text[] answerVariants;
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

    public Transform activeAnswer;

    private string[] eachLine;
    private string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";

    private int numberQuestion = 0;
    private int delayCheckAnswer = 2;
    private int trueAnswers = 0;
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

    public void LoadTable()
    {
        System.Action<string> callback = SplitStringDataToQuestions;

        StartCoroutine(LoadFromSheets.DownloadData(callback));
    }

    public void OnApplyClick()
    {
        LoadQuestionAndAnswersToScreen();

        applyButton.SetActive(false);
    }

    public void CheckAnswer(Text text)
    {
        if (isGameOver == false)
        {
            trueAnswer = FindTrueAnswer();
            
            numberOfChoise = int.Parse(text.name.ToString());
            
            if (trueAnswer == numberOfChoise)
            {
                answerButtons[trueAnswer].GetComponent<Image>().sprite = trueAnswerSprites[trueAnswer];
                Invoke(nameof(UpdateScene), delayCheckAnswer);
                numberQuestion += 5;
            }
            else
            {
                answerButtons[numberOfChoise].GetComponent<Image>().sprite = trueAnswerSprites[trueAnswer];

                Invoke(nameof(GameOver), delayCheckAnswer);
            }
        }
    }

    public int FindTrueAnswer()
    {
        int i = 0;
        foreach (Text text in answerVariants)
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
                answerVariants[k].text = " ";
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

    private void SplitStringDataToQuestions(string downloadData)
    {
        string allQuestionList = downloadData;

        allQuestionList = allQuestionList.Substring(31);

        string[] tempList = allQuestionList.Split('\n'); // текст в листі порядково

        eachLine = new string[tempList.Length * 5];

        for (int i = 0, index = 0; i < tempList.Length; i++)
        {
            var values = Regex.Split(tempList[i], SPLIT_RE);

            if (values.Length == 0 || values[0] == "") continue; // Якщо рядок пустий, то пропускаємо його

            for (int j = 0; j < values.Length; j++)
            {
                if (values[j].Contains("//") || values[j].Contains("*")) break;

                else
                {
                    eachLine[index] = values[j];
                    index++;
                }
            }
        }

        applyButton.SetActive(true);
    }

    private void LoadQuestionAndAnswersToScreen()
    {
        int[] array = RandomIntArray(4);

        question.text = eachLine[numberQuestion];
        
        answerVariants[0].text = eachLine[numberQuestion + array[0]];
        answerVariants[1].text = eachLine[numberQuestion + array[1]];
        answerVariants[2].text = eachLine[numberQuestion + array[2]];
        answerVariants[3].text = eachLine[numberQuestion + array[3]];

        for (int i = 0; i < 4; i++)
        {
            answerButtons[i].GetComponent<Image>().sprite = startAnswerSprites[i];
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
                for (; c < count; c++)
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

        //restartGame.SetActive(true);
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

        //restartGame.SetActive(true);
    }

    private void UpdateScene()
    {
        if (trueAnswers == 14)
            Victory();
        else
        {
            UpPrize();

            LoadQuestionAndAnswersToScreen();
        }
    }

    private void UpPrize()
    {
        if (trueAnswers < 14)
        {
            activeAnswer.position =
                new Vector3(activeAnswer.position.x, activeAnswer.position.y + upIndex, activeAnswer.position.z);

            trueAnswers++;

            if (trueAnswers == 5)
                prizePosition = activeAnswer.position.y - upIndex;
            if (trueAnswers == 10)
                prizePosition = activeAnswer.position.y - upIndex;
            if (trueAnswers == 13)
                prizePosition = activeAnswer.position.y - upIndex;
        }
        else
        {
            Victory();
        }
    }
}
