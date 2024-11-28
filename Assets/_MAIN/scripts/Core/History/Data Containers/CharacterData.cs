using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

namespace HISTORY
{
    [System.Serializable]
    public class CharacterData
    {
        public string characterName;
        public string displayName;
        public bool enabled;
        public Color color;
        public int priority;
        public bool isHighlighted;
        public bool isFacingLeft;
        public Vector2 position;
        public CharacterConfigCache characterConfigCache;

        public string dataJSON;

        [System.Serializable]
        public class CharacterConfigCache
        {
            public string name;
            public string alias;

            public Character.CharacterType characterType;

            public Color nameColor;
            public Color dialogueColor;

            public string nameFont;
            public string dialogueFont;

            public float nameFontSize;
            public float dialogueFontSize;

            public CharacterConfigCache(CharacterConfig_Data reference)
            {
                name = reference.name;
                alias = reference.alias;
                characterType = reference.characterType;

                nameColor = reference.nameColor;
                dialogueColor = reference.dialogueColor;

                nameFont = FilePaths.resources_fonts + reference.nameFont.name;
                dialogueFont = FilePaths.resources_fonts + reference.dialogueFont.name;

                nameFontSize = reference.namefontSize;
                dialogueFontSize = reference.dialoguefontSize;
            }
        }

        public static List<CharacterData> Capture()
        {
            List<CharacterData> characters = new List<CharacterData>();

            foreach (var character in CharacterManager.instance.allCharacters)
            {
                if (!character.isVisible)
                    continue;

                CharacterData entry = new CharacterData();
                entry.characterName = character.name;
                entry.displayName = character.displayName;
                entry.enabled = character.isVisible;
                entry.color = character.color;
                entry.priority = character.priority;
                entry.isHighlighted = character.highlighted;
                entry.isFacingLeft = character.isFacingLeft;
                entry.position = character.targetPosition;
                entry.characterConfigCache = new CharacterConfigCache(character.config);

                // get the sprite info
                SpriteData sdata = new SpriteData();
                sdata.layers = new List<SpriteData.LayerData>();

                Character_Sprite sc = character as Character_Sprite;
                foreach (var layer in sc.layers)
                {
                    var layerData = new SpriteData.LayerData();
                    layerData.color = layer.renderer.color;
                    layerData.spriteName = layer.renderer.sprite.name;
                    sdata.layers.Add(layerData);
                }

                entry.dataJSON = JsonUtility.ToJson(sdata);

                characters.Add(entry);
            }

            return characters;
        }

        [System.Serializable]
        public class SpriteData
        {
            public List<LayerData> layers;

            [System.Serializable]
            public class LayerData
            {
                public string spriteName;
                public Color color;
            }
        }
    }
}