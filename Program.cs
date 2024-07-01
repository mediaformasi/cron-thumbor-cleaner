using System.Diagnostics;

namespace CronThumborCleaner;
public class Program
{
    public static int DayCount = 1;
    public static string PathTempFolder = "/tmp";
    public static string GlobFilename = "tmp*";

    static void Main(string[] args)
    {
        Logging.Info($"Starting the clock with {DayCount} day delay.");
        while (true)
        {
            // Set timer
            var now = DateTime.Now;
            var nextDayDiff = now.AddDays(DayCount) - now;

            // Exec command
            var command = $"find {PathTempFolder} -maxdepth 1 -name \"{GlobFilename}\" -mtime +{DayCount} -type f -delete";
            Logging.Info($"Executing command: {command}");
            ExecShell(command);

            // Reset and dispose timer
            Logging.Info($"Waiting for next {DayCount} day.");
            Thread.Sleep(DayCount * 24 * 60 * 60 * 1000);
        }
    }

    public static void ExecShell(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "sh",
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            }
        };

        process.Start();
        process.StandardInput.WriteLine(command);
    }

}

public class Logging
{
    public static string NumberPadding(int number, int paddingCount = 2)
        => number.ToString($"D{paddingCount}");

    public static void Info(string log)
    {
        var now = DateTime.Now;
        Console.WriteLine(
            $"[{now.Year}-{NumberPadding(now.Month)}-{NumberPadding(now.Day)} " +
            $"{NumberPadding(now.Hour)}:{NumberPadding(now.Minute)}:{NumberPadding(now.Second)}] {log}"
        );
    }
}

