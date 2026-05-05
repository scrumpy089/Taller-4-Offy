using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ProfileSceneUI : MonoBehaviour
{
    [Header("Vista perfil")]
    public TMP_Text usernameText;
    public TMP_Text emailText;
    public TMP_Text birthdateText;
    public TMP_Text pointsText;
    public TMP_Text bestScoreText;
    public TMP_Text totalSessionsText;
    public Image profileImage;
    public Sprite defaultProfileSprite;

    [Header("Panel editar")]
    public GameObject editPanel;
    public TMP_InputField editUsernameInput;
    public TMP_InputField editBirthdateInput;
    public Image editProfileImage;

    [Header("Mensajes")]
    public GameObject messagePanel;
    public TMP_Text messageText;

    private string selectedImagePath = "";
    private string currentPhotoUrl = "";

    void Start()
    {
        editPanel.SetActive(false);
        messagePanel.SetActive(false);
        LoadProfile();
    }

    public void LoadProfile()
    {
        string uid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        FirebaseManager.Instance.DB
            .Collection("users")
            .Document(uid)
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    ShowMessage("Error cargando perfil", Color.red);
                    return;
                }

                var doc = task.Result;

                string username = doc.ContainsField("username") ? doc.GetValue<string>("username") : "Sin nombre";
                string email = doc.ContainsField("email") ? doc.GetValue<string>("email") : "";
                string birthdate = doc.ContainsField("birthdate") ? doc.GetValue<string>("birthdate") : "";
                currentPhotoUrl = doc.ContainsField("photoUrl") ? doc.GetValue<string>("photoUrl") : "";

                int points = doc.ContainsField("points") ? doc.GetValue<int>("points") : 0;
                int bestScore = doc.ContainsField("bestScore") ? doc.GetValue<int>("bestScore") : 0;
                int totalSessions = doc.ContainsField("totalSessions") ? doc.GetValue<int>("totalSessions") : 0;

                usernameText.text = username;
                emailText.text = email;
                birthdateText.text = birthdate;
                pointsText.text = "Puntos: " + points;
                bestScoreText.text = "Mejor puntaje: " + bestScore;
                totalSessionsText.text = "Sesiones: " + totalSessions;

                editUsernameInput.text = username;
                editBirthdateInput.text = birthdate;

                if (!string.IsNullOrEmpty(currentPhotoUrl))
                    StartCoroutine(LoadImageFromUrl(currentPhotoUrl, profileImage, editProfileImage));
                else
                {
                    profileImage.sprite = defaultProfileSprite;
                    editProfileImage.sprite = defaultProfileSprite;
                }
            });
    }

    public void OpenEditPanel()
    {
        editPanel.SetActive(true);
        messagePanel.SetActive(false);
    }

    public void CloseEditPanel()
    {
        editPanel.SetActive(false);
        selectedImagePath = "";
    }

    public void PickImageFromGallery()
    {
        NativeGallery.GetImageFromGallery(path =>
        {
            if (string.IsNullOrEmpty(path)) return;

            selectedImagePath = path;

            Texture2D texture = NativeGallery.LoadImageAtPath(path, 512, false);
            if (texture == null) return;

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            editProfileImage.sprite = sprite;
        }, "Selecciona una foto de perfil", "image/*");
    }

    public void SaveChanges()
    {
        string username = editUsernameInput.text.Trim();
        string birthdate = editBirthdateInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(birthdate))
        {
            ShowMessage("Completa nombre y fecha", Color.red);
            return;
        }

        if (string.IsNullOrEmpty(selectedImagePath))
        {
            SaveProfileData(username, birthdate, currentPhotoUrl);
        }
        else
        {
            UploadImageAndSave(username, birthdate);
        }
    }

    private void UploadImageAndSave(string username, string birthdate)
    {
        string uid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        StorageReference storageRef = FirebaseManager.Instance.Storage
            .GetReference("profilePictures/" + uid + ".jpg");

        storageRef.PutFileAsync(selectedImagePath).ContinueWithOnMainThread(uploadTask =>
        {
            if (uploadTask.IsFaulted || uploadTask.IsCanceled)
            {
                ShowMessage("Error subiendo foto", Color.red);
                Debug.LogError(uploadTask.Exception);
                return;
            }

            storageRef.GetDownloadUrlAsync().ContinueWithOnMainThread(urlTask =>
            {
                if (urlTask.IsFaulted || urlTask.IsCanceled)
                {
                    ShowMessage("Error obteniendo URL", Color.red);
                    Debug.LogError(urlTask.Exception);
                    return;
                }

                string photoUrl = urlTask.Result.ToString();
                SaveProfileData(username, birthdate, photoUrl);
            });
        });
    }

    private void SaveProfileData(string username, string birthdate, string photoUrl)
    {
        string uid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "username", username },
            { "birthdate", birthdate },
            { "photoUrl", photoUrl },
            { "hasProfile", true },
            { "hasUsername", true }
        };

        FirebaseManager.Instance.DB
            .Collection("users")
            .Document(uid)
            .UpdateAsync(data)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    ShowMessage("Perfil actualizado", Color.green);
                    editPanel.SetActive(false);
                    selectedImagePath = "";
                    LoadProfile();
                }
                else
                {
                    ShowMessage("Error guardando perfil", Color.red);
                    Debug.LogError(task.Exception);
                }
            });
    }

    IEnumerator LoadImageFromUrl(string url, Image mainImage, Image editImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            mainImage.sprite = defaultProfileSprite;
            editImage.sprite = defaultProfileSprite;
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        mainImage.sprite = sprite;
        editImage.sprite = sprite;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ShowMessage(string message, Color color)
    {
        messagePanel.SetActive(true);
        messageText.text = message;
        messageText.color = color;
    }

    public void ClearMessage()
    {
        messagePanel.SetActive(false);
        messageText.text = "";
    }
}