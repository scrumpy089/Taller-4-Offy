using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Extensions;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text messageText;
    public GameObject panel;

    void Start()
    {
        panel.SetActive(false);
    }

    public void Login()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            panel.SetActive(true);
            ShowMessage("Completa correo y contraseña", Color.red);
            return;
        }

        if (!FirebaseManager.Instance.IsReady)
        {
            ShowMessage("Firebase aún está cargando", Color.red);
            return;
        }

        FirebaseManager.Instance.Auth
            .SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    ShowMessage("Correo o contraseña incorrectos", Color.red);
                    Debug.LogError(task.Exception);
                    return;
                }

                SceneManager.LoadScene(2);
            });
    }

    public void ShowMessage(string message, Color color)
    {
        panel.SetActive(true);
        messageText.text = message;
        messageText.color = color;
    }

    public void ClearMessage()
    {
        panel.SetActive(false);
        messageText.text = "";
    }
}