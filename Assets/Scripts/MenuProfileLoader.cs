using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using UnityEngine.Networking;
using System.Collections;

public class MenuProfileLoader : MonoBehaviour
{
    public TMP_Text usernameText;
    public Image profileImage;
    public Sprite defaultProfileSprite;

    void Start()
    {
        LoadProfile();
    }

    void LoadProfile()
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsReady)
        {
            Debug.LogError("Firebase no está listo");
            return;
        }

        string uid = FirebaseManager.Instance.Auth.CurrentUser.UserId;

        FirebaseManager.Instance.DB
            .Collection("users")
            .Document(uid)
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error cargando perfil: " + task.Exception);
                    return;
                }

                var doc = task.Result;

                if (!doc.Exists)
                {
                    Debug.LogError("No existe perfil del usuario");
                    return;
                }

                string username = doc.ContainsField("username")
                    ? doc.GetValue<string>("username")
                    : "Sin nombre";

                string photoUrl = doc.ContainsField("photoUrl")
                    ? doc.GetValue<string>("photoUrl")
                    : "";

                usernameText.text = username;

                if (!string.IsNullOrEmpty(photoUrl))
                {
                    StartCoroutine(LoadProfileImage(photoUrl));
                }
                else
                {
                    profileImage.sprite = defaultProfileSprite;
                }
            });
    }

    IEnumerator LoadProfileImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error cargando imagen: " + request.error);
            profileImage.sprite = defaultProfileSprite;
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);

        profileImage.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }
}