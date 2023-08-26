using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject formationSelection;


    public void StartFormationSelection()
    {
        formationSelection.SetActive(true);
    }
}
