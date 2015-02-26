
public class BattleRandom
{
    private static int seed = 0xdead;

    private static int Rand() {
        seed = (214013 * seed + 2531011);
        return (seed >> 16) & 0x7FFF; 
    }

    public static void SetSeed(int s)
    {
        seed = s;
    }

    /// <summary>
    /// 获取min到max之间的随机数（不包括max） ※ 开区间 min...max
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Range(int min, int max)
    {
        var r = Rand();
        return min + (r % (max-min));
    }

    public static float Range(float min, float max)
    {
        return min + (max - min) * Rand01();
    }

    public static float Rand01()
    {
        return (float) Rand() / 0x8000;
    }
}
