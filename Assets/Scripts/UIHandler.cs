using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject formationSelection;
    [SerializeField] private GameObject menuObject;
    private bool PlayerMenuOpen = false;

    public delegate void FormationSelectedDelegate(Formation formation);
    public event FormationSelectedDelegate OnFormationSelected;

    public delegate void MenuToogleDelegate();
    public event MenuToogleDelegate OnMenuToogle;


    public void StartFormationSelection()
    {
        formationSelection.SetActive(true);
    }

    public void FormationSelected(Formation formation)
    {
        formationSelection.SetActive(false);
        OnFormationSelected?.Invoke(formation);
    }

    public void ToggleMenu()
    {
        menuObject.GetComponent<LayoutElement>().minWidth = PlayerMenuOpen ? 400 : 0;
        PlayerMenuOpen = !PlayerMenuOpen;
        OnMenuToogle?.Invoke();
    }
}
