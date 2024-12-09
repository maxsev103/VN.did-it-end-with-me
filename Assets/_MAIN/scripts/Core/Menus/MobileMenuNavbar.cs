using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileMenuNavbar : MonoBehaviour
{
    public Animator anim;
    public Button open;
    public Button close;
    public Button closeWholeScreen;

    public void Open()
    {
        open.gameObject.SetActive(false);
        close.gameObject.SetActive(true);
        closeWholeScreen.gameObject.SetActive(true);
        anim.Play("Open");
    }

    public void Close()
    {
        close.gameObject.SetActive(false);
        closeWholeScreen.gameObject.SetActive(false);
        open.gameObject.SetActive(true);
        anim.Play("Close");
    }
}
