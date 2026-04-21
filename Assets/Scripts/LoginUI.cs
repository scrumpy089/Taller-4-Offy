using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginUI : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    //public TMP_Text messageText;

    public void Login()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();


        //PASA A MENU TENIENDO LOS CAMPOS VACIOS, HAY QUE VALIDAR QUE NO ESTEN VACIOS

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            //messageText.text = "Completa correo y contraseña";
            return;
        }

        //messageText.text = "Ingreso correcto";
        SceneManager.LoadScene(2);
    }
}