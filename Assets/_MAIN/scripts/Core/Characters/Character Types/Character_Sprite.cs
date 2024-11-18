using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CHARACTERS
{
    public class Character_Sprite : Character
    {
        private const string SPRITE_RENDERERS_PARENT_NAME = "Renderers";
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();

        public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();

        private string artAssetsDirectory = "";

        public Character_Sprite(string name, CharacterConfig_Data config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab) 
        {
            rootCG.alpha = 0f;
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
                Image rendererImage  = child.GetComponent<Image>();

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

        public Sprite GetSprite(string spriteName)
        {
            if (config.characterType == CharacterType.SpriteSheet)
            {
                return null;
            }
            
            else
            {
                return Resources.Load<Sprite>($"{artAssetsDirectory}/{spriteName}");
            }
        }

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
    }
}