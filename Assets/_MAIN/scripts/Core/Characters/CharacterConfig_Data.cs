using AYellowpaper.SerializedCollections;
using DIALOGUE;
using System;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    [Serializable]
    public class CharacterConfig_Data
    {
        public string name;
        public string alias;
        public Character.CharacterType characterType;

        // custom colors for a specific character
        public Color nameColor;
        public Color dialogueColor;

        public TMP_FontAsset nameFont;
        public TMP_FontAsset dialogueFont;

        public float namefontSize;
        public float dialoguefontSize;

        [SerializedDictionary("Path / ID", "Sprite")]
        public SerializedDictionary<string, Sprite> sprites = new SerializedDictionary<string, Sprite>();

        public CharacterConfig_Data Copy()
        {
            CharacterConfig_Data result = new CharacterConfig_Data();

            result.name = name;
            result.alias = alias;
            result.characterType = characterType;

            result.nameFont = nameFont;
            result.dialogueFont = dialogueFont;

            result.nameColor = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a);
            result.dialogueColor = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a);

            result.namefontSize = namefontSize;
            result.dialoguefontSize = dialoguefontSize;

            return result;
        }

        private static Color defaultColor => DialogueSystem.instance.config.defaultTextColor;
        private static TMP_FontAsset defaultFont => DialogueSystem.instance.config.defaultFont;
        public static CharacterConfig_Data Default
        {
            get
            {
                CharacterConfig_Data result = new CharacterConfig_Data();

                result.name = "";
                result.alias = "";
                result.characterType = Character.CharacterType.Text;

                result.nameFont = defaultFont;
                result.dialogueFont = defaultFont;

                result.nameColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);
                result.dialogueColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);

                result.dialoguefontSize = DialogueSystem.instance.config.defaultDialogueFontSize;
                result.namefontSize = DialogueSystem.instance.config.defaultNameFontSize;

                return result;
            }
        }
    }
}