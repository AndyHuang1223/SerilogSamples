using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;

namespace ConsoleSample;

class Program
{
    static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        
        var position = new { Latitude = 25, Longitude = 134 };
        var elapsedMs = 34;

        Log.Information("Processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);
        
        var fruit = new[] { "Apple", "Pear", "Orange" };
        Log.Information("In my bowl I have {Fruit}", fruit);
        
    }
}