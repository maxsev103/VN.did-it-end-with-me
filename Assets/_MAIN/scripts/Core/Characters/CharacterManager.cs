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

        private const string CHARACTER_CASTING_ID = " as ";
        private const string CHARACTER_NAME_ID = "<charname>";
        private string characterRootPath => $"Character Sprites/{CHARACTER_NAME_ID}";
        private string characterPrefabPath => $"{characterRootPath}/Character - [{CHARACTER_NAME_ID}]";

        [SerializeField] private RectTransform _characterPanel = null;
        public RectTransform characterPanel => _characterPanel;

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

            string[] nameData = characterName.Split(CHARACTER_CASTING_ID, System.StringSplitOptions.RemoveEmptyEntries);
            result.name = nameData[0];
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;

            result.name = characterName;
            result.config = config.GetConfig(result.castingName);
            result.prefab = GetPrefabForCharacter(result.castingName);

            return result;
        }

        private GameObject GetPrefabForCharacter(string characterName)
        {
            string prefabPath = FormatCharacterPath(characterPrefabPath, characterName);
            return Resources.Load<GameObject>(prefabPath);
        }

        private string FormatCharacterPath(string path, string characterName) => path.Replace(CHARACTER_NAME_ID, characterName);

        private Character CreateCharacterFromInfo(CHARACTER_INFO info)
        {
            CharacterConfig_Data config = info.config;

            switch (info.config.characterType)
            {
                case Character.CharacterType.Text:
                    return new Character_Text(info.name, config);
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new Character_Sprite(info.name, config, info.prefab);
                default:
                    return null;  
            }
        }

        private class CHARACTER_INFO
        {
            public string name = "";
            public string castingName = "";

            public CharacterConfig_Data config = null;

            public GameObject prefab = null;
        }

    }
}