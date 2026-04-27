using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public List<QuestionsAndAnswers> QnA;
    public GameObject[] options;
    public int currentQuestion;

    public TMP_Text QuestionTxt;
    public TMP_Text feedbackText;

    public Image characterImage;
    public Sprite correctSprite;
    public Sprite wrongSprite;

    public CanvaManagement canvasManager;

    // Timer
    public float questionTime = 15f;
    private float timeLeft;
    private bool canAnswer = true;

    public TMP_Text timerText;

    void Start()
    {
        GameData.Reset();
        GameData.totalQuestions = QnA.Count;

        generateQuestion();
    }

    void Update()
    {
        // Timer
        if (!canAnswer) return;

        timeLeft -= Time.deltaTime;

        if (timerText != null)
            timerText.text = Mathf.Ceil(timeLeft).ToString();

        if (timeLeft <= 0)
        {
            canAnswer = false;
            Answer(false);
        }
    }

    public void Answer(bool isCorrect)
    {
        // Evitar múltiples respuestas
        if (!canAnswer) return;
        canAnswer = false;

        canvasManager.OpenCanvaPopup();

        // Actualizar datos y mostrar feedback
        if (isCorrect)
        {
            // Incrementar respuestas correctas y calcular puntos
            GameData.correctAnswers++;

            int points = CalculatePoints();
            GameData.score += points;

            feedbackText.text = "Correcto +" + points + " pts";
        }
        else
        {
            // Incrementar respuestas incorrectas
            GameData.wrongAnswers++;
            feedbackText.text = "Incorrecto";
        }

        // Cambiar color del texto y sprite del personaje
        feedbackText.color = isCorrect ? Color.green : Color.red;
        characterImage.sprite = isCorrect ? correctSprite : wrongSprite;

        // Eliminar pregunta actual para no repetirla
        QnA.RemoveAt(currentQuestion);
    }

    public void NextQuestion()
    {
        canvasManager.CloseCanvaPopup();

        // Si ya no hay preguntas → cambiar escena
        if (QnA.Count == 0)
        {
            SceneManager.LoadScene("ResultsScene"); // 👈 cambia esto
            return;
        }

        generateQuestion();
    }

    void setAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<TMP_Text>().text =
                QnA[currentQuestion].Answers[i];

            if (QnA[currentQuestion].CorrectAnswer == i + 1)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = true;
            }
        }
    }

    void generateQuestion()
    {
        currentQuestion = Random.Range(0, QnA.Count);

        // Reiniciar timer
        timeLeft = questionTime;
        canAnswer = true;

        QuestionTxt.text = QnA[currentQuestion].Questions;
        setAnswers();
    }

    // Calcula puntos basados en el tiempo restante
    int CalculatePoints()
    {
        float percentage = (timeLeft + 2f) / questionTime;

        int minPoints = 50;
        int maxPoints = 100;

        return Mathf.RoundToInt(Mathf.Lerp(minPoints, maxPoints, percentage));
    }
}