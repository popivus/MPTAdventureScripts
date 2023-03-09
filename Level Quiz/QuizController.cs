using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class QuizController : MonoBehaviour
{
    [SerializeField] private GameObject cameraCutsceneStart, cameraCutsceneEnd;
    [SerializeField] private GameObject[] succesMarkers, failMarkers, answersBoxes;
    [SerializeField] private TextMeshPro timerText;
    [SerializeField] private TextMeshPro[] answerTexts;
    [Space]
    [SerializeField] private TextMeshPro questionText;
    [SerializeField] private Material successMaterial, failMaterial;
    private int correctAnswersCount = 0, failAnswersCount = 0;

    private List<Question> questions;
    private System.Random random;
    private Question newQuestion;
    
    void Start()
    {
        random = new System.Random();
        timer = QuestionTimer(20);
        questions = new List<Question>
        {
            new Question("Сколько весит тип данных int в языке C#?",
                new List<string>
                {
                    "1 байт",
                    "2 байта",
                    "4 байта"
                },2),
            new Question("Сколько весит тип данных short в языке C#?",
                new List<string>
                {
                    "1 байт",
                    "2 байта",
                    "4 байта"
                },1),
            new Question("Сколько весит тип данных long в языке C#?",
                new List<string>
                {
                    "4 байтa",
                    "8 байтов",
                    "16 байтов"
                },1),
            new Question("Сколько весит тип данных float в языке C#?",
                new List<string>
                {
                    "1 байт",
                    "2 байта",
                    "4 байта"
                },2),
            new Question("Сколько весит тип данных double в языке C#?",
                new List<string>
                {
                    "4 байтa",
                    "8 байтов",
                    "16 байтов"
                },1),
            new Question("Сколько весит тип данных decimal в языке C#?",
                new List<string>
                {
                    "4 байтa",
                    "8 байтов",
                    "16 байтов"
                },2),
            new Question("Какое количество предметов умещается в стак в игре Minecraft?",
                new List<string>
                {
                    "64",
                    "50",
                    "96"
                },0),
            new Question("Что делает оператор \"%\" в языке C#?",
                new List<string>
                {
                    "Возвращает остаток от деления",
                    "Возвращает процент от суммы",
                    "Возвращает тригонометрическую функцию"
                },0),
            new Question("2 + 2 * 2 = ?",
                new List<string>
                {
                    "4",
                    "6",
                    "8"
                },1),
            new Question("Сколько весит тип данных double в языке C#?",
                new List<string>
                {
                    "4 байтa",
                    "8 байтов",
                    "16 байтов"
                },1),
            new Question("Как называлась первая ЭВМ в СССР?",
                new List<string>
                {
                    "МЭСМ",
                    "Стрела",
                    "IBM PC"
                },0),
            new Question("В каком году появился язык программирования Java?",
                new List<string>
                {
                    "1992",
                    "1995",
                    "2000"
                },1),
            new Question("Какого цвета коров не существует?",
                new List<string>
                {
                    "Чёрных",
                    "Белых",
                    "Прозрачных"
                },2),
            new Question("В каком году началась Вторая Мировая Война?",
                new List<string>
                {
                    "1939",
                    "1941",
                    "1945"
                },0),
            new Question("Кто не был героем мультфильма \"Ледниковый период\"?",
                new List<string>
                {
                    "Тигр Диего",
                    "Ленивец Саид",
                    "Мамонт Мэнни"
                },1),
            new Question("8526 * 3027 = ?",
                new List<string>
                {
                    "25 808 202",
                    "24 712 532",
                    "22 879 748"
                },0),
            new Question("В каком году была создана социальная сеть \"ВКонтакте\"?",
                new List<string>
                {
                    "2002",
                    "2006",
                    "2005"
                },1),
            new Question("Какое животное в середине ХХ века помогало определять беременность женщин?",
                new List<string>
                {
                    "Волк",
                    "Лиса",
                    "Жаба"
                },2)
        };
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Здравствуй, студент!", "Преподаватель", 2f),
            new Speech("Пройди мой тест и получишь студенческий билет!", "Преподаватель", 5f),
            new Speech("Но смотри, после трёх неправильных ответов ты проиграешь!", "Преподаватель", 6f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cameraCutsceneStart, speeches, $"Пройдите тест (0/3)"));
        StartCoroutine(FirstQuestion(14f));
    }

    private IEnumerator FirstQuestion(float delay)
    {
        yield return new WaitForSeconds(delay);
        NewQuestion();
    }

    private void NewQuestion()
    {
        foreach (GameObject answerBox in answersBoxes) answerBox.tag = "Answer";
        newQuestion = questions[random.Next(0, questions.Count - 1)];
        questionText.text = newQuestion.question;
        for (int i = 0; i < newQuestion.answers.Count; i++) answerTexts[i].text = $"{i+1}) {newQuestion.answers[i]}";
        timer = QuestionTimer(20);
        StartCoroutine(timer);
    }

    public void Answer(int number)
    {
        foreach (GameObject answerBox in answersBoxes) answerBox.tag = "Untagged";
        StopCoroutine(timer);
        if (newQuestion.correctAnswerNumber == number)
        {
            StartCoroutine(CorrectAnswer());
        }
        else
        {
            StartCoroutine(FailAnswer("Неправильный ответ!"));
        }
    }

    private IEnumerator CorrectAnswer()
    {
        AudioManager.instance.PlaySound(1);
        succesMarkers[correctAnswersCount].GetComponent<Renderer>().material = successMaterial;
        correctAnswersCount++;
        UIController.instance.objective.GetComponent<TextMeshProUGUI>().text = $"Пройдите тест ({correctAnswersCount}/3)";
        var color = questionText.color;
        questionText.text = "Правильный ответ!";
        questionText.color = new Color(0, 1, 0);
        yield return new WaitForSeconds(3f);
        questionText.color = color;
        if (correctAnswersCount != 3) NewQuestion();
        else EndLevel();
    }

    private IEnumerator FailAnswer(string reason)
    {
        AudioManager.instance.PlaySound(2);
        failMarkers[failAnswersCount].GetComponent<Renderer>().material = failMaterial;
        failAnswersCount++;
        var color = questionText.color;
        questionText.text = reason;
        questionText.color = new Color(1, 0, 0);
        yield return new WaitForSeconds(3f);
        questionText.color = color;
        if (failAnswersCount != 3) NewQuestion();
        else StartCoroutine(RestartLevel());
    }

    private IEnumerator RestartLevel()
    {
        questionText.color = new Color(1, 0, 0);
        questionText.text = "Вы проиграли!";
        yield return new WaitForSeconds(3f);
        UIController.instance.fadeToBlack = true;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(9);
    }

    private void EndLevel()
    {
        PlayerPrefs.SetInt("Level9", 1);
        PlayerPrefs.SetInt("Last Level", 7);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Так держать!", "Преподаватель", 1.5f),
            new Speech("Держи студенческий билет!", "Преподаватель", 4.5f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cameraCutsceneEnd, speeches, "", 2));
    }

    private IEnumerator timer;
    private IEnumerator QuestionTimer(int seconds)
    {
        timerText.text = seconds.ToString();
        for (int s = 0; s < seconds; s++)
        {
            timerText.text = (seconds - s).ToString();
            yield return new WaitForSeconds(1f);
        }
        foreach (GameObject answerBox in answersBoxes) answerBox.tag = "Untagged";
        StartCoroutine(FailAnswer("Время вышло!"));
    }

    private class Question
    {
        public Question(string question, List<string> answers, int correctAnswerNumber)
        {
            this.question = question;
            this.answers = answers;
            this.correctAnswerNumber = correctAnswerNumber;
        }

        public string question {get;set;}
        public List<string> answers {get;set;}
        public int correctAnswerNumber {get;set;}
    }

    public static QuizController instance;
    public void Awake()
    {
        instance = this;
    }
}
