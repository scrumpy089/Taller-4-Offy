public static class GameData
{
    public static int score = 0;
    public static int correctAnswers = 0;
    public static int wrongAnswers = 0;
    public static int totalQuestions = 0;
    public static int maxQuestions = 4;

    public static void Reset()
    {
        score = 0;
        correctAnswers = 0;
        wrongAnswers = 0;
        totalQuestions = 0;
    }
}