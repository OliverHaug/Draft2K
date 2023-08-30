using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using DG.Tweening;

public class FormationSelection : MonoBehaviour
{
    [SerializeField] private RectTransform FormationFieldTransform;
    [SerializeField] private SqliteHandler sqliteHandler;
    [SerializeField] private List<GameObject> formationButtons;
    [SerializeField] private List<RectTransform> formationDisplayPositions;
    [SerializeField] private List<Formation> randomFormationList = new List<Formation>();

    void Awake()
    {
        GetRandomFormations(5);
        UpdateFormationDisplay(randomFormationList[0]);
        ChangeFormationButtons();
    }

    private void ChangeFormationButtons()
    {
        UIHandler uIHandler = GameObject.FindFirstObjectByType<UIHandler>();

        for (int i = 0; i < randomFormationList.Count; i++)
        {
            Sprite formationImage = Resources.Load<Sprite>($"Images/Formations/{Path.GetFileNameWithoutExtension(randomFormationList[i].images)}");
            Image imageComponent = formationButtons[i].GetComponentsInChildren<Image>()[1];
            if (formationImage != null && imageComponent != null)
            {
                imageComponent.sprite = formationImage;
            }

            TextMeshProUGUI textComponent = formationButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = randomFormationList[i].formationName;
            }
            formationButtons[i].SetActive(true);

            int tempI = i;

            Button buttonComponent = formationButtons[tempI].GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => uIHandler.FormationSelected(randomFormationList[tempI]));
            }

            EventTrigger trigger = formationButtons[tempI].GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = formationButtons[tempI].AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => UpdateFormationDisplay(randomFormationList[tempI]));
            trigger.triggers.Add(entry);
        }
    }

    private void UpdateFormationDisplay(Formation formation)
    {
        for (int i = 0; i < formationDisplayPositions.Count; i++)
        {
            int localIndex = i;
            formationDisplayPositions[localIndex].DOLocalMove(Vector2.zero, 0.2f).OnComplete(() =>
            {
                formationDisplayPositions[localIndex].GetComponentInChildren<TextMeshProUGUI>().text = formation.positions[localIndex].positionName;
                float x = (formation.positions[localIndex].x * FormationFieldTransform.sizeDelta.x - (FormationFieldTransform.sizeDelta.x / 2));
                float y = (formation.positions[localIndex].y * FormationFieldTransform.sizeDelta.y - (FormationFieldTransform.sizeDelta.y / 2));
                formationDisplayPositions[localIndex].DOLocalMove(new Vector2(x, -y), 0.4f);
            });
        }
    }

    private void GetRandomFormations(int count)
    {
        List<Formation> allFormations = sqliteHandler.LoadAllFormations();
        List<Formation> randomFormations = new List<Formation>();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, allFormations.Count);
            randomFormations.Add(allFormations[randomIndex]);
            allFormations.RemoveAt(randomIndex);
        }

        randomFormationList = randomFormations;
    }
}
