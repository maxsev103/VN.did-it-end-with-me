using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMD_DatabaseExtensionExamples : CMDDatabaseExtension
{
    new public static void Extend(CommandDatabase database)
    {
        // add command with no parameters
        database.AddCommand("Print", new Action(PrintDefaultMessage));
    }

    private static void PrintDefaultMessage()
    {
        Debug.Log("Printing a default message to console.");
    }
}
