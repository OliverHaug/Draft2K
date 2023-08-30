using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    DataLoader dataLoader;

    void Start()
    {
        dataLoader = GameObject.FindFirstObjectByType<DataLoader>();
    }
    public void LoadRandomPlayers()
    {
        int minRating = 80;
        int minPlayers = 5;
        string[] playerNames = new string[0];
        dataLoader.GetRandomPlayers(minRating, minPlayers, playerNames);
    }
}
