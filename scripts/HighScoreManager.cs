using System;
using Godot;

public static class HighScoreManager
{
    private static readonly string scoreFilePath = "user://highScore.dat";

    public static int LoadHighScore()
    {
        // Load the high score from a file. If the file is not found or is corrupted then returns 0, the default score.

        using (var file = new File())
        {
            try
            {
                file.Open(scoreFilePath, File.ModeFlags.Read);
                return (int) file.Get32();
            }
            catch
            {
                return 0;
            }
        }
    }

    public static void SaveHighScore(int highScore)
    {
        // Load the high score from a file. If the file is not found or is corrupted then returns 0, the default score.

        using (var file = new File())
        {
            try
            {
                file.Open(scoreFilePath, File.ModeFlags.Write);
                file.Store32((uint) highScore);
            }
            catch
            {
                GD.PushWarning($"Failed to write high score into file {scoreFilePath}");
            }
        }
    }
}