using Serilog;
using Serilog.Formatting.Compact;

namespace FormattingOutputSample;

class Program
{
    static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(new CompactJsonFormatter(), "logs/log.txt")
            .CreateLogger();
        var position = new { Latitude = 25, Longitude = 134 };
        var elapsedMs = 34;

        Log.Information("Processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);

        var fruit = new[] { "Apple", "Pear", "Orange" };
        Log.Information("In my bowl I have {Fruit}", fruit);
        
        
        Log.CloseAndFlush();
    }
}