using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;

public class RegisterUI : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;

    public TMP_Text messageText;
    public GameObject PanelAviso;
    public GameObject loadingButton;
    public GameObject registerButton;

    public GameObject PanelRegister;
    public GameObject PanelWelcome;
    public GameObject PanelProfileSetup;

    private bool isRegistering = false;

    void Start()
    {
        PanelRegister.SetActive(true);
        PanelAviso.SetActive(false);
        PanelWelcome.SetActive(false);
        PanelProfileSetup.SetActive(false);

        registerButton.SetActive(false);
        loadingButton.SetActive(true);
    }

    void Update()
    {
        if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsReady)
        {
            registerButton.SetActive(true);
            loadingButton.SetActive(false);
        }
    }

    public void Register()
    {
        if (isRegistering) return;
        isRegistering = true;

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();
        string confirmPassword = confirmPasswordInput.text.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            ShowMessage("Completa todos los campos", Color.red);
            isRegistering = false;
            return;
        }

        if (password != confirmPassword)
        {
            ShowMessage("Las contraseñas no coinciden", Color.red);
            isRegistering = false;
            return;
        }

        if (password.Length < 6)
        {
            ShowMessage("La contraseña debe tener mínimo 6 caracteres", Color.red);
            isRegistering = false;
            return;
        }

        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsReady)
        {
            ShowMessage("Firebase aún está cargando", Color.red);
            isRegistering = false;
            return;
        }

        FirebaseManager.Instance.Auth
            .CreateUserWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    HandleRegisterError(task.Exception);
                    isRegistering = false;
                    return;
                }

                FirebaseUser user = task.Result.User;
                SaveUserProfile(user.UserId, email);
            });
    }

    private void SaveUserProfile(string uid, string email)
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "username", "Sin nombre" },
            { "email", email },
            { "points", 0 },
            { "bestScore", 0 },
            { "totalSessions", 0 },
            { "hasUsername", false }
        };

        FirebaseManager.Instance.DB
            .Collection("users")
            .Document(uid)
            .SetAsync(data)
            .ContinueWithOnMainThread(task =>
            {
                isRegistering = false;

                if (task.IsCompleted)
                {
                    PanelRegister.SetActive(false);
                    PanelAviso.SetActive(false);
                    PanelWelcome.SetActive(true);
                }
                else
                {
                    ShowMessage("Cuenta creada, pero falló el perfil", Color.red);
                    Debug.LogError(task.Exception);
                }
            });
    }

    private void HandleRegisterError(System.AggregateException exception)
    {
        string message = "No se pudo crear la cuenta";

        if (exception != null)
        {
            foreach (var inner in exception.Flatten().InnerExceptions)
            {
                FirebaseException firebaseEx = inner as FirebaseException;

                if (firebaseEx != null)
                {
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                    switch (errorCode)
                    {
                        case AuthError.EmailAlreadyInUse:
                            message = "Este correo ya está registrado";
                            break;

                        case AuthError.InvalidEmail:
                            message = "Correo inválido";
                            break;

                        case AuthError.WeakPassword:
                            message = "La contraseña debe tener mínimo 6 caracteres";
                            break;

                        default:
                            message = "No se pudo crear la cuenta";
                            break;
                    }
                }
            }
        }

        ShowMessage(message, Color.red);
        Debug.LogError(exception);
    }

    public void ShowMessage(string message, Color color)
    {
        PanelAviso.SetActive(true);
        messageText.text = message;
        messageText.color = color;
    }

    public void ClearMessage()
    {
        PanelAviso.SetActive(false);
        messageText.text = "";
    }

    public void GoToProfileSetup()
    {
        PanelWelcome.SetActive(false);
        PanelProfileSetup.SetActive(true);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}