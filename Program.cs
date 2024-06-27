using System.CommandLine;
using System.Diagnostics;

namespace CronThumborCleaner;
public class Program
{
    static void Main(string[] args)
    {
        var pathArgs = new Option<string>(
            name: "--path",
            description: "Path of the folder",
            getDefaultValue: () => { return "/tmp/"; });

        var filenameArgs = new Option<string>(
            name: "--filename",
            description: "Filename targets in glob",
            getDefaultValue: () => { return "tmp*"; });

        var dayArgs = new Option<int>(
            name: "--day",
            description: "Maximum day of file created",
            getDefaultValue: () => { return 1; });

        // Notes for me. This using IEnumerable for registering args
        var cmd = new RootCommand("App for cleaning thumbor cache")
        {
            dayArgs, pathArgs, filenameArgs
        };
        cmd.SetHandler(
            Runner,
            dayArgs, pathArgs, filenameArgs);

        cmd.Invoke(args);
    }

    public static void Runner(int day, string path, string filename)
    {
        Logging.Info($"Starting the clock with {day} day delay.");
        while (true)
        {
            // Set timer
            var now = DateTime.Now;
            var nextDayDiff = now.AddDays(day) - now;
            var timer = new System.Timers.Timer(nextDayDiff);
            var enabled = true;
            timer.Elapsed += (sender, e) => { enabled = false; };

            // Exec command
            var command = $"find {path} -maxdepth 1 -name \"{filename}\" -mtime +{day} -type f -delete";
            Logging.Info($"Executing command: {command}");
            ExecShell(command);

            // Reset and dispose timer
            timer.Start();
            Logging.Info($"Waiting for next {day} day.");
            while (enabled) { }
            timer.Stop();
            timer.Dispose();
        }
    }

    public static void ExecShell(string command)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "bash",
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

