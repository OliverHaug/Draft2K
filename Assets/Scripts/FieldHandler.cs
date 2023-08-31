using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class FieldHandler : MonoBehaviour
{
    [SerializeField] private List<Transform> positionObjects;
    [SerializeField] private RectTransform FootballField;
    private UIHandler uIHandler;
    [SerializeField] private Formation formation;

    private void Awake()
    {
        uIHandler = GameObject.FindFirstObjectByType<UIHandler>();
        uIHandler.OnMenuToogle += () => StartCoroutine(UpdatePositionsAfterDelay());

    }
    public void StartDraftWithFormation(Formation _formation)
    {
        formation = _formation;
        UpdatePositionTransform(.5f);
    }

    private IEnumerator UpdatePositionsAfterDelay()
    {
        yield return null;

        UpdatePositionTransform(0f);
    }

    private void UpdatePositionTransform(float moveTime)
    {
        for (int i = positionObjects.Count - 1; i >= 0; i--)
        {
            if (formation.positions.Count > i && formation.positions[i] != null)
            {
                float x = (formation.positions[i].x * FootballField.sizeDelta.x - (FootballField.sizeDelta.x / 2));
                float y = (formation.positions[i].y * FootballField.sizeDelta.y - (FootballField.sizeDelta.y / 2));
                Debug.Log("Pos: " + formation.positions[i].positionName + ", x: " + x + ", y: " + y + " sizeDelta: " + FootballField.sizeDelta);
                positionObjects[i].DOLocalMove(new Vector2(x, -y), moveTime);
                positionObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = formation.positions[i].positionName;
            }
        }
    }

    public void PlacePlayerAtPosition(PlayerModel player)
    {
        Debug.Log(player.name);
    }
}
