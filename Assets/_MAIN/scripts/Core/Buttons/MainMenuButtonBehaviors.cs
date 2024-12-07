using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehaviors : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private static ButtonBehaviors selectedButton = null;
    public Animator anim;

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.instance.PlaySoundEffect(FilePaths.GetPathToResources(FilePaths.resources_sfx, "switch sound 3"));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectedButton != null && selectedButton != this)
        {
            selectedButton.OnPointerExit(null);
        }

        anim.Play("Enter");
        AudioManager.instance.PlaySoundEffect(FilePaths.GetPathToResources(FilePaths.resources_sfx, "switch sound 1"));
        selectedButton = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.Play("Exit");
    }
}
