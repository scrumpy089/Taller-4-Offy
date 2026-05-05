using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using Firebase.Storage;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ProfileSetupUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField birthdateInput;
    public Image profileImage;

    public GameObject Panel1;
    public GameObject Panel2;

    private string selectedImagePath = "";
    private string uploadedPhotoUrl = "";

    public void Start()
    {
        Panel1.SetActive(true);
        Panel2.SetActive(false);
    }

    public void PickImageFromGallery()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (string.IsNullOrEmpty(path)) return;

            selectedImagePath = path;

            Texture2D texture = NativeGallery.LoadImageAtPath(path, 512, false);
            if (texture == null) return;

            profileImage.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }, "Selecciona una foto", "image/*");
    }

    public void SaveProfile()
    {
        string username = usernameInput.text.Trim();
        string birthdate = birthdateInput.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(birthdate))
        {
            Debug.Log("Completa nombre y fecha");
            return;
        }

        if (string.IsNullOrEmpty(selectedImagePath))
        {
            SaveProfileData(username, birthdate, "");
        }
        else
        {
            UploadProfileImage(username, birthdate);
        }
    }

    private void UploadProfileImage(string username, string birthdate)
    {
        string uid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        StorageReference storageRef = FirebaseManager.Instance.Storage.GetReference(
            "profilePictures/" + uid + ".jpg"
        );

        storageRef.PutFileAsync(selectedImagePath).ContinueWithOnMainThread(uploadTask =>
        {
            if (uploadTask.IsFaulted || uploadTask.IsCanceled)
            {
                Debug.LogError("Error subiendo foto: " + uploadTask.Exception);
                return;
            }

            storageRef.GetDownloadUrlAsync().ContinueWithOnMainThread(urlTask =>
            {
                if (urlTask.IsFaulted || urlTask.IsCanceled)
                {
                    Debug.LogError("Error obteniendo URL: " + urlTask.Exception);
                    return;
                }

                uploadedPhotoUrl = urlTask.Result.ToString();
                SaveProfileData(username, birthdate, uploadedPhotoUrl);
            });
        });
    }

    public void TakePhoto()
    {
        NativeCamera.TakePicture((path) =>
        {
            if (path == null) return;

            Texture2D texture = NativeGallery.LoadImageAtPath(path, 512);
            if (texture == null) return;

            profileImage.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
        }, 512);
    }

    private void SaveProfileData(string username, string birthdate, string photoUrl)
    {
        string uid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "username", username },
            { "birthdate", birthdate },
            { "photoUrl", photoUrl },
            { "hasUsername", true },
            { "hasProfile", true }
        };

        FirebaseManager.Instance.DB
            .Collection("users")
            .Document(uid)
            .UpdateAsync(data)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    SceneManager.LoadScene("MenuScene");
                }
                else
                {
                    Debug.LogError("Error guardando perfil: " + task.Exception);
                }
            });
    }

    public void nextPanel()
    {
        Panel1.SetActive(false);
        Panel2.SetActive(true);
    }
}