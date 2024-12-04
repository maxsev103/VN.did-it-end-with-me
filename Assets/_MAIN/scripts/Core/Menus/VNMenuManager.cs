using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VNMenuManager : MonoBehaviour
{
    public static VNMenuManager instance;

    private MenuPage activePage = null;
    private bool isOpen = false;

    [SerializeField] private CanvasGroup root;
    [SerializeField] private MenuPage[] pages;

    private CanvasGroupController rootCG;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
    }

    private MenuPage GetPage(MenuPage.PageType pageType)
    {
        return pages.FirstOrDefault(page => page.pageType == pageType);
    }

    public void Open_SavePage()
    {
        var page = GetPage(MenuPage.PageType.SaveandLoad);
        var slm = page.anim.GetComponentInParent<SaveandLoadMenu>();
        slm.menuFunction = SaveandLoadMenu.MenuFunction.Save;
        OpenPage(page);
    }

    public void Open_LoadPage()
    {
        var page = GetPage(MenuPage.PageType.SaveandLoad);
        var slm = page.anim.GetComponentInParent<SaveandLoadMenu>();
        slm.menuFunction = SaveandLoadMenu.MenuFunction.Load;
        OpenPage(page);
    }

    public void Open_ConfigPage()
    {
        var page = GetPage(MenuPage.PageType.Config);
        OpenPage(page);
    }

    public void Open_HelpPage()
    {
        var page = GetPage(MenuPage.PageType.Help);
        OpenPage(page);
    }

    private void OpenPage(MenuPage page)
    {
        if (page == null)
            return;

        if (activePage != null && activePage != page)
            activePage.Close();

        page.Open();
        activePage = page;

        if (!isOpen)
            Open_Root();
    }

    public void Open_Root()
    {
        rootCG.Show(speed: 2f);
        rootCG.SetInteractableState(true);
        isOpen = true;
    }

    public void Close_Root()
    {
        rootCG.Hide(speed: 2f);
        rootCG.SetInteractableState(false);
        isOpen = false;
    }
}
