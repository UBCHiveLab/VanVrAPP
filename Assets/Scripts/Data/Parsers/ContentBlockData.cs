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

    public ContentBlockData(BlockType type, string title, string content)
    {
        this.type = type;
        this.title = title;
        this.content = content;
    }
}
