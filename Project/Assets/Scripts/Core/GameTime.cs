
public class GameTime : Singleton<GameTime>
{
    public static float time;

    public static void Reset()
    {
        time = 0.0f;
    }

    public static void Update(float dt)
    {
        time += dt;
    }
}
