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
        SceneManager.LoadScene(2);
    }

    public void IrAPerfil()
    {
        SceneManager.LoadScene(3);
    }

    public void IrAListaAmigos()
    {
        SceneManager.LoadScene(4);
    }

    public void IrARacha()
    {
        SceneManager.LoadScene(5);
    }

    public void IrASalas()
    {
        SceneManager.LoadScene(6); // Carga la escena de salas
    }

    public void IrAUnirseSala()
    {
        SceneManager.LoadScene(7); 
    }

    public void IrAInicioSesion()
    {
        SceneManager.LoadScene(8);
    }



}
