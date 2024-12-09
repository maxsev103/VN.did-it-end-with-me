using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonBehaviors : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public static UIButtonBehaviors selectedButton = null;
    public UIButtonType buttonType = UIButtonType.Default;
    public bool enableEnterSFX = false;

    private static string mouseEnterSFX;
    private static string mouseClickSFX_A;
    private static string mouseClickSFX_B;

    private void Start()
    {
        mouseEnterSFX = FilePaths.GetPathToResources(FilePaths.resources_sfx, "switch sound 3");
        mouseClickSFX_A = FilePaths.GetPathToResources(FilePaths.resources_sfx, "switch sound 1");
        mouseClickSFX_B = FilePaths.GetPathToResources(FilePaths.resources_sfx, "switch sound 2");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonType == UIButtonType.Default)
            AudioManager.instance.PlaySoundEffect(mouseClickSFX_A, volume: 0.7f);
        else
            AudioManager.instance.PlaySoundEffect(mouseClickSFX_B, volume: 0.7f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (enableEnterSFX)
            AudioManager.instance.PlaySoundEffect(mouseEnterSFX, volume: 0.7f);

        selectedButton = this;
    }

    public enum UIButtonType { Default, Reject };
}
