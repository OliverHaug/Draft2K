using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.Universal;
using Newtonsoft.Json;
using System.IO;

[System.Serializable]
public class RandomPlayersRequest
{
    public int minRating;
    public int numPlayers;
    public string[] playerNames;
    public string position;
}

public class DataLoader : MonoBehaviour
{
    private const string baseUrl = "https://draft2k.boundlessdev.de";

    public void GetRandomPlayers(int minRating, int numPlayers, string[] playerNames, string position = null, Action<PlayerModel[]> callback = null)
    {
        RandomPlayersRequest requestData = new RandomPlayersRequest();
        requestData.minRating = minRating;
        requestData.numPlayers = numPlayers;
        requestData.playerNames = playerNames;
        requestData.position = position;

        string jsonRequest = JsonUtility.ToJson(requestData);

        string api = baseUrl + "/random_players";

        StartCoroutine(SendRandomPlayersRequest(api, jsonRequest, callback));
    }

    private IEnumerator SendRandomPlayersRequest(string api, string jsonRequest, Action<PlayerModel[]> callback)
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
                List<PlayerModel> randomPlayers = JsonConvert.DeserializeObject<List<PlayerModel>>(responseJson);

                foreach (PlayerModel player in randomPlayers)
                {
                    string _savePath = Application.persistentDataPath + "/images";
                    string _imageUrl = baseUrl + "/images";



                    string savePlayerImagePath = Path.Combine(_savePath, "player", player.image);
                    string playerImageFolder = Path.GetDirectoryName(savePlayerImagePath);
                    string playerImageUrl = _imageUrl + "/player/" + player.image;
                    if (!File.Exists(savePlayerImagePath))
                    {
                        if (!Directory.Exists(playerImageUrl))
                        {
                            Directory.CreateDirectory(playerImageFolder);
                        }
                        StartCoroutine(DownloadAndSaveImage(playerImageUrl, savePlayerImagePath));
                    }

                    string saveNationImagePath = Path.Combine(_savePath, "nation", player.nation.image);
                    string nationImageFolder = Path.GetDirectoryName(saveNationImagePath);
                    string nationImageUrl = _imageUrl + "/nation/" + player.nation.image;
                    if (!File.Exists(saveNationImagePath))
                    {
                        if (!Directory.Exists(nationImageFolder))
                        {
                            Directory.CreateDirectory(nationImageFolder);
                        }
                        StartCoroutine(DownloadAndSaveImage(nationImageUrl, saveNationImagePath));
                    }

                    string saveLeagueImagePath = Path.Combine(_savePath, "league", player.club.league.image);
                    string leagueImageFolder = Path.GetDirectoryName(saveLeagueImagePath);
                    string leagueImageUrl = _imageUrl + "/league/" + player.club.league.image;
                    if (!File.Exists(saveLeagueImagePath))
                    {
                        if (!Directory.Exists(leagueImageFolder))
                        {
                            Directory.CreateDirectory(leagueImageFolder);
                        }
                        StartCoroutine(DownloadAndSaveImage(leagueImageUrl, saveLeagueImagePath));
                    }

                    string saveClubImagePath = Path.Combine(_savePath, "club", player.club.image);
                    string clubImageFolder = Path.GetDirectoryName(saveClubImagePath);
                    string clubImageUrl = _imageUrl + "/club/" + player.club.image;
                    if (!File.Exists(saveClubImagePath))
                    {
                        if (!Directory.Exists(clubImageFolder))
                        {
                            Directory.CreateDirectory(clubImageFolder);
                        }
                        StartCoroutine(DownloadAndSaveImage(clubImageUrl, saveClubImagePath));
                    }

                    string saveCardImagePath = Path.Combine(_savePath, "card", player.card.image);
                    string cardImageFolder = Path.GetDirectoryName(saveCardImagePath);
                    string cardImageUrl = _imageUrl + "/card/" + player.card.image;
                    if (!File.Exists(saveCardImagePath))
                    {
                        if (!Directory.Exists(cardImageFolder))
                        {
                            Directory.CreateDirectory(cardImageFolder);
                        }
                        StartCoroutine(DownloadAndSaveImage(cardImageUrl, saveCardImagePath));
                    }
                }
                if (callback != null)
                {
                    callback(randomPlayers.ToArray());
                }
            }
        }

    }

    private IEnumerator DownloadAndSaveImage(string imageUrl, string savePath)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                byte[] imageBytes = texture.EncodeToPNG();
                File.WriteAllBytes(savePath, imageBytes);
            }
            else
            {
                Debug.LogError("Failed to download image: " + webRequest.error);
            }
        }
    }
}
