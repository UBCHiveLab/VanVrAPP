using UnityEngine;

/**
 * Holds parsed data from the content of an annotation. Used to generate rich media annotation content.
 */
public enum BlockType
{
    SEPARATOR,
    TEXT,
    AUDIO,
    IMAGE,
    VIDEO,
    LINK
}

public class ContentBlockData
{
    public BlockType type;
    public string title;
    public string content;
    public Vector2Int widthHeight;
    public string cite;

    public ContentBlockData(BlockType type, string title, string content, string cite, Vector2Int widthHeight)
    {
        this.type = type;
        this.title = title;
        this.content = content;
        this.widthHeight = widthHeight;
        this.cite = cite;
    }
}
