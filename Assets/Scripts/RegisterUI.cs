using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RegisterUI : MonoBehaviour
{
    //public TMP_InputField usernameInput;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    //public TMP_Text messageText;

    public void Register()
    {
        //string username = usernameInput.text.Trim();
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();
        string confirmPassword = confirmPasswordInput.text.Trim();

        /*
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            messageText.text = "Completa todos los campos";
            return;
        }
        */

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            //messageText.text = "Completa todos los campos";
            return;
        }

        if (password != confirmPassword)
        {
            //messageText.text = "Las contraseñas no coinciden";
            return;
        }

        //messageText.text = "Cuenta creada";
        SceneManager.LoadScene(0);
    }
}