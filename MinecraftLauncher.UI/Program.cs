using MinecraftLauncher.Core;
using MinecraftLauncher.Core.Interfaces;
using MinecraftLauncher.Core.Logging;
using MinecraftLauncher.Core.Managers;
using MinecraftLauncher.Core.Services;
using MinecraftLauncher.Core.Validators;
using Serilog;

namespace MinecraftLauncher.UI;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        
        // Initialize launcher
        LauncherInitializer.Initialize();
        
        // Create logger
        var logger = Log.Logger;
        
        // Create managers
        var configManager = new ConfigurationManager();
        var profileManager = new ProfileManager(configManager);
        var httpClient = new HttpClientService();
        var fileDownloader = new FileDownloadManager(httpClient);
        var modManager = new ModManager(httpClient, fileDownloader);
        var versionManager = new VersionManager(httpClient, fileDownloader);
        var modLoaderManager = new ModLoaderManager(httpClient, fileDownloader);
        var minecraftLauncher = new Core.Managers.MinecraftLauncher(
            versionManager, 
            modLoaderManager, 
            modManager, 
            httpClient);
        var announcementManager = new AnnouncementManager(httpClient, logger);
        var skinRenderer = new SkinRenderer(httpClient);
        var statisticsManager = new StatisticsManager(httpClient);
        var friendManager = new FriendManager(httpClient);
        var ipValidator = new IPValidator(logger);
        var errorLogger = new ErrorLogger(logger);
        var crashAnalyzer = new CrashAnalyzer(logger);
        
        // Create and run main form
        var mainForm = new MainForm(
            profileManager,
            minecraftLauncher,
            announcementManager,
            skinRenderer,
            statisticsManager,
            friendManager,
            ipValidator,
            configManager,
            errorLogger,
            crashAnalyzer);
            
        Application.Run(mainForm);
    }    
}