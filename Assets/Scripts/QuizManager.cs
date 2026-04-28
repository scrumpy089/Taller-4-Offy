using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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

    private bool timeOutTriggered = false;

    void Start()
    {
        GameData.Reset();
        GameData.totalQuestions = Mathf.Min(GameData.maxQuestions, QnA.Count);
        Debug.Log("MaxQuestions al iniciar: " + GameData.maxQuestions);

        generateQuestion();
    }

    void Update()
    {
        // Timer
        if (!canAnswer) return;

        timeLeft -= Time.deltaTime;
        
        if (timerText != null)
            timerText.text = Mathf.Ceil(timeLeft).ToString();

        // Cambiar color del timer a rojo si quedan 5 segundos o menos
        if (timeLeft <= 5)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }

        // Evitar múltiples triggers de timeout
        if (timeLeft <= 0 && !timeOutTriggered)
        {
            timeOutTriggered = true;
            StartCoroutine(TimeOutRoutine());
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

        // Si ya no hay preguntas → cambiar escena (asesinado este loco)
        // Ahora si las preguntas correctas e incorrectas equivalen a las totales se cambia la escena
        if (GameData.correctAnswers + GameData.wrongAnswers >= GameData.totalQuestions)
        {
            SceneManager.LoadScene("ResultsScene");
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
        timeOutTriggered = false;
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
        float percentage = timeLeft / questionTime;

        int minPoints = 50;
        int maxPoints = 100;

        return Mathf.RoundToInt(Mathf.Lerp(minPoints, maxPoints, percentage));
    }

    IEnumerator TimeOutRoutine()
    {
        Answer(false);

        yield return new WaitForSeconds(1.5f);

        NextQuestion();
    }
}