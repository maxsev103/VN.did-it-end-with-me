using COMMANDS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class CommandTesting : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //StartCoroutine(Running());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                CommandManager.instance.Execute("MoveChara", "left");
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                CommandManager.instance.Execute("MoveChara", "right");
        }

        IEnumerator Running()
        {
            yield return CommandManager.instance.Execute("Print");
            yield return CommandManager.instance.Execute("Print_1p", "Heyy bestie");
            yield return CommandManager.instance.Execute("Print_mp", "Line 1", "Line 2", "Line 3");

            yield return CommandManager.instance.Execute("Lambda");
            yield return CommandManager.instance.Execute("Lambda_1p", "Heyy Hyperion AI Lambda");
            yield return CommandManager.instance.Execute("Lambda_mp", "Lambda 1", "Lambda 2", "Lambda 3");

            yield return CommandManager.instance.Execute("Process");
            yield return CommandManager.instance.Execute("Process_1p", "3");
            yield return CommandManager.instance.Execute("Process_mp", "Process 1", "Process 2", "Process 3");
        }
    }
}