using System;
using UnityEngine;
using VISUALNOVEL;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_VN : CMDDatabaseExtension
    {
        new public static void Extend(CommandDatabase database)
        {
            // Variable Assignment
            database.AddCommand("setspvalue", new Action<string>(SetSPValue));
        }

        private static void SetSPValue(string data)
        {
            if (int.TryParse(data, out int intVal))
            {
                VNGameSave.activeFile.SPValue = intVal;
                return;
            }

            Debug.LogWarning($"Invalid value for SetSPValue '{data}'");
        }
    }
}