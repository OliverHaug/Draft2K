using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum CardType
{
    Small,
    Big,
}

[System.Serializable]
class TMPUIAttributes
{
    public TextMeshProUGUI label;
    public TextMeshProUGUI value;
}

public class Card : MonoBehaviour
{

    [SerializeField] private CardType cardType;
    [SerializeField] private GameObject playerFaceNormal;
    [SerializeField] private GameObject playerFaceDynamic;
    [SerializeField] private TextMeshProUGUI ratingText;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private List<TMPUIAttributes> attributes;
    [SerializeField] private GameObject nationImage;
    [SerializeField] private GameObject leagueImage;
    [SerializeField] private GameObject clubImage;

    public void CreateCard(PlayerModel player)
    {
        if (cardType == CardType.Small)
        {

        }
        else
        {
            GetComponent<Image>().sprite = LoadImage(Path.Combine(Application.persistentDataPath, "images", "card", player.card.image));
        }


        if (player.image[0] == 'p')
        {
            //TODO: Load Images
            playerFaceNormal.SetActive(false);
            playerFaceDynamic.SetActive(true);
        }
        else
        {
            playerFaceNormal.SetActive(true);
            playerFaceDynamic.SetActive(false);
            playerFaceNormal.GetComponent<Image>().sprite = LoadImage(Path.Combine(Application.persistentDataPath, "images", "player", player.image));

        }



        for (int i = 0; i < attributes.Count; i++)
        {
            attributes[i].value.text = player.attributes.attribute[i].value.ToString();
            attributes[i].label.text = player.attributes.attribute[i].label.ToUpper().Substring(0, Math.Min(player.attributes.attribute[i].label.Length, 3));
        }

        if (player.card.id == 2)
        {

        }
        else
        {
            nationImage.GetComponent<RawImage>().texture = LoadImage(Path.Combine(Application.persistentDataPath, "images", "nation", player.nation.image)).texture;
            leagueImage.GetComponent<RawImage>().texture = LoadImage(Path.Combine(Application.persistentDataPath, "images", "league", player.club.league.image)).texture;
            clubImage.GetComponent<RawImage>().texture = LoadImage(Path.Combine(Application.persistentDataPath, "images", "club", player.club.image)).texture;
            nationImage.SetActive(true);
            leagueImage.SetActive(false);
            clubImage.SetActive(true);
        }

        ratingText.text = player.attributes.rating.ToString();
        positionText.text = player.attributes.position;
        playerNameText.text = player.displayName;

        Color textColor;
        if (ColorUtility.TryParseHtmlString(player.card.overlayTextColor, out textColor))
        {
            ratingText.color = textColor;
            positionText.color = textColor;
            playerNameText.color = textColor;

            for (int i = 0; i < attributes.Count; i++)
            {
                attributes[i].value.color = textColor;
                attributes[i].label.color = textColor;
            }
        }
    }

    private Sprite LoadImage(string imagePath)
    {
        if (File.Exists(imagePath))
        {
            byte[] imageData = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(2, 2);

            texture.LoadImage(imageData);
            Texture2D trimmedTexture = RemoveTransparentBorder(texture);
            Sprite newSprite = Sprite.Create(trimmedTexture, new Rect(0, 0, trimmedTexture.width, trimmedTexture.height), Vector2.one * 0.5f);
            return newSprite;
        }
        else
        {
            Debug.LogError("Bild nicht gefunden: " + imagePath);
            return null;
        }

        static Texture2D RemoveTransparentBorder(Texture2D sourceTexture)
        {
            Color32[] pixels = sourceTexture.GetPixels32();
            int width = sourceTexture.width;
            int height = sourceTexture.height;

            int left = width;
            int right = 0;
            int top = height;
            int bottom = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color32 pixel = pixels[y * width + x];
                    if (pixel.a != 0)
                    {
                        left = Mathf.Min(left, x);
                        right = Mathf.Max(right, x);
                        top = Mathf.Min(top, y);
                        bottom = Mathf.Max(bottom, y);
                    }
                }
            }

            int newWidth = right - left + 1;
            int newHeight = bottom - top + 1;

            Texture2D trimmedTexture = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, false);
            Color32[] trimmedPixels = new Color32[newWidth * newHeight];

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    Color32 pixel = pixels[y * width + x];
                    trimmedPixels[(y - top) * newWidth + (x - left)] = pixel;
                }
            }

            trimmedTexture.SetPixels32(trimmedPixels);
            trimmedTexture.Apply();

            return trimmedTexture;
        }
    }
}
