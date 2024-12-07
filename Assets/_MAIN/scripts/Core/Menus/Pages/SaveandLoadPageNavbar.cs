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
    [SerializeField] private List<GameObject> pageNavigationButtons;

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

            ob.name = $"Page {i}";
            TextMeshProUGUI txt = button.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = i.ToString();
            int closureIndex = i;
            button.onClick.AddListener(() => SelectSaveFilePage(closureIndex));

            pageNavigationButtons.Add(ob);
        }

        prev.SetActive(pageButtonLimit <= maxPages);
        next.SetActive(pageButtonLimit <= maxPages);

        next.transform.SetAsLastSibling();

        UpdateNavbarPageButtons();
    }

    private void UpdateNavbarPageButtons()
    {
        ColorUtility.TryParseHtmlString("#8A4F1C", out Color textSelectedColor);

        for (int i = 0; i < pageNavigationButtons.Count; i++)
        {
            var pageButton = pageNavigationButtons[i];
            var text = pageButton.GetComponentInChildren<TextMeshProUGUI>();

            if ((i + 1) == selectedPage)
            {
                text.color = textSelectedColor;
                continue;
            }

            text.color = Color.white;
        }
    }

    private void SelectSaveFilePage(int pageNumber)
    {
        selectedPage = pageNumber;
        UpdateNavbarPageButtons();
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
