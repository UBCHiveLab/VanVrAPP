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
            } else if (content.StartsWith("vid") || content.StartsWith("video")) {
                Match match = Regex.Match(content, "src=[\'|\"]([^\'\"]*)[\'|\"]");
                string src = match.Groups[1].Value;

                match = Regex.Match(content, "title=[\'|\"](.*?)[\'|\"]");
                string title = src;
                if (match.Groups.Count > 0 && match.Groups[1].Value != "") {
                    title = match.Groups[1].Value;
                }

                match = Regex.Match(content, "width=[\'|\"](\\d*)[\'|\"]");
                int width = -1;
                if (match.Groups.Count > 0 && match.Groups[1].Value != "")
                {
                    int.TryParse(match.Groups[1].Value, out width);
                }

                match = Regex.Match(content, "height=[\'|\"](\\d*)[\'|\"]");
                int height = -1;
                if (match.Groups.Count > 0 && match.Groups[1].Value != "") {
                    int.TryParse(match.Groups[1].Value, out height);
                }

                blocks.Add(new ContentBlockData(BlockType.VIDEO, title, src, new Vector2Int(width, height)));
            } else if (content.StartsWith("img") || content.StartsWith("image")) {
                Match match = Regex.Match(content, "src=[\'|\"]([^\'\"]*)[\'|\"]");
                string src = match.Groups[1].Value;
                match = Regex.Match(content, "title=[\'|\"](.*?)[\'|\"]");
                string title = src;
                if (match.Groups.Count > 0) {
                    title = match.Groups[1].Value;
                }

                blocks.Add(new ContentBlockData(BlockType.IMAGE, title, src, Vector2Int.zero));

            } else if (content.StartsWith("aud") || content.StartsWith("audio")) {
                Match match = Regex.Match(content, "src=[\'|\"]([^\'\"]*)[\'|\"]");
                string src = match.Groups[1].Value;
                match = Regex.Match(content, "title=[\'|\"](.*?)[\'|\"]");
                string title = src;
                if (match.Groups.Count > 0) {
                    title = match.Groups[1].Value;
                }
                blocks.Add(new ContentBlockData(BlockType.AUDIO, title, src, Vector2Int.zero));

            } else if (content.Trim() != "") {
                blocks.Add(new ContentBlockData(BlockType.TEXT, null, content, Vector2Int.zero));
            } else {
                blank = true;
            }

            if (!blank && i > 0 && i < contents.Count - 1) {
                blocks.Add(new ContentBlockData(BlockType.SEPARATOR, null, null, Vector2Int.zero));
            }
        }

        return blocks;
    }

}
