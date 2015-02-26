
public class IsoWall
{
    public enum WallDirection
    {
        Center,//单独的木桩
        Right,//朝右上
        Top,//朝左上
        TopAndRight,//右上和左上
    }

    public int x;
    public int y;
    public WallDirection direction;
}
