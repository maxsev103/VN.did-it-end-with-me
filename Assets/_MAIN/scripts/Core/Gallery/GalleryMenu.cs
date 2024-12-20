using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GalleryMenu : MonoBehaviour
{
    private const int PAGE_BUTTON_LIMIT = 4;
    public int maxPages { get; private set; } = 0;
    public int selectedPage { get; private set; } = 1;

    [SerializeField] private CanvasGroup root;
    private CanvasGroupController rootCG;

    [SerializeField] private Texture[] galleryImages;

    [SerializeField] private Button[] galleryPreviewButtons;
    [SerializeField] private Button panelSelectionButtonPrefab;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private List<GameObject> pageNavigationButtons;

    [SerializeField] private CanvasGroup previewPanel;
    private CanvasGroupController previewPanelCG;
    [SerializeField] private Button previewButton;

    private bool initialized = false;
    private int previewsPerPage => galleryPreviewButtons.Length;

    // Start is called before the first frame update
    void Start()
    {
        rootCG = new CanvasGroupController(this, root);
        previewPanelCG = new CanvasGroupController(this, previewPanel);

        GalleryConfig.Load();

        GetAllGalleryImages();
    }

    public void Open()
    {
        if (!initialized)
            Initialize();

        rootCG.Show();
        rootCG.SetInteractableState(true);
    }

    public void Close()
    {
        rootCG.Hide();
        rootCG.SetInteractableState(false);
    }

    private void GetAllGalleryImages()
    {
        galleryImages = Resources.LoadAll<Texture>(FilePaths.resources_gallery);

        // sort the gallery images properly
        galleryImages = galleryImages.OrderBy(texture =>
        {
            if (int.TryParse(texture.name, out int num))
            {
                return num;
            }

            // if parse fails, put the texture at the end
            return int.MaxValue;
        }).ToArray();
    }

    private void Initialize()
    {
        initialized = true;
        ConstructNavbar();
        LoadPage(1);
    }

    private void ConstructNavbar()
    {
        int totalImages = galleryImages.Length;
        maxPages = (int)Mathf.Ceil((float)totalImages / previewsPerPage);
        int pageLimit = PAGE_BUTTON_LIMIT < maxPages ? PAGE_BUTTON_LIMIT : maxPages;

        for (int i = 1; i <= pageLimit; i++)
        {
            GameObject buttonObject = Instantiate(panelSelectionButtonPrefab.gameObject, panelSelectionButtonPrefab.transform.parent);
            buttonObject.gameObject.SetActive(true);

            Button button = buttonObject.GetComponent<Button>();
            TextMeshProUGUI txt = button.GetComponentInChildren<TextMeshProUGUI>();

            buttonObject.name = $"Page - {i}";
            int page = i;
            button.onClick.AddListener(() => LoadPage(page));
            txt.text = i.ToString();

            pageNavigationButtons.Add(buttonObject);
        }
        
        prevButton.gameObject.SetActive(pageLimit <= maxPages);
        nextButton.gameObject.SetActive(pageLimit <= maxPages);

        nextButton.transform.SetAsLastSibling();
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

    private void LoadPage(int pageNumber)
    {
        int startingIndex = (pageNumber - 1) * previewsPerPage;

        for (int i = 0; i < previewsPerPage; i++)
        {
            int index = i + startingIndex;
            Button button = galleryPreviewButtons[i];

            button.onClick.RemoveAllListeners();

            if (index >= galleryImages.Length)
            {
                button.transform.parent.gameObject.SetActive(false);
                continue;
            }
            else
            {
                button.transform.parent.gameObject.SetActive(true);
                RawImage renderer = button.targetGraphic as RawImage;
                Texture previewImage = galleryImages[index];

                if (GalleryConfig.ImageIsUnlocked(previewImage.name))
                {
                    renderer.color = Color.white;
                    renderer.texture = previewImage;
                    button.onClick.AddListener(() => ShowPreviewImage(previewImage));
                }
                else
                {
                    renderer.color = Color.black;
                    renderer.texture = null;
                }
            }
        }

        selectedPage = pageNumber;
        UpdateNavbarPageButtons();
    }

    private void ShowPreviewImage(Texture image)
    {
        RawImage renderer = previewButton.targetGraphic as RawImage;
        renderer.texture = image;
        previewPanelCG.Show();
        previewPanelCG.SetInteractableState(true);
    }

    public void HidePreviewImage()
    {
        previewPanelCG.Hide();
        previewPanelCG.SetInteractableState(false);
    }

    public void ToNextPage()
    {
        if (selectedPage < maxPages)
            LoadPage(selectedPage + 1);
    }

    public void ToPreviousPage()
    {
        if (selectedPage > 1)
            LoadPage(selectedPage - 1);
    }
}
