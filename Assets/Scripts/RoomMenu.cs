using UnityEngine;

public class RoomMenu : MonoBehaviour
{
    public GameObject PreMenu;
    public GameObject Menu;
    public GameObject Comfirmacion;

    public void Start()
    {
        PreMenu.SetActive(true);
        Menu.SetActive(false);
        Comfirmacion.SetActive(false);
    }

    public void CreateRoom()
    {
        PreMenu.SetActive(false);
        Menu.SetActive(true);
    }

    public void ContinueMenu()
    {
        Menu.SetActive(false);
        Comfirmacion.SetActive(true);
    }

    public void BackToPreMenu()
    {
        Menu.SetActive(false);
        PreMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        Comfirmacion.SetActive(false);
        Menu.SetActive(true);
    }
}