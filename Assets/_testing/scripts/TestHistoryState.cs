using HISTORY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class TestHistoryState : MonoBehaviour
    {
        public HistoryState historyState = new HistoryState();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                historyState = HistoryState.Capture();

            if (Input.GetKeyDown(KeyCode.R))
                historyState.Load();
        }
    }
}