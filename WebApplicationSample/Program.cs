using Serilog;
using Serilog.Events;

namespace WebApplicationSample;

public class Program
{
    public static void Main(string[] args)
    {
       
        // Log.Logger = new LoggerConfiguration()
        //     .WriteTo.Console()
        //     // .CreateLogger();
        //     .CreateBootstrapLogger();
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                // .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                // .Enrich.FromLogContext()
                // .WriteTo.Console()
                .CreateLogger();
            
            Log.Information("Starting web application");

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add Serilog to the container
            builder.Services.AddSerilog();
            // builder.Services.AddSerilog((services, lc) => lc
            //     .ReadFrom.Configuration(builder.Configuration)
            //     .ReadFrom.Services(services)
            //     );
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthorization();
            
            // Use Serilog to log HTTP requests
            app.UseSerilogRequestLogging();
            // app.UseSerilogRequestLogging(options =>
            // {
            //     // Customize the message template
            //     options.MessageTemplate = "Handled {RequestMethod} {StatusCode} Path: {RequestPath} Elapsed:{Elapsed}";
            //
            //     // // Emit debug-level events instead of the defaults
            //     // options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
            //     //
            //     // Attach additional properties to the request completion event
            //     options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            //     {
            //         diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            //         diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            //     };
            // });
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}