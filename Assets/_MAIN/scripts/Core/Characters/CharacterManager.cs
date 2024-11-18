using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

namespace CHARACTERS
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager instance { get; private set; }
        private Dictionary<string, Character> characters = new Dictionary<string, Character>();

        private CharacterConfig_SO config => DialogueSystem.instance.config.characterConfigurationAsset;

        private void Awake()
        {
            instance = this;
        }

        public CharacterConfig_Data GetCharacterConfig(string characterName)
        {
            return config.GetConfig(characterName);
        }

        public Character CreateCharacter(string characterName)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogWarning($"A Character called '{characterName}' already exists. Character creation failed.");
                return null;
            }

            CHARACTER_INFO info = GetCharacterInfo(characterName);

            Character character = CreateCharacterFromInfo(info);

            characters.Add(characterName.ToLower(), character);

            return character;
        }

        public Character GetCharacter(string characterName, bool createIfNotExisting = false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
                return characters[characterName.ToLower()];
            else if (createIfNotExisting)
                return CreateCharacter(characterName);

            return null;
        }

        private CHARACTER_INFO GetCharacterInfo(string characterName)
        {
            CHARACTER_INFO result = new CHARACTER_INFO();

            result.name = characterName;
            result.config = config.GetConfig(characterName);

            return result;
        }

        private Character CreateCharacterFromInfo(CHARACTER_INFO info)
        {
            CharacterConfig_Data config = info.config;

            switch (info.config.characterType)
            {
                case Character.CharacterType.Text:
                    return new Character_Text(info.name, config);
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new Character_Sprite(info.name, config);
                default:
                    return null;  
            }
        }

        private class CHARACTER_INFO
        {
            public string name = "";

            public CharacterConfig_Data config = null;
        }
    }
}