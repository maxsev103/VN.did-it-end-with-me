using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class TestChoicePanel : MonoBehaviour
    {
        ChoicePanel panel;

        // Start is called before the first frame update
        void Start()
        {
            panel = ChoicePanel.instance;
            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            string[] choices = new string[]
            {
                "Trust the survivors",
                "Be cautious of them",
                "Actively antagonize the survivors due to mistrust and bad blood"
            };

            panel.Show("What should Ysella do?", choices);

            while (panel.isWaitingForUserChoice)
                yield return null;

            var decision = panel.lastDecision;

            Debug.Log($"Made choice {decision.answerIndex}: '{decision.choices[decision.answerIndex]}'");
        }
    }
}