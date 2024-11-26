using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class TagManager
{
    private readonly Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>();
    private readonly Regex tagRegex = new Regex("<\\w+>");

    public TagManager()
    {
        InitializeTags();
    }

    private void InitializeTags()
    {
        tags["<mainChar>"] = () => "You"; // to be replaced
        tags["<time>"] = () => DateTime.Now.ToString("hh:mm tt");
        tags["<playerLvl>"] = () => "15"; // to be replaced
        tags["<tempVal1>"] = () => "42"; // to be replaced
    }

    public string Inject(string text)
    {
        if (tagRegex.IsMatch(text))
        {
            foreach (Match match in tagRegex.Matches(text))
            {
                if (tags.TryGetValue(match.Value, out var tagValueReqeust))
                {
                    text = text.Replace(match.Value, tagValueReqeust());
                }
            }
        }

        return text;
    }
}
