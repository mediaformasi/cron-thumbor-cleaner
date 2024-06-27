using System.CommandLine;

namespace CronThumborCleaner;
public class Program
{
    static void Main(string[] args)
    {
        var cmd = new RootCommand("App for cleaning thumbor cache");

        var delayArgs = new Option<int>(
            name: "--delay",
            description: "Delay on seconds",
            getDefaultValue: () => { return 86400; });
        cmd.AddOption(delayArgs);
        cmd.SetHandler((delay) => Runner(ref delay), delayArgs);

        cmd.Invoke(args);
    }

    public static void Runner(ref int delay)
    {
        Logging.Info($"Starting the clock with {delay} seconds delay.");
        while (true)
        {
            Logging.Info($"Waiting for next {delay} seconds.");
            Thread.Sleep(delay * 1000);
        }
    }

}

public class Logging
{
    public static void Info(string log)
    {
        var now = DateTime.Now;
        Console.WriteLine(
            $"[{now.Year}-{now.Month}-{now.Day} {now.Hour}:{now.Minute}:{now.Second}] {log}"
        );
    }
}

