using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using CHARACTERS;

namespace TESTING
{
    public class TestGraphicLayer : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
            GraphicLayer layer = panel.GetLayer(0, true);
            Texture blendtex = Resources.Load<Texture>("Graphics/Transition Effects/fade4");

            layer.SetTexture("Graphics/BG Images/4. GARAGE v2", blendingTexture: blendtex);

            layer.SetVideo("Graphics/BG Videos/Fantasy Landscape", transitionSpeed: 0.5f, blendingTexture: blendtex);

            yield return new WaitForSeconds(2);

            GraphicPanel cg = GraphicPanelManager.instance.GetPanel("CG");
            GraphicLayer cinLayer = cg.GetLayer(0, true);

            Character Ysella = CharacterManager.instance.CreateCharacter("Ysella");
            Ysella.Show();

            yield return Ysella.Say("Now to my God.");

            cinLayer.SetTexture("Graphics/Gallery/furina");

            yield return DialogueSystem.instance.Say("Narrator", "The Humanity to God's Divinity.");

            //layer.currentGraphic.FadeOut();

            //yield return new WaitForSeconds(2);

            //Debug.Log(layer.currentGraphic);

        }
    }
}