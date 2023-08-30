using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DraftHandler : MonoBehaviour
{
    private Formation selectedFormation;
    private UIHandler uIHandler;

    private void Awake()
    {
        uIHandler = gameObject.GetComponent<UIHandler>();
        uIHandler.OnFormationSelected += HandleFormationSelected;
    }
    private void Start()
    {
        uIHandler.StartFormationSelection();
    }

    private void HandleFormationSelected(Formation formation)
    {
        selectedFormation = formation;
        FieldHandler fieldHandler = GameObject.FindFirstObjectByType<FieldHandler>();
        if (fieldHandler != null)
        {
            fieldHandler.StartDraftWithFormation(selectedFormation);
        }
    }

}
