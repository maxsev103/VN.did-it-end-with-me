using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CHARACTERS
{
    public class Character_Sprite : Character
    {
        private const string SPRITE_RENDERERS_PARENT_NAME = "Renderers";
        private const string SPRITESHEET_DEFAULT_SHEET_NAME = "Default"; // if using a spritesheet for generic, single appearance sprites
        private const char SPRITESHEET_TEX_SPRITE_DELIMITER = '-';

        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();

        public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();

        private string artAssetsDirectory = "";

        public override bool isVisible
        { 
            get { return isRevealing || rootCG.alpha == 1; }
            set { rootCG.alpha = value ? 1 : 0; }
        }

        public Character_Sprite(string name, CharacterConfig_Data config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab) 
        {
            rootCG.alpha = ENABLE_ON_START ? 1 : 0;
            artAssetsDirectory = rootAssetsFolder + "/Images";

            GetLayers();

            Debug.Log($"Created Sprite Character: '{name}'");
        }

        private void GetLayers()
        {
            Transform rendererRoot = animator.transform.Find(SPRITE_RENDERERS_PARENT_NAME);

            if (rendererRoot == null)
                return;

            for (int i = 0; i < rendererRoot.transform.childCount; i++)
            {
                Transform child = rendererRoot.transform.GetChild(i);
                Image rendererImage = child.GetComponentInChildren<Image>();

                if (rendererImage != null)
                {
                    CharacterSpriteLayer layer = new CharacterSpriteLayer(rendererImage, i);
                    layers.Add(layer);
                    child.name = $"Layer: {i}";
                }
            }
        }

        public void SetSprite(Sprite sprite, int layer = 0)
        {
            layers[layer].SetSprite(sprite);
        }

        /// <summary>
        /// Gets the passed sprite name regardless of whether they are a sprite character or a spritesheet character
        /// </summary>
        /// <param name="spriteName"></param>
        /// <returns></returns>
        public Sprite GetSprite(string spriteName)
        {
            // checks if the character is a spritesheet character
            if (config.characterType == CharacterType.SpriteSheet)
            {
                // if it is, then split the passed spriteName to check if the spritesheet sprite delimiter is present
                string[] data = spriteName.Split(SPRITESHEET_TEX_SPRITE_DELIMITER);
                Sprite[] spriteArray = new Sprite[0];

                // if a delimiter was used, then get the texture name and the sprite to be fetched and load it
                if (data.Length == 2)
                {
                    string textureName = data[0];
                    spriteName = data[1];
                    spriteArray = Resources.LoadAll<Sprite>($"{artAssetsDirectory}/{textureName}");
                }
                // if a texture name was not specified, use the textureName "default"
                else
                {
                    spriteArray = Resources.LoadAll<Sprite>($"{artAssetsDirectory}/{SPRITESHEET_DEFAULT_SHEET_NAME}");
                }

                if (spriteArray.Length == 0)
                    Debug.LogWarning($"Character '{name}' does not have a default art asset called '{SPRITESHEET_DEFAULT_SHEET_NAME}'");

                return Array.Find(spriteArray, sprite => sprite.name == spriteName);
            }
            // if it isn't a spritesheet character, then it is a sprite character and the assets can be found using the passed
            // spriteName on its own
            else
            {
                return Resources.Load<Sprite>($"{artAssetsDirectory}/{spriteName}");
            }
        }

        public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 1f)
        {
            CharacterSpriteLayer spriteLayer = layers[layer];

            return spriteLayer.TransitionSprite(sprite, speed);
        }

        // Fades a character in or out
        public override IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1.0f : 0.0f;
            CanvasGroup self = rootCG;

            while (self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime);
                yield return null;
            }

            co_revealing = null;
            co_hiding = null;
        }

        public override void SetColor(Color color)
        {
            base.SetColor(color);

            color = displayColor;

            foreach (CharacterSpriteLayer layer in layers)
            {
                layer.StopChangingColor();
                layer.SetColor(color);
            }
        }

        public override IEnumerator ChangingColor(Color color, float speed)
        {
            foreach (CharacterSpriteLayer layer in layers)
                layer.TransitionColor(color, speed);

            yield return null;

            while (layers.Any(l => l.isChangingColor))
            {
                yield return null;
            }

            co_changingColor = null;
        }

        public override IEnumerator Highlighting(bool highlight, float speedMultiplier)
        {
            Color targetColor = displayColor;

            foreach (CharacterSpriteLayer layer in layers)
                layer.TransitionColor(targetColor, speedMultiplier);

            yield return null;

            while (layers.Any(l => l.isChangingColor))
            {
                yield return null;
            }

            co_highlighting = null;
        }
    }
}