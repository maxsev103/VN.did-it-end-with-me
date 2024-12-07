using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicPanelManager : MonoBehaviour
{
    public static GraphicPanelManager instance { get; private set; }

    [field:SerializeField] public GraphicPanel[] allPanels { get; private set; }

    public const float defaultTransitionSpeed = 1.5f;

    private void Awake()
    {
        instance = this;
    }

    public GraphicPanel GetPanel(string name)
    {
        name = name.ToLower(); 

        foreach (var panel in allPanels)
        {
            if (panel.panelName.ToLower() == name)
                return panel;
        }

        return null;
    }
}
