using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DIALOGUE
{
    public class DialogueContinuationPrompt : MonoBehaviour
    {
        private RectTransform root;

        [SerializeField] private Animator anim;
        [SerializeField] private TextMeshProUGUI tmpro;
        [SerializeField] private RectTransform dialogueBoxRoot;

        public bool isShowing => anim.gameObject.activeSelf;

        void Start()
        {
            root = GetComponent<RectTransform>();
        }

        public void Show()
        {
            if (tmpro.text == string.Empty)
            {
                if (isShowing)
                    Hide();
                return;
            }

            tmpro.ForceMeshUpdate();

            anim.gameObject.SetActive(true);
            root.transform.SetParent(dialogueBoxRoot.transform);

            Vector2 parentBottomRight = dialogueBoxRoot.rect.xMax * Vector2.right +
                      dialogueBoxRoot.rect.yMin * Vector2.up;
            Vector2 offset = new Vector2(-40, 50);
            float targetPosX = parentBottomRight.x + offset.x;
            float targetPosY = parentBottomRight.y + offset.y;

            root.transform.localPosition = new Vector3(targetPosX, targetPosY, 0);
        }

        public void Hide()
        {
            anim.gameObject.SetActive(false);
        }
    }
}