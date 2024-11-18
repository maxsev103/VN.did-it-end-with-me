using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    public class Character_Sprite : Character
    {
        public Character_Sprite(string name, CharacterConfig_Data config) : base(name, config) 
        {
            Debug.Log($"Created Sprite Character: '{name}'");
        }
    }
}