using CHARACTERS;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [CreateAssetMenu(fileName = "Dialogue System Configuration", menuName = "Dialogue System/Dialogue Configuration Asset")]
    public class DialogueSystemConfiguration_SO : ScriptableObject
    {
        public CharacterConfig_SO characterConfigurationAsset;

        public Color defaultTextColor = new Color(245, 243, 240, 255);
        public TMP_FontAsset defaultFont;
    }
}