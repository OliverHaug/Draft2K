using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;

public class DataLoader : MonoBehaviour
{
    private const string baseUrl = "https://draft2k.boundlessdev.de/";

    public PlayerModel[] GetRandomPlayers(int minRating, int numPlayers, string[] playerNames, string position = null)
    {
        string jsonRequest = JsonUtility.ToJson(new
        {
            minRating = minRating,
            numPlayers = numPlayers,
            playerNames = playerNames,
            position = position
        });

        string api = baseUrl + "random_players";

        StartCoroutine(SendRandomPlayersRequest(api, jsonRequest));

        return null;
    }

    private IEnumerator SendRandomPlayersRequest(string api, string jsonRequest)
    {
        using (UnityWebRequest request = new UnityWebRequest(api, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequest);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Fehler beim Senden des Requests: " + request.error);
                yield break;
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                PlayerModel[] randomPlayers = JsonUtility.FromJson<PlayerModel[]>(responseJson);

                foreach (PlayerModel player in randomPlayers)
                {
                    Debug.Log(player.name);
                }
            }
        }

    }
}
