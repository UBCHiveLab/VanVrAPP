using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class AnnotationParser
{
    /**
     * Helper method that splits the given content string to find rich media and split text blocks.
     */
    private List<string> TokenizeContent(string content) {
        return Regex.Split(content, "\\[|\\]").ToList();
    }

    /**
     * Parses through tokenized content and creates blocks from prefabs.
     */
    public List<ContentBlockData> ParseAndAddContentBlocks(string parseableString) {

        List<ContentBlockData> blocks = new List<ContentBlockData>();

        List<string> contents = TokenizeContent(parseableString);


        for (int i = 0; i < contents.Count; i++) {
            bool blank = false;
            string content = contents[i];
            if (content == "[" || content == "]") {
                // ignore
            } else if (content.StartsWith("vid") || content.StartsWith("video"))
            {
                string src = MatchStringAttribute(content, "src"); //source matches anything between two quotations marks
                string title = MatchStringAttributeDefault(content, "title", src);
                string cite = MatchStringAttributeDefault(content, "cite", "");
                int width = MatchIntAttribute(content, "width", -1);
                int height = MatchIntAttribute(content, "height", -1);
                blocks.Add(new ContentBlockData(BlockType.VIDEO, title, src, cite, new Vector2Int(width, height)));

            } else if (content.StartsWith("img") || content.StartsWith("image")) {
                string src = MatchStringAttribute(content, "src"); //source matches anything between two quotations marks
                string title = MatchStringAttributeDefault(content, "title", src);
                string cite = MatchStringAttributeDefault(content, "cite", "");
                int width = MatchIntAttribute(content, "width", -1);
                int height = MatchIntAttribute(content, "height", -1);
                blocks.Add(new ContentBlockData(BlockType.IMAGE, title, src, cite, new Vector2Int(width, height)));
            } else if (content.StartsWith("aud") || content.StartsWith("audio")) {
                string src = MatchStringAttribute(content, "src"); //source matches anything between two quotations marks
                string title = MatchStringAttributeDefault(content, "title", src);
                string cite = MatchStringAttributeDefault(content, "cite", "");
                blocks.Add(new ContentBlockData(BlockType.AUDIO, title, src, cite, Vector2Int.zero));

            } else if (content.Trim() != "") {
                string title = MatchStringAttributeDefault(content, "title", "");
                string cite = MatchStringAttributeDefault(content, "cite", "");
                blocks.Add(new ContentBlockData(BlockType.TEXT, title, content, cite, Vector2Int.zero));
            } else {
                blank = true;
            }

            if (!blank && i > 0 && i < contents.Count - 1) {
                blocks.Add(new ContentBlockData(BlockType.SEPARATOR, null, null, "", Vector2Int.zero));
            }
        }

        return blocks;
    }


    string MatchStringAttributeDefault(string content, string att, string defaultValue, string pattern = "(.*?)")
    {
        Match match = Regex.Match(content, $"{att}=[\'|\"]{pattern}[\'|\"]");
        string val = defaultValue;
        if (match.Groups.Count > 0 && match.Groups[1].Value != "") {
             val = match.Groups[1].Value;
        }

        return val;
    }

    string MatchStringAttribute(string content, string att, string pattern="(.*?)") {
        Match match = Regex.Match(content, $"{att}=[\'|\"]{pattern}[\'|\"]");
        string val = "";
        if (match.Groups.Count > 0 && match.Groups[1].Value != "") {
            val = match.Groups[1].Value;
        }
        else
        {
            throw new Exception($"Missing required attribute {att}");
        }

        return val;
    }

    int MatchIntAttribute(string content, string att, int defaultValue, string pattern = "(\\d*)") {
        Match match = Regex.Match(content, $"{att}=[\'|\"]{pattern}[\'|\"]");
        int val = defaultValue;
        if (match.Groups.Count > 0 && match.Groups[1].Value != "") {
            string sval = match.Groups[1].Value;
            if (!int.TryParse(sval, out val))
            {
                throw new Exception($"Couldn't parse attribute {att} to integer from value {sval}");
            }
        }

        return val;
    }

}
