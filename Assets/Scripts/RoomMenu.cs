using UnityEngine;

public class RoomMenu : MonoBehaviour
{
    public GameObject CanvaPreMenu;
    public GameObject CanvaMenu;

    public void CreateRoom()
    {
        CanvaPreMenu.SetActive(false);
        CanvaMenu.SetActive(true);
    }

    public void BackToPreMenu()
    {
        CanvaPreMenu.SetActive(true);
        CanvaMenu.SetActive(false);
    }
}