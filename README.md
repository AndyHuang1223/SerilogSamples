# Serilog-日誌套件入門

官網介紹：https://serilog.net/

## Example application 
1. 新增 Conosle App 專案(.Net 8)
2. 安裝 Nuget 套件
    ```
    $ dotnet add package Serilog
    $ dotnet add package Serilog.Sinks.Console
    $ dotnet add package Serilog.Sinks.File
    ```
3. 修改`Program.cs`
    ```csharp=
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt",
                    rollingInterval: RollingInterval.Day)
                .MinimumLevel.Debug() // Set the minimum log level
                .CreateLogger();

            Log.Information("Hello, Serilog!");

            try
            {
                int a = 10, b = 0;
                Log.Debug("Dividing {A} by {B}", a, b);
                Console.WriteLine(a / b);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something wrong!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
    ```
4. 查看結果
![image](https://hackmd.io/_uploads/SyOHRK2lkl.png)

## Configuration Basics 設定入門
### Creating a logger
新增`Loggerconfiguration`物件:
```csharp=
Log.Logger = new LoggerConfiguration().CreateLogger();
Log.Information("No one listens to me!");

// Finally, once just before the application exits...
Log.CloseAndFlush();
```
設定了Logger，但沒有設定 `Sinks` 就無法顯示Log內容。
### Sinks
可以使用的Sinks列表:[Provided Sinks](https://github.com/serilog/serilog/wiki/Provided-Sinks)
確定有安裝到以下的 Nuget 套件
```
    $ dotnet add package Serilog.Sinks.Console
    $ dotnet add package Serilog.Sinks.File
```
修改 `LoggerConfiguration`物件，在`CreateLogger()`前加入`WriteTo.Console() `, `.WriteTo.File("logs/log.txt")`。
```csharp=
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()    // Write log to Conosle
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // Write log to file
    .CreateLogger();
Log.Information("Hello, Serilog!");
```

### Output templates
Text-based的 Sinks 可以透過 *output templates* 處理顯示的模板。
```csharp=
.WriteTo.File("/logs/log.txt",
      rollingInterval: RollingInterval.Day,
      outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
```
也可以試試看修改 Console 的 outputTempalte。

### Minimum level
Logger可以設定最小紀錄層級，就不會什麼東西都被存進去了。
預設為`Information`
```csharp=
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()    // Write log to Conosle
    .WriteTo.File("logs/log.txt") // Write log to file
    .CreateLogger();

Log.Information("Hello, Serilog!");

// 新增以下程式碼
Log.Debug("This is Debug message!");
```
這時你應該看不到Debug的訊息。

修改 `LoggerConfiguration`
```csharp=
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()    // Write log to Conosle
    .WriteTo.File("logs/log.txt") // Write log to file
    // 新增下面這行
    .MinimumLevel.Debug()    // Set the minimum log level
    .CreateLogger();

Log.Information("Hello, Serilog!");


Log.Debug("This is Debug message!");
```

此時可以看到輸出的Debug層級訊息：This is Debug message!

### Enrichers
`Enrichers` 元件，可以簡單自訂要額外記錄的 properties，可以附加到 log event上。
sample01:
新增`ThreadIdEnricher` class
```csharp=
class ThreadIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ThreadId", Thread.CurrentThread.ManagedThreadId));
    }
}
```
修改`LoggerConfiguration`
```csharp=
Log.Logger = new LoggerConfiguration()
    .Enrich.With(new ThreadIdEnricher())
    .WriteTo.Console(
        outputTemplate: "{Timestamp:HH:mm} [{Level}] ({ThreadId}) {Message}{NewLine}{Exception}")
    .CreateLogger();
```
outpout template上的`{ThreadId}`就可以看到結果。

sample02:
如果要加入的值是常數時，也可以直接設定。`LoggerConfiguration`。
設定後可以在 outpout template 上看到以及可以被篩選。
```csharp=
Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("Version", "1.0.0")
    .WriteTo.Console(
    outputTemplate: "{Timestamp:HH:mm} [{Level}] (Version:{Version}) {Message}{NewLine}{Exception}")
    .CreateLogger();
```


### Filters
透過`Filter`可以篩選要被記錄Log，設定的部分透過`Matching` class，撰寫`LogEvent` predicates。
有`ByExcluding()`, `ByIncludingOnly()`可以使用。
```csharp=
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Filter.ByExcluding(Matching.WithProperty<int>("Count", p => p < 10))
    .CreateLogger();


Log.Information("Hello, Serilog!");        // 這個會紀錄
Log.Information("Count: {Count}", 5);      // 這個不會紀錄
Log.Information("Count: {Count}", 10);     //這個會記錄
```

### Sub-loggers
可以根據不同的情境，以Pipeline形式撰寫更精細的Log相關設定，包含像是`Filter`以及要存入的Sinks。
```csharp=
var subLogger = new LoggerConfiguration()
            .Filter.ByIncludingOnly(Matching.WithProperty<int>("Count", p => p < 10))
            .WriteTo.File("logs/sublog.txt")
            .CreateLogger();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Logger(subLogger)
    .WriteTo.Console()
    .CreateLogger();
    
// 以下的Log內容都會在Console內出現
Log.Information("Hello, Serilog!"); 
Log.Information("Count: {Count}", 5);  // 這個Log會另外存到 /logs/sublog.txt
Log.Information("Count: {Count}", 10);

```

以上為 Serlog 基本的使用方式。

---

## Formatting Output
從 `outputTemplate`可以設定許多內建的參數[formatting-plain-text](https://github.com/serilog/serilog/wiki/Formatting-Output#formatting-plain-text)作為紀錄模板。

```csharp=
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
```
大多數的 sinks 都有提供 JSON 格式可以使用，所以可以利用`Serilog.Formatting.Compact`，將純文字轉為JSON。

### 使用 `Serilog.Formatting.Compact`
安裝`Serilog.Formatting.Compact`套件
```
dotnet add package Serilog.Formatting.Compact
```

### 為 file sink 提供 `Serilog.Formatting.Compact`

```csharp=
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
        
        Log.CloseAndFlush();
    }
}
```
結果：
```json=
{
  "@t": "2024-10-28T08:37:11.4427990Z",
  "@mt": "Processed {@Position} in {Elapsed:000} ms.",
  "@r": ["034"],
  "Position": { "Latitude": 25, "Longitude": 134 },
  "Elapsed": 34
}
```
模板相關說明：
https://github.com/serilog/serilog-formatting-compact

---

## 其他參考範例
- [Serilog.AspNetCore 範例](https://hackmd.io/@andyhuang1223/HJfhC63gke)
- [Elastic 整合 Microsoft.Extension.Logging](https://hackmd.io/@andyhuang1223/S12gaWTxJg)
