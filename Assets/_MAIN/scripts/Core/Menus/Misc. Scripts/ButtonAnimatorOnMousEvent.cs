using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimatorOnMousEvent : MonoBehaviour
{
    [SerializeField] private SaveandLoadPageNavbar navbar;
    public Animator prevAnim;
    public Animator nextAnim;

    public void PlayPrevButtonAnimation()
    {
        if (navbar.selectedPage > 1)
            prevAnim.SetTrigger("PrevBounce");
    }

    public void PlayNextButtonAnimation()
    {
        if (navbar.selectedPage < navbar.maxPages)
            nextAnim.SetTrigger("NextBounce");
    }
}
