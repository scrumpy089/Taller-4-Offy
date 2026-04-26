using UnityEngine;

public class CanvaManagement : MonoBehaviour
{
    public GameObject canvaMain;
    public GameObject canvaQuiz;
    public GameObject canvaPopup;
    void Start()
    {
        canvaMain.SetActive(true);
        canvaQuiz.SetActive(false);
        canvaPopup.SetActive(false);
    }
    public void OpenCanvaQuiz()
    {
        canvaQuiz.SetActive(true);
    }
    public void CloseCanvaQuiz()
    {
        canvaQuiz.SetActive(false);
    }
    public void OpenCanvaPopup()
    {
        canvaPopup.SetActive(true);
    }    
    public void CloseCanvaPopup()
    {
        canvaPopup.SetActive(false);
    }

}
