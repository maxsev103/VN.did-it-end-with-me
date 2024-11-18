using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
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
            Character Agnes = CharacterManager.instance.CreateCharacter("Agnes");
            Character Ysella = CharacterManager.instance.CreateCharacter("Ysella");
            Character Zombie = CharacterManager.instance.CreateCharacter("Zombie");

            List<string> lines = new List<string>()
            {
                "\"Once upon a time, there lived a wolf in the forest.\"",
                "\"This wolf lived everyday picking berries to eat.\"",
                "\"Did it hunt, you ask? Why, sure it did. But it never hunted other animals in the forest.\"",
                "\"No. Instead, it hunted intruders who dared to disrupt the forest's peace.\""
            };

            yield return Agnes.Say(lines);

            lines = new List<string>()
            {
                "\"The wolf acted like the forest's bodyguard, protecting it from malicious actors trying to take" +
                "advantage of it.\"",
                "\"It howled{wa 0.3} and it growled and it gnashed its teeth at any a person who it could smell had nefarious" +
                " intentions.\"",
                "\"Because of this, it had gained a reputation among the villages on the outskirts of the forest.\"",
                "Did you hear?{c}Hear what?{c}About that barbaric wolf in the forest?"
            };

            yield return Ysella.Say(lines);

            lines = new List<string>()
            {
                "\"As the rumours spread of a wolf so ferocious that no one, not even lumbermen, would dare enter the forest," +
                "the King began sending huntsmen to aid in his subjects' plight.\"",
                "\"These huntsmen were unlike any bumbling hunter looking to make quick coin from the animals.\"",
                "\"No. They were truly well-trained---{wa 0.5}well trained to kill on sight.\"",
                "\"And they would stop at nothing until that dastardly wolf was {wa 0.3}d{wa 0.3}e{wa 0.3}a{wa 0.3}d{wa 0.3}.\""
            };

            yield return Zombie.Say(lines);

            Debug.Log("Finished.");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}