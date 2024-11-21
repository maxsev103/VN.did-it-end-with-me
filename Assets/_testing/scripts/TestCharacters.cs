using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);
        // Start is called before the first frame update
        void Start()
        {
            //Character Ysella = CharacterManager.instance.CreateCharacter("Ysella");
            //Character Ysella2 = CharacterManager.instance.CreateCharacter("Ysella");

            //Character Zombie = CharacterManager.instance.CreateCharacter("Zombie");
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Character_Sprite Ysella = CreateCharacter("Ysella") as Character_Sprite;
            Character_Sprite YsellaAlter = CreateCharacter("Ysella(?) as Ysella") as Character_Sprite;
            YsellaAlter.isVisible = false;

            Sprite faceSprite = Ysella.GetSprite("Ysella_neutral");
            Ysella.SetSprite(faceSprite, 1);

            Ysella.SetPosition(new Vector2(0.30f, 0));
            YsellaAlter.SetPosition(new Vector2(0.65f, 0));
            YsellaAlter.SetColor(Color.black);

            Ysella.Unhighlight();
            yield return Ysella.Flip();
            yield return new WaitForSeconds(1);
            yield return Ysella.Flip();
            yield return new WaitForSeconds(1);

            Ysella.Highlight();
            yield return Ysella.Say(".{wa 0.2}.{wa 0.2}.{wa 0.2}And I'm lost.{c}Just my luck,{wa 0.5} I just HAD to get lost in some creepy place like this.{c}Ugh.");
            Ysella.Unhighlight();

            YsellaAlter.Flip();
            yield return YsellaAlter.Say("...");
            
            Ysella.Highlight();
            Ysella.Animate("Hop");
            yield return Ysella.Say("Who's there!?");
            Ysella.Unhighlight();

            YsellaAlter.Show();
            YsellaAlter.Highlight();
            yield return YsellaAlter.Say("Hello, Ysella.{c}It's nice of you to finally show up.");
            YsellaAlter.Unhighlight();

            Ysella.Highlight();
            Ysella.Animate("Shiver", true);
            yield return Ysella.Say("W-{wa 0.1}who are you?");
            Ysella.Unhighlight();

            YsellaAlter.Hide();
            yield return new WaitForSeconds(1);
            Ysella.Animate("Shiver", false);
            Ysella.Highlight();
            yield return Ysella.Say("What the hell was that?!");
            Ysella.Unhighlight();

            yield return null;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}