using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Character Ysella = CharacterManager.instance.CreateCharacter("Ysella");
            Character Ysella2 = CharacterManager.instance.CreateCharacter("Ysella");
            Character Agnes = CharacterManager.instance.CreateCharacter("Agnes");
            Character Zombie = CharacterManager.instance.CreateCharacter("Zombie");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}