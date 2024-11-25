using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;

namespace TESTING
{
    public class TestAudio : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Running());
        }

        Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        IEnumerator Running()
        {
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
            GraphicLayer layer = panel.GetLayer(0, true);
            Texture blendTex = Resources.Load<Texture>("Graphics/Transition Effects/fade4");

            yield return new WaitForSeconds(1);

            Character_Sprite Ysella = CreateCharacter("Ysella") as Character_Sprite;
            Ysella.SetPosition(new Vector2(-0.5f, 0));
            Ysella.MoveToPosition(new Vector2(0.3f, 0), speed: 1f);
            Ysella.Show();

            yield return new WaitForSeconds(2);

            yield return DialogueSystem.instance.Say("narrator", "After a few moments, ain slowly began to fall.");
            AudioManager.instance.PlayTrack("Audio/Ambience/RainyMood", volumeCap: 0.4f);

            Ysella.SetSprite(Ysella.GetSprite("Ysella_neutral"), 1);
            
            yield return Ysella.Say("What the-- where did this rain come from?!");
            AudioManager.instance.PlaySoundEffect("Audio/SFX/thunder_01", volume: 0.2f);

            yield return new WaitForSeconds(1);
            AudioManager.instance.PlaySoundEffect("Audio/SFX/thunder_strong_01", volume: 0.5f);
            Ysella.Animate("Hop");

            yield return Ysella.Say("I need to find shelter!");

            yield return Ysella.MoveToPosition(new Vector2(1.5f, 0), speed: 3f);

            yield return DialogueSystem.instance.Say("Narrator", ".{wa 0.4}.{wa 0.4}.");

            layer.SetTexture("Graphics/BG Images/1. DINING ROOM 1", blendingTexture: blendTex);
            AudioManager.instance.PlayTrack("Audio/Music/Calm2", volumeCap: 0.4f);

            yield return new WaitForSeconds(1);

            Ysella.SetPosition(new Vector2(-0.5f, 0));
            Ysella.MoveToPosition(new Vector2(0.2f, 0), speed: 0.5f);
            yield return Ysella.Say("Phew! Finally escaped that rain!");

            yield return null;
        }
    }
}