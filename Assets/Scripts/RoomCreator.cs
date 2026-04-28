using UnityEngine;
using TMPro;

public class RoomCreator : MonoBehaviour
{
    public TMP_Dropdown questionDropdown;

    int[] options = { 4, 5, 6 };

    public void OnDropdownChanged()
    {
        int selectedIndex = questionDropdown.value;
        GameData.maxQuestions = options[selectedIndex];

        Debug.Log("Guardado: " + GameData.maxQuestions);
    }
}
