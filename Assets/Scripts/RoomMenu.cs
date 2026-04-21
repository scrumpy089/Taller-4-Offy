using UnityEngine;

public class RoomMenu : MonoBehaviour
{
    public GameObject CanvaActive;
    public GameObject CanvaInactive;

    public void ActiveToInactive()
    {
        CanvaActive.SetActive(false);
        CanvaInactive.SetActive(true);
    }

    public void InactiveToActive()
    {
        CanvaInactive.SetActive(false);
        CanvaActive.SetActive(true);
    }
}