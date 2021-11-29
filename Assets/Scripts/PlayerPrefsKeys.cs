public static class PlayerPrefsKeys
{
    public static string BestScore = "BestScore";
    public static string OverallPoints = "OverallPoints";

    public static string BestLevelScore(string level) => "BestScore-Level_" + level;
    
}
