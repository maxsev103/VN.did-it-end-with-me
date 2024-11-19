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
            Character_Sprite Ysella2 = CreateCharacter("Clone as Ysella") as Character_Sprite;

            Sprite faceSprite = Ysella.GetSprite("Ysella_neutral");
            Ysella.TransitionSprite(faceSprite, 1);

            Ysella.SetPosition(Vector2.zero);
            Ysella2.SetPosition(Vector2.one);

            Ysella.Unhighlight();
            yield return Ysella2.Say("So....");
            
            Ysella2.Unhighlight();
            Ysella.Highlight();
            yield return Ysella.Say("So.........");

            faceSprite = Ysella.GetSprite("Ysella_happy");
            Ysella.TransitionSprite(faceSprite, layer: 1);

            yield return Ysella.MoveToPosition(new Vector2(0, 0.5f));
            yield return Ysella.MoveToPosition(new Vector2(0, -0.5f));

            Ysella2.TransitionSprite(Ysella.GetSprite("Ysella_neutral"), 1);

            Ysella.Unhighlight();
            Ysella2.Highlight();
            yield return Ysella2.Say("Wtf girl why did u jump");

            Ysella2.Unhighlight();
            Ysella.Highlight();
            yield return Ysella.Say("Girl idk");

            Ysella.Unhighlight();
            Ysella.TransitionSprite(Ysella.GetSprite("Ysella_neutral"), 1);

            yield return null;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}