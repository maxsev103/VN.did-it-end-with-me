using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GalleryMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup root;
    private CanvasGroupController rootCG;

    [SerializeField] private Texture[] galleryImages;

    [SerializeField] private Button[] galleryPreviewButtons;
    [SerializeField] private Button panelselectionButtonPrefab;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [SerializeField] private CanvasGroup previewPanel;
    private CanvasGroupController previewPanelCG;

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
    }

    public void Close()
    {
        rootCG.Hide();
    }

    private void GetAllGalleryImages()
    {
        galleryImages = Resources.LoadAll<Texture>(FilePaths.resources_gallery);
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
        int targetPages = (int)Mathf.Ceil((float)totalImages / previewsPerPage);

        for (int i = 1; i <= targetPages; i++)
        {
            GameObject buttonObject = Instantiate(panelselectionButtonPrefab.gameObject, panelselectionButtonPrefab.transform.parent);
            buttonObject.gameObject.SetActive(true);

            Button button = buttonObject.GetComponent<Button>();
            TextMeshProUGUI txt = button.GetComponentInChildren<TextMeshProUGUI>();

            buttonObject.name = i.ToString();
            int page = i;
            button.onClick.AddListener(() => LoadPage(page));
            txt.text = i.ToString();
        }

        nextButton.transform.SetAsLastSibling();
    }

    private void LoadPage(int pageNumber)
    {

    }
}
