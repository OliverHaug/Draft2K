using System.Collections.Generic;
using System.IO;
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
            attributes[i].label.text = player.attributes.attribute[i].label;
        }

        if (player.card.id == 1)
        {

        }
        else
        {
            //TODO: Load Images
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

            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            return newSprite;
        }
        else
        {
            Debug.LogError("Bild nicht gefunden: " + imagePath);
            return null;
        }
    }
}
