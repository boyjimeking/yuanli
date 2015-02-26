
using UnityEngine;

public class GameDataAlgorithm
{
    /// <summary>
    /// 获取战争奖励系数
    /// </summary>
    /// <param name="deltaLevel">进攻方和防守方的等级差</param>
    /// <returns></returns>
    public static float GetBattleRewardRatio(int attackerLevel,int defenderLevel)
    {
        var model = DataCenter.Instance.FindBattleMedalRewardModel(attackerLevel, defenderLevel);
        return model.resourceRatio;
    }

    /// <summary>
    /// 战斗奖牌奖励
    /// </summary>
    /// <param name="attackerLevel"></param>
    /// <param name="defenderLevel"></param>
    /// <param name="star"></param>
    /// <returns></returns>
    public static int GetBattleRewardMetal(int attackerLevel,int defenderLevel, int star)
    {
        var model = DataCenter.Instance.FindBattleMedalRewardModel(attackerLevel, defenderLevel);
        switch (star)
        {
        case 1:
            return model.medalStar1;
        case 2:
            return model.medalStar2;
        case 3:
            return model.medalStar3;
        }
        return 0;
    }
    /// <summary>
    /// 战斗荣誉奖励
    /// </summary>
    /// <param name="attackerScore"></param>
    /// <param name="defenderScore"></param>
    /// <param name="star"></param>
    /// <returns></returns>
    public static int GetBattleRewardScore(int attackerScore, int defenderScore, int star)
    {
        var deltaScore = Mathf.Clamp(defenderScore - attackerScore,-110,110);
        var score = deltaScore * 0.1f + 35;
        switch (star)
        {
        case 1:
            score *= 0.3f;
            break;
        case 2:
            score *= 0.6f;
            break;
        }
        return Mathf.RoundToInt(score);
    }
    /// <summary>
    /// 资源转换成钻石
    /// </summary>
    /// <param name="resources"></param>
    /// <returns></returns>
    public static int ResourceToGem(int resources)
    {
        var ranges = new []{100, 1000, 10000, 100000, 1000000, 10000000};
        var gems = new []{1, 5, 25, 125, 600, 3000};
        if (resources <= 0) return (0);
        if (resources <= ranges[0])
            return (gems[0]);
        int i;
        for (i = 1; i < ranges.Length - 1; i++)
            if (resources <= ranges[i]) return (Mathf.RoundToInt((resources - ranges[i - 1]) / ((ranges[i] - ranges[i - 1]) / (gems[i] - gems[i - 1])) + gems[i - 1]));
        i = ranges.Length - 1;
        return (Mathf.RoundToInt((resources - ranges[i - 1]) / ((ranges[i] - ranges[i - 1]) / (gems[i] - gems[i - 1])) + gems[i - 1]));
    }
    /// <summary>
    /// 时间转换成钻石
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static int TimeToGem(int seconds)
    {
        var ranges = new[]  { 60, 3600, 86400, 604800};
        var gems = new[]    { 1,  20,   260,   1000};
        if (seconds <= 0) return (0);
        if (seconds <= ranges[0])
            return (gems[0]);
        int i;
        for (i = 1; i < ranges.Length - 1; i++)
            if (seconds <= ranges[i]) return (Mathf.RoundToInt((seconds - ranges[i - 1]) / ((ranges[i] - ranges[i - 1]) / (gems[i] - gems[i - 1])) + gems[i - 1]));
        i = ranges.Length - 1;
        return (Mathf.RoundToInt((seconds - ranges[i - 1]) / ((ranges[i] - ranges[i - 1]) / (gems[i] - gems[i - 1])) + gems[i - 1]));
    }
}
