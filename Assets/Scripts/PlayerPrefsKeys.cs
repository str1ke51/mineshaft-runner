public static class PlayerPrefsKeys
{
    public static string BestScore = "BestScore";
    public static string OverallPoints = "OverallPoints";

    public static string BestLevelScore(int sceneIndex) => "BestScore-Level_" + sceneIndex;
    
}
