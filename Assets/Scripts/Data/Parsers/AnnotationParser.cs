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
                if (match.Groups.Count > 0) {
                    title = match.Groups[1].Value;
                }
                blocks.Add(new ContentBlockData(BlockType.VIDEO, title, src));
            } else if (content.StartsWith("img") || content.StartsWith("image")) {
                Match match = Regex.Match(content, "src=[\'|\"]([^\'\"]*)[\'|\"]");
                string src = match.Groups[1].Value;
                match = Regex.Match(content, "title=[\'|\"](.*?)[\'|\"]");
                string title = src;
                if (match.Groups.Count > 0) {
                    title = match.Groups[1].Value;
                }   
                blocks.Add(new ContentBlockData(BlockType.IMAGE, title, src));

            } else if (content.StartsWith("aud") || content.StartsWith("audio")) {
                Match match = Regex.Match(content, "src=[\'|\"]([^\'\"]*)[\'|\"]");
                string src = match.Groups[1].Value;
                match = Regex.Match(content, "title=[\'|\"](.*?)[\'|\"]");
                string title = src;
                if (match.Groups.Count > 0) {
                    title = match.Groups[1].Value;
                }
                blocks.Add(new ContentBlockData(BlockType.AUDIO, title, src));

            } else if (content.Trim() != "") {
                blocks.Add(new ContentBlockData(BlockType.TEXT, null, content));
            } else {
                blank = true;
            }

            if (!blank && i > 0 && i < contents.Count - 1) {
                blocks.Add(new ContentBlockData(BlockType.SEPARATOR, null, null));
            }
        }

        return blocks;
    }

}
