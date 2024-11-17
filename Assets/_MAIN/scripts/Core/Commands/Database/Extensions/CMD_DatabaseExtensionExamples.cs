using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_DatabaseExtensionExamples : CMDDatabaseExtension
{
    new public static void Extend(CommandDatabase database)
    {
        // add actions with no parameters
        database.AddCommand("Print", new Action(PrintDefaultMessage));
        database.AddCommand("Print_1p", new Action<string>(PrintUserMessage));
        database.AddCommand("Print_mp", new Action<string[]>(PrintLines));

        // add lambda with no parameters
        database.AddCommand("Lambda", new Action(() => { Debug.Log("Printing a default message to console from Lambda command."); }));
        database.AddCommand("Lambda_1p", new Action<string>((arg) => { Debug.Log($"Log user Lambda Message: '{arg}'"); }));
        database.AddCommand("Lambda_mp", new Action<string[]>((args) => { Debug.Log(string.Join(", ", args)); }));

        // add coroutine with no parameters
        database.AddCommand("Process", new Func<IEnumerator>(SimpleProcess));
        database.AddCommand("Process_1p", new Func<string, IEnumerator>(LineProcess));
        database.AddCommand("Process_mp", new Func<string[], IEnumerator>(MultiLineProcess));

        // special test
        database.AddCommand("MoveChara", new Func<string, IEnumerator>(MoveCharacter));
    }

    private static void PrintDefaultMessage()
    {
        Debug.Log("Printing a default message to console.");
    }

    private static void PrintUserMessage(string message)
    {
        Debug.Log($"User message: '{message}'");
    }

    private static void PrintLines(string[] lines)
    {
        int i = 1;
        foreach (string line in lines)
            Debug.Log($"{i++}. '{line}'");
    }

    private static IEnumerator SimpleProcess()
    {
        for (int i = 0; i < 5; i++)
        {
            Debug.Log($"Process running... [{i}]");
            yield return new WaitForSeconds(1);
        }
    }

    private static IEnumerator LineProcess(string data)
    {
        if (int.TryParse(data, out int num))
        {
            for (int i = 1; i <= num; i++)
            {
                Debug.Log($"Process Running... [{i}]");
                yield return new WaitForSeconds(1);
            }
        }
    }

    private static IEnumerator MultiLineProcess(string[] data)
    {
        foreach (string line in data)
        {
            Debug.Log($"Process Message: '{line}'");
            yield return new WaitForSeconds(0.5f);
        }
    }

    private static IEnumerator MoveCharacter(string direction)
    {
        bool left = direction.ToLower() == "left";

        // get variable neeeded
        Transform character = GameObject.Find("Image").transform;
        float moveSpeed = 50;

        // calculate target position for image
        float targetX = left ? -50 : 50;

        // calculate current pos of image
        float currentX = character.position.x;

        // move image gradually towards the left
        while (Mathf.Abs(targetX - currentX) > 0.1f)
        {
            currentX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
            character.position = new Vector3(currentX, character.position.y, character.position.z);
            yield return null;
        }
    }
}
