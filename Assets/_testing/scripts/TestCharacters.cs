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
            Character guard1 = CreateCharacter("Guard1 as Ysella");
            Character guard2 = CreateCharacter("Guard2 as Ysella");
            Character guard3 = CreateCharacter("Guard3 as Ysella");

            guard1.SetPosition(Vector2.zero);
            guard2.SetPosition(new Vector2(0.5f, 0.5f));
            guard3.SetPosition(Vector2.one);

            guard1.Show();
            guard2.Show();
            guard3.Show();

            yield return guard1.MoveToPosition(Vector2.one);
            yield return guard1.MoveToPosition(Vector2.zero);

            yield return guard3.MoveToPosition(Vector2.zero, smooth:true);
            yield return guard3.MoveToPosition(Vector2.one, smooth:true);

            yield return null;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}