using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

namespace DIALOGUE
{
    public class DL_SpeakerData
    {
        public string name, castName;
        public string displayName => hasNameCasting ? castName : name;
        public Vector2 castPosition;
        public List<(int layer, string expression)> CastExpressions { get; set; }

        public bool hasNameCasting => castName != string.Empty;
        public bool isCastingPosition = false;
        public bool isCastingExpressions => CastExpressions.Count > 0;

        public bool makeCharacterEnter = false;
        public bool makeCharacterEnterLeft = false;
        public bool makeCharacterEnterRight = false;

        private const string NAMECAST_ID = " as ";
        private const string POSITIONCAST_ID = " at ";
        private const string EXPRESSIONCAST_ID = @" [";
        private const char AXIS_DELIM = ':';
        private const char EXPRESSIONLAYER_JOINER = ',';
        private const char EXPRESSIONLAYER_DELIM = ':';

        private const string ENTER_KEYWORD = "enter ";
        private const string ENTER_STAGE_LEFT = "left ";
        private const string ENTER_STAGE_RIGHT = "right ";

        private string ProcessKeywords(string rawSpeaker)
        {
            if (rawSpeaker.StartsWith(ENTER_KEYWORD))
            {
                rawSpeaker = rawSpeaker.Substring(ENTER_KEYWORD.Length);
                makeCharacterEnter = true;

                if (rawSpeaker.StartsWith(ENTER_STAGE_LEFT)) { 
                    rawSpeaker = rawSpeaker.Substring(ENTER_STAGE_LEFT.Length);
                    makeCharacterEnterLeft = true;
                    makeCharacterEnterRight = false;
                }
                else if (rawSpeaker.StartsWith(ENTER_STAGE_RIGHT)) {
                    rawSpeaker = rawSpeaker.Substring(ENTER_STAGE_RIGHT.Length);
                    makeCharacterEnterRight = true;
                    makeCharacterEnterLeft = false;
                }
            }

            return rawSpeaker;
        }

        public DL_SpeakerData(string rawSpeaker)
        {
            rawSpeaker = ProcessKeywords(rawSpeaker);

            string pattern = @$"{NAMECAST_ID}|{POSITIONCAST_ID}|{EXPRESSIONCAST_ID.Insert(EXPRESSIONCAST_ID.Length - 1, @"\")}";
            MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

            castName = "";
            castPosition = Vector2.zero;
            CastExpressions = new List<(int layer, string expression)>();

            if (matches.Count == 0)
            {
                name = rawSpeaker;
                return;
            }

            int index = matches[0].Index;
            name = rawSpeaker.Substring(0, index);

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                int startIndex = 0, endIndex = 0;

                if (match.Value == NAMECAST_ID)
                {
                    startIndex = match.Index + NAMECAST_ID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
                }
                else if (match.Value == POSITIONCAST_ID)
                {
                    isCastingPosition = true;
                    startIndex = match.Index + POSITIONCAST_ID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                    string[] axis = castPos.Split(AXIS_DELIM, System.StringSplitOptions.RemoveEmptyEntries);

                    float.TryParse(axis[0], out castPosition.x);

                    if (axis.Length > 1)
                        float.TryParse(axis[1], out castPosition.y);
                }
                else if (match.Value == EXPRESSIONCAST_ID)
                {
                    startIndex = match.Index + EXPRESSIONCAST_ID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                    CastExpressions = castExp.Split(EXPRESSIONLAYER_JOINER)
                        .Select(x =>
                        {
                            var parts = x.Trim().Split(EXPRESSIONLAYER_DELIM);

                            if (parts.Length == 2)
                                return (int.Parse(parts[0]), parts[1]);
                            else
                                return (0, parts[0]);

                        }).ToList();
                }
            }

        }
    }
}