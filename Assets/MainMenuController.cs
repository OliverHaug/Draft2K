using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public TMP_Dropdown playerRatingDropdown; 
    public TMP_Dropdown numberOfPlayersDropdown;
    public TMP_Dropdown numberOfFormationsDropdown;

    public void StartDraft(){
        int playerRating = int.Parse(playerRatingDropdown.options[playerRatingDropdown.value].text);
        int numberOfPlayers = int.Parse(numberOfPlayersDropdown.options[numberOfPlayersDropdown.value].text);
        int numberOfFormations = int.Parse(numberOfFormationsDropdown.options[numberOfFormationsDropdown.value].text);

        Debug.Log("Go to the Draft");
    }
}
