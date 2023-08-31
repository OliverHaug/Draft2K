using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    [SerializeField] private List<GameObject> playerCards;
    [SerializeField] private GameObject loadingText;
    [SerializeField] private DataLoader dataLoader;
    private List<PlayerModel> randomPlayers;

    public void StartPlayerSelection(int minRating, int minPlayers, string[] playerNames, string position = null)
    {

        loadingText.SetActive(true);
        dataLoader.GetRandomPlayers(minRating, minPlayers, playerNames, position, (players) =>
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

                Button playerButton = playerCards[i].GetComponent<Button>();
                if (playerButton != null)
                {
                    int playerIndex = i;
                    playerButton.onClick.AddListener(() => OnPlayerClick(playerIndex));
                }
            }
        }
    }

    private void OnPlayerClick(int playerIndex)
    {
        FieldHandler fieldHandler = GameObject.FindFirstObjectByType<FieldHandler>();
        if (fieldHandler != null)
        {
            fieldHandler.PlacePlayerAtPosition(randomPlayers[playerIndex]);

        }
    }
}
