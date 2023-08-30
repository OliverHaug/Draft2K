using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        }



        for (int i = 0; i < attributes.Count; i++)
        {
            attributes[i].value.text = player.attribute.attributes[i].value;
            attributes[i].label.text = player.attribute.attributes[i].label;
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

        ratingText.text = player.attribute.rating.ToString();
        positionText.text = player.attribute.position;
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


}
