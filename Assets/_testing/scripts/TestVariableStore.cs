using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class TestVariableStore : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            VariableStore.CreateDatabase("CalyxDB");
            VariableStore.CreateDatabase("YsellaDB");

            VariableStore.CreateVariable("CalyxDB.num1", 1);
            VariableStore.CreateVariable("YsellaDB.num10", 10);
            VariableStore.CreateVariable("CalyxDB.isLightOn", true);
            VariableStore.CreateVariable("float1", 1.05f);
            VariableStore.CreateVariable("YsellaDB.float2", 2.1f);
            VariableStore.CreateVariable("str1", "string 1");
            VariableStore.CreateVariable("str2", "string 2");

            VariableStore.PrintAllDatabases();
            VariableStore.PrintAllVariables();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                VariableStore.RemoveVariable("float1");
                VariableStore.RemoveVariable("CalyxDB.isLightOn");
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                VariableStore.TrySetValue("float1", 8.32f);
                VariableStore.TrySetValue("str1", "Startend");
                VariableStore.TrySetValue("CalyxDB.num1", 100);
                VariableStore.TrySetValue("CalyxDB.isLightOn", false);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                VariableStore.PrintAllVariables();
            }
        }
    }
}