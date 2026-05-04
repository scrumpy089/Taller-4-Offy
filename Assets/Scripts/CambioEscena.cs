using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public int destino; // Numero de la escena a cargar

    public void CambiarEscena()
    {
        SceneManager.LoadScene(destino);
    }

    public void IrAMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void IrAPerfil()
    {
        SceneManager.LoadScene("ProfileScene");
    }

    public void IrAListaAmigos()
    {
        SceneManager.LoadScene("FriendListScene");
    }

    public void IrARacha()
    {
        SceneManager.LoadScene("StreakScene");
    }

    public void IrASalas()
    {
        SceneManager.LoadScene("RoomCreateScene"); // Carga la escena de salas
    }

    public void IrAUnirseSala()
    {
        SceneManager.LoadScene("RoomJoinScene"); 
    }

    public void IrAIniciarJuego()
    {
        SceneManager.LoadScene("SessionScene");
    }

    public void IrAQuiz()
    {
        SceneManager.LoadScene("QuizScene");
    }

    public void IrAResultados()
    {
        SceneManager.LoadScene("ResultsScene");
    }

}
