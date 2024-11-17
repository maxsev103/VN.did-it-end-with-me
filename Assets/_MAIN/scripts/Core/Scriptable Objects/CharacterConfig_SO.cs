using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    [CreateAssetMenu(fileName = "Character Configuration Asset", menuName = "Dialogue System/Character Configuration Asset")]
    public class CharacterConfig_SO : ScriptableObject
    {
        public CharacterConfig_Data[] characters;

        public CharacterConfig_Data GetConfig(string characterName)
        {
            characterName = characterName.ToLower();

            for (int i = 0; i < characters.Length; i++)
            {
                CharacterConfig_Data data = characters[i];

                if (string.Equals(characterName, data.name.ToLower()) || string.Equals(characterName, data.alias.ToLower()))
                    return data.Copy();
            }

            return CharacterConfig_Data.Default;
        }
    }
}