using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    public abstract class Character
    {
        public const bool ENABLE_ON_START = false;
        private const float UNHIGHLIGHTED_DARKEN_STRENGTH = 0.65f;
        public const bool DEFAULT_ORIENTATION_IS_FACING_LEFT = false;
        public const string ANIMATION_REFRESH_TRIGGER = "Refresh";

        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        public CharacterConfig_Data config;
        public Animator animator;
        public Color color { get; protected set; } = Color.white;
        protected Color displayColor => highlighted ? highlightedColor : unhighlightedColor;
        protected Color highlightedColor => color;
        protected Color unhighlightedColor => new Color(color.r * UNHIGHLIGHTED_DARKEN_STRENGTH, color.g * UNHIGHLIGHTED_DARKEN_STRENGTH, color.b * UNHIGHLIGHTED_DARKEN_STRENGTH, color.a);
        public bool highlighted { get; protected set; } = true;
        protected bool facingLeft = DEFAULT_ORIENTATION_IS_FACING_LEFT;
        public int priority { get; protected set; }

        protected CharacterManager manager => CharacterManager.instance;
        public DialogueSystem dialogueSystem => DialogueSystem.instance;

        //Coroutines
        protected Coroutine co_revealing, co_hiding;
        protected Coroutine co_moving;
        protected Coroutine co_changingColor;
        protected Coroutine co_highlighting;
        protected Coroutine co_flipping;

        //Coroutine Booleans
        public bool isRevealing => co_revealing != null;
        public bool isHiding => co_hiding != null;
        public bool isMoving => co_moving != null;
        public virtual bool isVisible { get; set; }
        public bool isChangingColor => co_changingColor != null;
        public bool isHighlighting => (highlighted && co_highlighting != null);
        public bool isUnHighlighting => (!highlighted && co_highlighting != null);
        public bool isFacingLeft => facingLeft;
        public bool isFacingRight => !facingLeft;
        public bool isFlipping => co_flipping != null;

        public Character(string name, CharacterConfig_Data config, GameObject prefab)
        {
            this.name = name;
            displayName = name;
            this.config = config;

            if (prefab != null)
            {
                GameObject ob = Object.Instantiate(prefab, manager.characterPanel);
                ob.name = manager.FormatCharacterPath(manager.characterPrefabNameFormat, name);
                ob.SetActive(true);
                root = ob.GetComponent<RectTransform>();
                animator = root.GetComponentInChildren<Animator>();
            }
        }

        public enum CharacterType 
        {
            Text,
            Sprite,
            SpriteSheet
        }

        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });

        public Coroutine Say(List<string> dialogue)
        {
            dialogueSystem.ShowSpeakerName(displayName);
            UpdateTextCustomizationOnScreen();
            return dialogueSystem.Say(dialogue);
        }

        public void SetNameColor(Color color) => config.nameColor = color;
        public void SetDialogueColor(Color color) => config.dialogueColor = color;
        public void SetNamefont(TMP_FontAsset font) => config.nameFont = font;
        public void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont = font;

        public void UpdateTextCustomizationOnScreen() => dialogueSystem.ApplySpeakerDataToContainer(config);
        public void ResetConfigurationData() => CharacterManager.instance.GetCharacterConfig(name, getOriginal: true);

        public virtual Coroutine Show(float speedMultiplier = 1f)
        {
            if (isRevealing)
                manager.StopCoroutine(co_revealing);

            if (isHiding)
                manager.StopCoroutine(co_hiding);

            co_revealing = manager.StartCoroutine(ShowingOrHiding(true, speedMultiplier));
            return co_revealing;
        }

        public virtual Coroutine Hide(float speedMultiplier = 1f)
        {
            if (isHiding)
                manager.StopCoroutine(co_hiding);

            if (isRevealing)
                manager.StopCoroutine(co_revealing);

            co_hiding = manager.StartCoroutine(ShowingOrHiding(false, speedMultiplier));
            return co_hiding;
        }

        public virtual IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f)
        {
            Debug.Log("Show/Hide cannot be called from the base Character class.");
            yield return null;
        }

        public virtual void SetPosition(Vector2 position)
        {
            if (root == null)
                return;

            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPosToRelativeCharaAnchorTargets(position);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;
        }

        public virtual Coroutine MoveToPosition(Vector2 position, float speed = 2f, bool smooth = false)
        {
            if (root == null)
                return null;

            if (isMoving)
                manager.StopCoroutine(co_moving);

            co_moving = manager.StartCoroutine(MovingToPosition(position, speed, smooth));

            return co_moving;
        }

        private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth)
        {
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPosToRelativeCharaAnchorTargets(position);
            Vector2 padding = root.anchorMax - root.anchorMin;

            while (root.anchorMin != minAnchorTarget)
            {
                root.anchorMin = smooth ?
                    Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime)
                    : Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);

                root.anchorMax = root.anchorMin + padding;

                if (smooth && Vector2.Distance(root.anchorMin, minAnchorTarget) <= 0.001f)
                {
                    root.anchorMin = minAnchorTarget;
                    root.anchorMax = maxAnchorTarget;
                    break;
                }

                yield return null;
            }

            co_moving = null;
        }

        protected (Vector2, Vector2) ConvertUITargetPosToRelativeCharaAnchorTargets(Vector2 position)
        {
            Vector2 padding = root.anchorMax - root.anchorMin;

            float max_X = 1f - padding.x;
            float max_Y = 1f - padding.y;

            Vector2 minAnchorTarget = new Vector2(max_X * position.x, max_Y * position.y);
            Vector2 maxAnchorTarget = minAnchorTarget + padding;

            return (minAnchorTarget, maxAnchorTarget);
        }

        public virtual void SetColor(Color color)
        {
            this.color = color;
        }

        public Coroutine TransitionColor(Color color, float speed = 1)
        {
            this.color = color;

            if (isChangingColor)
                manager.StopCoroutine(co_changingColor);

            co_changingColor = manager.StartCoroutine(ChangingColor(displayColor, speed));

            return co_changingColor;
        }

        public virtual IEnumerator ChangingColor(Color color, float speed)
        {
            Debug.Log("Color changing is not applicable on this character type!");
            yield return null;
        }

        public Coroutine Highlight(float speed = 1f, bool immediate = false)
        {
            if (isHighlighting || isUnHighlighting)
                manager.StopCoroutine(co_highlighting);

            highlighted = true;
            co_highlighting = manager.StartCoroutine(Highlighting(speed, immediate));

            return co_highlighting;
        }

        public Coroutine Unhighlight(float speed = 1f, bool immediate = false)
        {
            if (isHighlighting || isUnHighlighting)
                manager.StopCoroutine(co_highlighting);

            highlighted = false;
            co_highlighting = manager.StartCoroutine(Highlighting(speed, immediate));

            return co_highlighting;
        }

        public virtual IEnumerator Highlighting(float speedMultiplier, bool immediate = false)
        {
            Debug.Log("Highlighting is not available on this character type!");
            yield return null;
        }

        public Coroutine Flip(float speed = 1, bool immediate = false)
        {
            if (isFacingRight)
                return FaceLeft(speed, immediate);
            else
                return FaceRight(speed, immediate);
        } 

        public Coroutine FaceLeft(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                manager.StopCoroutine(co_flipping);

            facingLeft = true;
            co_flipping = manager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));

            return co_flipping;
        }

        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                manager.StopCoroutine(co_flipping);

            facingLeft = false;
            co_flipping = manager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));

            return co_flipping;
        }

        public virtual IEnumerator FaceDirection(bool faceLeft, float speedMultiplier,  bool immediate)
        {
            Debug.Log("Cannot flip a character of this type!");
            yield return null;
        }

        public void SetPriority(int priority, bool autoSortCharactersOnUI = true)
        {
            this.priority = priority;
            if (autoSortCharactersOnUI)
                manager.SortCharacters();
        }

        public void Animate(string animation)
        {
            animator.SetTrigger(animation);
        }

        public void Animate(string animation, bool state)
        {
            animator.SetBool(animation, state);
            animator.SetTrigger(ANIMATION_REFRESH_TRIGGER);
        }

        public virtual void OnReceiveCastingExpression(int layer, string expression)
        {
            return;
        }
    }
}