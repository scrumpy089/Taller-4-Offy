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

    void Start()
    {
        generateQuestion();
    }

    public void Answer(bool isCorrect)
    {
        canvasManager.OpenCanvaPopup();

        // Texto
        feedbackText.text = isCorrect ? "Correcto" : "Incorrecto";
        feedbackText.color = isCorrect ? Color.green : Color.red;

        // Imagen del personaje
        if (isCorrect)
        {
            characterImage.sprite = correctSprite;
        }
        else
        {
            characterImage.sprite = wrongSprite;
        }

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
        QuestionTxt.text = QnA[currentQuestion].Questions;
        setAnswers();
    }
}