using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using System.Linq;
using Unity.VisualScripting;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Characters : CMDDatabaseExtension
    {
        private static string[] PARAM_IMMEDIATE => new string[] { "-i", "-immediate" };
        private static string[] PARAM_ENABLE => new string[] { "-e", "-enabled" };
        private static string PARAM_XPOS => "-x";
        private static string PARAM_YPOS => "-y";
        private static string[] PARAM_SPEED => new string[] { "-spd", "-speed" };
        private static string[] PARAM_SMOOTH => new string[] { "-sm", "-smooth" };
        private static string[] PARAM_ANIMATION => new string[] { "-anim", "-animation" };
        private static string[] PARAM_ANIMATION_STATE => new string[] { "-st", "-state" };

        new public static void Extend(CommandDatabase database)
        {
            // add global character commands (commands can be applied to all characters)
            database.AddCommand("createcharacter", new Action<string[]>(CreateCharacter));
            database.AddCommand("movecharacter", new Func<string[], IEnumerator>(MoveCharacter));
            database.AddCommand("show", new Func<string[], IEnumerator>(ShowAll));
            database.AddCommand("hide", new Func<string[], IEnumerator>(HideAll));
            database.AddCommand("sort", new Action<string[]>(SortCharacters));
            database.AddCommand("highlight", new Func<string[], IEnumerator>(HighlightAll));
            database.AddCommand("h", new Func<string[], IEnumerator>(HighlightAll));
            database.AddCommand("u", new Func<string[], IEnumerator>(UnhighlightAll));
            database.AddCommand("unhighlight", new Func<string[], IEnumerator>(UnhighlightAll));
            database.AddCommand("flip", new Func<string[], IEnumerator>(FlipAll));

            // add commands to characters
            CommandDatabase baseCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_BASE);
            baseCommands.AddCommand("move", new Func<string[], IEnumerator>(MoveCharacter));
            baseCommands.AddCommand("show", new Func<string[], IEnumerator>(Show));
            baseCommands.AddCommand("hide", new Func<string[], IEnumerator>(Hide));
            baseCommands.AddCommand("setpriority", new Action<string[]>(SetPriority));
            baseCommands.AddCommand("setcolor", new Func<string[], IEnumerator>(SetColor));
            baseCommands.AddCommand("setposition", new Action<string[]>(SetPosition));
            baseCommands.AddCommand("highlight", new Func<string[], IEnumerator>(Highlight));
            baseCommands.AddCommand("unhighlight", new Func<string[], IEnumerator>(Unhighlight));
            baseCommands.AddCommand("flip", new Func<string[], IEnumerator>(Flip));
            baseCommands.AddCommand("animate", new Action<string[]>(Animate));

            // add character type specific commands
            CommandDatabase spriteCommands = CommandManager.instance.CreateSubDatabase(CommandManager.DATABASE_CHARACTERS_SPRITE);
            spriteCommands.AddCommand("setsprite", new Func<string[], IEnumerator>(SetSprite));
        }

        public static void CreateCharacter(string[] data)
        {
            // Format: CreateCharacter(character name [-e t/f -i t/f]); [parameter] = can be omitted
            string characterName = data[0];
            bool enable = false;
            bool immediate = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_ENABLE, out enable, defaultValue: false);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            Character character = CharacterManager.instance.CreateCharacter(characterName);

            if (!enable)
                return;

            if (immediate)
                character.isVisible = true;
            else
                character.Show();
        }

        public static void SortCharacters(string[] data)
        {
            // Format: Sort()
            CharacterManager.instance.SortCharacters(data);
        }

        public static IEnumerator MoveCharacter(string[] data)
        {
            // Format: MoveCharacter(character name -x xvalue -y yvalue [-spd speedValue -sm t/f -i t/f]); [parameter] = can be omitted
            // Note: either X or Y value can be omitted so long as one or the other was specified i.e. only x value was specified and vice versa
            string characterName = data[0];
            Character character = CharacterManager.instance.GetCharacter(characterName);

            if (character == null)
                yield break;

            float x = 0, y = 0;
            float speed = 1;
            bool smooth = false;
            bool immediate = false;

            var parameters = ConvertDataToParameters(data);

            // try to get the x axis position
            parameters.TryGetValue(PARAM_XPOS, out x);

            // try to get the y axis position
            parameters.TryGetValue(PARAM_YPOS, out y);

            // try to get the speed
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1);

            // try to get the smoothing
            parameters.TryGetValue(PARAM_SMOOTH, out smooth, defaultValue: false);

            //try to get immediate setting of position
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            Vector2 position = new Vector2(x, y);

            if (immediate)
                character.SetPosition(position);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetPosition(position); });
                yield return character.MoveToPosition(position, speed, smooth);
            }

        }

        public static IEnumerator ShowAll(string[] data)
        {
            // Format: Show(character name+ [-spd speedValue -i t/f]); [parameter] = can be omitted, '+' = one or more
            List<Character> characters = new List<Character>();
            bool immediate = false;
            float speed = 1f;

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfNotExisting: false);
                if (character != null)
                {
                    characters.Add(character);
                }
            }

            if (characters.Count == 0)
                yield break;

            // convert data array to parameter container
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            foreach (Character character in characters)
            {
                if (immediate)
                    character.isVisible = true;
                else
                    character.Show(speed);
            }

            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                        character.isVisible = true;
                });

                while (characters.Any(c => c.isRevealing))
                    yield return null;
            }
        }

        public static IEnumerator HideAll(string[] data) 
        {
            // Format: Hide(character name+ [-spd speedValue -i t/f]); [parameter] = can be omitted, '+' = one or more
            List<Character> characters = new List<Character>();
            bool immediate = false;
            float speed = 1f;

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfNotExisting: false);
                if (character != null)
                {
                    characters.Add(character);
                }
            }

            if (characters.Count == 0)
                yield break;

            // convert data array to parameter container
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            foreach (Character character in characters)
            {
                if (immediate)
                    character.isVisible = false;
                else
                    character.Hide(speed);
            }

            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (Character character in characters)
                        character.isVisible = false;
                });

                while (characters.Any(c => c.isHiding))
                    yield return null;
            }
        }

        public static IEnumerator HighlightAll(string[] data)
        {
            // Format: Highlight(character name+ [-only t/f -i t/f]); [parameter] = can be omitted, '+' = one or more
            // Note: -only has the shorthand -o, meaning that you only want the specified character to be highlighted and unhighlight the rest
            List<Character> characters = new List<Character>();
            bool immediate = false;
            bool handleUnspecifiedCharacters = true;
            List<Character> unspecifiedCharacters = new List<Character>();

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfNotExisting: false);
                if (character != null)
                {
                    characters.Add(character);
                }
            }

            if (characters.Count == 0)
                yield break;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(new string[] { "-o", "-only" }, out handleUnspecifiedCharacters, defaultValue: true);

            foreach (Character character in characters)
            {
                character.Highlight(immediate: immediate);
            }

            // if we are forcing any unspecified characters to use the opposite highlighted status
            if (handleUnspecifiedCharacters)
            {
                foreach (Character character in CharacterManager.instance.allCharacters)
                {
                    if (characters.Contains(character))
                        continue;

                    unspecifiedCharacters.Add(character);
                    character.Unhighlight(immediate: immediate);
                }
            }

            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (var character in characters)
                        character.Highlight(immediate: true);

                    if (handleUnspecifiedCharacters) return;

                    foreach (var character in unspecifiedCharacters)
                        character.Unhighlight(immediate: true);
                });

                while (characters.Any(c => c.isHighlighting) || (handleUnspecifiedCharacters && unspecifiedCharacters.Any(uc => uc.isUnHighlighting)))
                    yield return null;
            }
        }

        public static IEnumerator UnhighlightAll(string[] data)
        {
            // Format: Unhighlight(character name+ [-only t/f -i t/f]); [parameter] = can be omitted, '+' = one or more
            // Note: -only has the shorthand -o, meaning that you only want the specified character to be unhighlighted and highlight the rest
            List<Character> characters = new List<Character>();
            bool immediate = false;
            bool handleUnspecifiedCharacters = true;
            List<Character> unspecifiedCharacters = new List<Character>();

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s, createIfNotExisting: false);
                if (character != null)
                {
                    characters.Add(character);
                }
            }

            if (characters.Count == 0)
                yield break;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(new string[] { "-o", "-only" }, out handleUnspecifiedCharacters, defaultValue: true);

            foreach (Character character in characters)
            {
                character.Unhighlight(immediate: immediate);
            }

            // if we are forcing any unspecified characters to use the opposite highlighted status
            if (handleUnspecifiedCharacters)
            {
                foreach (Character character in CharacterManager.instance.allCharacters)
                {
                    if (characters.Contains(character))
                        continue;

                    unspecifiedCharacters.Add(character);
                    character.Highlight(immediate: immediate);
                }
            }

            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (var character in characters)
                        character.Unhighlight(immediate: true);

                    if (handleUnspecifiedCharacters) return;

                    foreach (var character in unspecifiedCharacters)
                        character.Highlight(immediate: true);
                });

                while (characters.Any(c => c.isUnHighlighting) || (handleUnspecifiedCharacters && unspecifiedCharacters.Any(uc => uc.isHighlighting)))
                    yield return null;
            }
        }

        public static IEnumerator FlipAll(string[] data)
        {
            List<Character> characters = new List<Character>();
            float speed = 1f;
            bool immediate = false;

            foreach (string s in data)
            {
                Character character = CharacterManager.instance.GetCharacter(s);
                if (character != null)
                    characters.Add(character);
            }

            if (characters.Count == 0)
                yield break;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            if (!immediate)
                parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            foreach (Character character in characters)
            {
                if (immediate)
                    character.Flip(immediate: immediate);
                else
                    character.Flip(speed: speed);
            }

            if (!immediate)
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() =>
                {
                    foreach (var character in characters)
                    {
                        if(character.isFacingLeft)
                            character.FaceDirection(faceLeft: true, speed, immediate: true);
                        else
                            character.FaceDirection(faceLeft: false, speed, immediate: true);
                    }
                });

                while (characters.Any(c => c.isFlipping))
                    yield return null;
            }
        }

        #region BASE CHARACTER COMMANDS
        public static IEnumerator Show(string[] data)
        {
            // Format: characterName.Show()
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
                yield break;

            bool immediate = false;
            float speed = 1f;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            if (immediate)
                character.isVisible = true;
            else
            {
                // long running process should have a stop action to cancel out the coroutine and run logic that completes the command
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = true; });

                yield return character.Show(speed);
            }
        }

        public static IEnumerator Hide(string[] data)
        {
            //Format: characterName.Hide()
            Character character = CharacterManager.instance.GetCharacter(data[0]);

            if (character == null)
                yield break;

            bool immediate = false;
            float speed = 1f;
            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            if (immediate)
                character.isVisible = false;
            else
            {
                // long running process should have a stop action to cancel out the coroutine and run logic that completes the command
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { if (character != null) character.isVisible = false; });

                yield return character.Hide(speed);
            }
        }

        public static void SetPosition(string[] data)
        {
            // Format: characterName.SetPosition(-x xvalue -y yvalue)
            Character character = CharacterManager.instance.GetCharacter(data[0]);
            float x = 0, y = 0;

            if (character == null || data.Length < 2)
                return;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_XPOS, out x, defaultValue: 0);
            parameters.TryGetValue(PARAM_YPOS, out y, defaultValue: 0);

            character.SetPosition(new Vector2(x, y));
        }

        public static void SetPriority(string[] data)
        {
            // Format: characterName.SetPriority(priorityValue)
            Character character = CharacterManager.instance.GetCharacter(data[0]);
            int priority;

            if (character == null || data.Length < 2)
                return;

            if (!int.TryParse(data[1], out priority))
                priority = 0;

            character.SetPriority(priority);
        }

        public static IEnumerator SetColor(string[] data)
        {
            // Format: characterName.SetColor(colorName)
            Character character = CharacterManager.instance.GetCharacter(data[0]);
            string colorName;
            float speed;
            bool immediate;

            if (character == null || data.Length < 2)
                yield break;

            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-c", "-color" }, out colorName);

            bool specifiedSpeed = parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            if (!specifiedSpeed)
                parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: true);
            else
                immediate = false;

            Color color = Color.white;
            color = color.GetColorFromName(colorName);

            if (immediate)
                character.SetColor(color);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetColor(color); });
                character.TransitionColor(color, speed);
            }

            yield break;
        }

        public static IEnumerator Highlight(string[] data)
        {
            // Format: characterName.Highlight([-i t/f]); [parameter] = can be omitted
            Character character = CharacterManager.instance.GetCharacter(data[0]) as Character;
            bool immediate = false;

            if (character == null)
                yield break;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            if (immediate)
                character.Highlight(immediate: true);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.Highlight(immediate: true); });
                yield return character.Highlight();
            }
        }

        public static IEnumerator Unhighlight(string[] data)
        {
            // Format: characterName.Unhighlight([-i t/f]); [parameter] = can be omitted
            Character character = CharacterManager.instance.GetCharacter(data[0]) as Character;
            bool immediate = false;

            if (character == null)
                yield break;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            if (immediate)
                character.Unhighlight(immediate: true);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.Unhighlight(immediate: true); });
                yield return character.Unhighlight();
            }
        }

        public static IEnumerator Flip(string[] data)
        {
            Character_Sprite character = CharacterManager.instance.GetCharacter(data[0]) as Character_Sprite;
            float speed = 1f;
            bool immediate = false;

            if (character == null)
                yield break;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            if (!immediate)
                parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            if (immediate)
                character.Flip(immediate: immediate);
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => {
                    if (character.isFacingLeft)
                        character.FaceDirection(faceLeft: true, speed, immediate: true);
                    else
                        character.FaceDirection(faceLeft:false, speed, immediate: true);
                });

                yield return character.Flip(speed);
            }

        }

        public static void SingleAnimation(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]) as Character;
            string animation;

            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_ANIMATION, out animation);

            if (animation != string.Empty)
                character.Animate(animation);
        }

        public static void ContinuousAnimation(string[] data)
        {
            Character character = CharacterManager.instance.GetCharacter(data[0]) as Character;
            string animation;
            bool state;

            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(PARAM_ANIMATION, out animation);
            parameters.TryGetValue(PARAM_ANIMATION_STATE, out state, defaultValue: true);

            if (animation != string.Empty)
                character.Animate(animation, state);
        }

        public static void Animate(string[] data)
        {
            if (data.Length == 1)
                SingleAnimation(data);
            else
                ContinuousAnimation(data);
        }

        #endregion

        #region SPRITE CHARACTER COMMANDS

        public static IEnumerator SetSprite(string[] data)
        {
            // Format: spriteCharacterName.SetSprite(spriteName -l layerNumber [-spd speedValue || -i t/f]); [param1 || param2] = only one or the other can be omitted
            Character_Sprite character = CharacterManager.instance.GetCharacter(data[0], createIfNotExisting: false) as Character_Sprite;
            int layer = 0;
            string spriteName;
            bool immediate = false;
            float speed;

            if (character == null || data.Length < 2)
                yield break;

            var parameters = ConvertDataToParameters(data, startingIndex: 1);

            parameters.TryGetValue(new string[] { "-s", "-sprite" }, out spriteName);
            parameters.TryGetValue(new string[] { "-l", "-layer" }, out layer, defaultValue: 0);

            bool specifiedSpeed = parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);

            if (!specifiedSpeed)
                parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            // run the logic
            Sprite sprite = character.GetSprite(spriteName);

            if (sprite == null)
                yield break;
            
            if (immediate) 
            {
                character.SetSprite(sprite, layer);
            }
            else
            {
                CommandManager.instance.AddTerminationActionToCurrentProcess(() => { character?.SetSprite(sprite, layer); });
                yield return character.TransitionSprite(sprite, layer, speed);
            }
        }

        #endregion
    }
}