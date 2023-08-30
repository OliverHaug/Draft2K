using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    [SerializeField] private List<GameObject> playerCards;
    [SerializeField] private GameObject loadingText;
    DataLoader dataLoader;
    private List<PlayerModel> randomPlayers;

    void Start()
    {
        dataLoader = GameObject.FindFirstObjectByType<DataLoader>();
    }
    public void LoadRandomPlayers()
    {
        int minRating = 80;
        int minPlayers = 5;
        string[] playerNames = new string[0];
        //StartCoroutine(LoadRandomPlayersCoroutine(minRating, minPlayers, playerNames));
        loadingText.SetActive(true);
        dataLoader.GetRandomPlayers(minRating, minPlayers, playerNames, null, (players) =>
        {
            randomPlayers = new List<PlayerModel>(players);

            StartCoroutine(LoadImagesAndShowPlayers());
        });
    }

    private IEnumerator LoadImagesAndShowPlayers()
    {
        foreach (PlayerModel player in randomPlayers)
        {
            string playerImagePath = Path.Combine(Application.persistentDataPath, "images", "player", Path.GetFileName(player.image));
            string nationImagePath = Path.Combine(Application.persistentDataPath, "images", "nation", Path.GetFileName(player.nation.image));
            string leagueImagePath = Path.Combine(Application.persistentDataPath, "images", "league", Path.GetFileName(player.club.league.image));
            string clubImagePath = Path.Combine(Application.persistentDataPath, "images", "club", Path.GetFileName(player.club.image));
            string cardImagePath = Path.Combine(Application.persistentDataPath, "images", "card", Path.GetFileName(player.card.image));

            bool imagesDownloaded = File.Exists(playerImagePath) && File.Exists(nationImagePath) && File.Exists(leagueImagePath) && File.Exists(clubImagePath) && File.Exists(cardImagePath);

            if (!imagesDownloaded)
            {
                yield return new WaitUntil(() =>
                    File.Exists(playerImagePath) &&
                    File.Exists(nationImagePath) &&
                    File.Exists(leagueImagePath) &&
                    File.Exists(clubImagePath) &&
                    File.Exists(cardImagePath)
                );
            }
        }

        ShowPlayersCard();

        loadingText.SetActive(false);
    }


    private void ShowPlayersCard()
    {
        for (int i = 0; i < playerCards.Count; i++)
        {
            if (i < randomPlayers.Count && playerCards[i] != null)
            {
                playerCards[i].GetComponent<Card>().CreateCard(randomPlayers[i]);
                playerCards[i].SetActive(true);
            }
        }
    }
}
