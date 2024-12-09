using UnityEngine;

public class ButtonAnimatorOnMousEvent : MonoBehaviour
{
    [SerializeField] private SaveandLoadPageNavbar saveLoadNavbar = null;
    [SerializeField] private GalleryMenu galleryNavBar = null;
    public Animator prevAnim;
    public Animator nextAnim;

    public void PlayPrevButtonAnimation()
    {
        if (saveLoadNavbar.selectedPage > 1 || (galleryNavBar != null && galleryNavBar.selectedPage > 1))
            prevAnim.SetTrigger("PrevBounce");
    }

    public void PlayNextButtonAnimation()
    {
        if (saveLoadNavbar.selectedPage < saveLoadNavbar.maxPages || (galleryNavBar != null && galleryNavBar.selectedPage > galleryNavBar.maxPages))
            nextAnim.SetTrigger("NextBounce");
    }
}
