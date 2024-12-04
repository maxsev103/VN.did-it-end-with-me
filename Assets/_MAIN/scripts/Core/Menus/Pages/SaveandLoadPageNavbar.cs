using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveandLoadPageNavbar : MonoBehaviour
{
    [SerializeField] private SaveandLoadMenu menu;

    private bool initialized = false;

    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject prev;
    [SerializeField] private GameObject next;

    private const int MAX_BUTTONS = 7;

    public int selectedPage { get; private set; } = 1;
    public int maxPages { get; private set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        if (initialized) 
            return;

        initialized = true;

        maxPages = Mathf.CeilToInt((float)SaveandLoadMenu.MAX_FILES / menu.slotsPerPage);
        int pageButtonLimit = MAX_BUTTONS < maxPages ? MAX_BUTTONS : maxPages;

        for (int i = 1; i <= pageButtonLimit; i++)
        {
            GameObject ob = Instantiate(buttonPrefab.gameObject, buttonPrefab.transform.parent);
            ob.SetActive(true);
            Button button = ob.GetComponent<Button>();

            ob.name = $"Page {i.ToString()}";
            TextMeshProUGUI txt = button.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = i.ToString();
            int closureIndex = i;
            button.onClick.AddListener(() => SelectSaveFilePage(closureIndex));
        }

        prev.SetActive(pageButtonLimit < maxPages);
        next.SetActive(pageButtonLimit < maxPages);

        next.transform.SetAsLastSibling();
    }

    private void SelectSaveFilePage(int pageNumber)
    {
        selectedPage = pageNumber;
        menu.PopulateSaveSlotsForPage(selectedPage);
    }

    public void ToNextPage()
    {
        if (selectedPage < maxPages)
            SelectSaveFilePage(selectedPage + 1);
    }

    public void ToPrevPage()
    {
        if (selectedPage > 1)
            SelectSaveFilePage(selectedPage - 1);

    }
}
