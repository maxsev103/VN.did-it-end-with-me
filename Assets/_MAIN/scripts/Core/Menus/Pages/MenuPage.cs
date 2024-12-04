using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    public enum PageType { SaveandLoad, Config, Help }
    public PageType pageType;

    private const string OPEN = "Open";
    private const string CLOSE = "Close";

    [SerializeField] public Animator anim;
    public virtual void Open()
    {
        anim.SetTrigger(OPEN);
    }

    public virtual void Close(bool closeAllMenus = false)
    {
        anim.SetTrigger(CLOSE);

        if (closeAllMenus)
        {
            VNMenuManager.instance.Close_Root();
        }
    }
}
