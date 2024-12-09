using UnityEngine;

public class ButtonAnimatorOnMousEvent : MonoBehaviour
{
    [SerializeField] private SaveandLoadPageNavbar saveLoadNavbar = null;
    [SerializeField] private GalleryMenu galleryNavBar = null;
    public Animator prevAnim;
    public Animator nextAnim;

    public void PlayPrevButtonAnimation()
    {
        if (saveLoadNavbar.selectedPage > 1)
            prevAnim.SetTrigger("PrevBounce");

        if (galleryNavBar == null)
            return;
        else if (galleryNavBar.selectedPage > 1)
            nextAnim.SetTrigger("NextBounce");
    }

    public void PlayNextButtonAnimation()
    {
        if (saveLoadNavbar.selectedPage < saveLoadNavbar.maxPages)
            nextAnim.SetTrigger("NextBounce");

        if (galleryNavBar == null)
            return;
        else if (galleryNavBar.selectedPage > galleryNavBar.maxPages)
            nextAnim.SetTrigger("NextBounce");
    }
}
