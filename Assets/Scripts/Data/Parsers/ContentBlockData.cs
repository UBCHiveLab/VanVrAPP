using UnityEngine;

public enum BlockType
{
    SEPARATOR,
    TEXT,
    AUDIO,
    IMAGE,
    VIDEO
}

public class ContentBlockData
{
    public BlockType type;
    public string title;
    public string content;
    public Vector2Int widthHeight;

    public ContentBlockData(BlockType type, string title, string content, Vector2Int widthHeight)
    {
        this.type = type;
        this.title = title;
        this.content = content;
        this.widthHeight = widthHeight;
    }
}
