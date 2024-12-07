using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonBehaviors : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public static UIButtonBehaviors selectedButton = null;
    public UIButtonType buttonType = UIButtonType.Default;
    public bool enableEnterSFX = false;

    private static string mouseEnterSFX = FilePaths.GetPathToResources(FilePaths.resources_sfx, "switch sound 3");
    private static string mouseClickSFX_A = FilePaths.GetPathToResources(FilePaths.resources_sfx, "switch sound 1");
    private static string mouseClickSFX_B = FilePaths.GetPathToResources(FilePaths.resources_sfx, "switch sound 2");

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buttonType == UIButtonType.Default)
            AudioManager.instance.PlaySoundEffect(mouseClickSFX_A);
        else
            AudioManager.instance.PlaySoundEffect(mouseClickSFX_B);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (enableEnterSFX)
            AudioManager.instance.PlaySoundEffect(mouseEnterSFX);

        selectedButton = this;
    }

    public enum UIButtonType { Default, Reject };
}
