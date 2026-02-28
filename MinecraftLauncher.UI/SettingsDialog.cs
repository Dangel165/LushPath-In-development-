using MinecraftLauncher.Core.Logging;
using System.Diagnostics;

namespace MinecraftLauncher.UI;

/// <summary>
/// Settings dialog for launcher configuration
/// </summary>
public partial class SettingsDialog : Form
{
    private readonly ErrorLogger _errorLogger;

    public SettingsDialog(ErrorLogger errorLogger)
    {
        _errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
        InitializeComponent();
        InitializeDarkMode();
        LoadSettings();
    }

    private void InitializeDarkMode()
    {
        // Apply dark mode colors
        this.BackColor = ColorTranslator.FromHtml("#1e1e1e");
        this.ForeColor = ColorTranslator.FromHtml("#ffffff");
        
        // Apply to all child controls
        ApplyDarkModeToControls(this.Controls);
    }

    private void ApplyDarkModeToControls(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {
            if (control is Button || control is ComboBox || control is TextBox || control is Panel || control is GroupBox)
            {
                control.BackColor = ColorTranslator.FromHtml("#2d2d30");
                control.ForeColor = ColorTranslator.FromHtml("#ffffff");
            }
            
            if (control.HasChildren)
            {
                ApplyDarkModeToControls(control.Controls);
            }
        }
    }

    private void LoadSettings()
    {
        // Load current settings
        versionLabel.Text = $"Version: {GetApplicationVersion()}";
    }

    private string GetApplicationVersion()
    {
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        return version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "1.0.0";
    }

    private void openLogsButton_Click(object sender, EventArgs e)
    {
        try
        {
            var logsDirectory = _errorLogger.GetLogsDirectory();
            
            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }
            
            Process.Start(new ProcessStartInfo
            {
                FileName = logsDirectory,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error opening logs directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void clearCacheButton_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to clear the cache?\n\nThis will remove cached skins, announcements, and statistics.",
            "Confirm Clear Cache",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (result == DialogResult.Yes)
        {
            try
            {
                var cacheDirectory = MinecraftLauncher.Core.LauncherPaths.CacheDirectory;
                
                if (Directory.Exists(cacheDirectory))
                {
                    Directory.Delete(cacheDirectory, true);
                    Directory.CreateDirectory(cacheDirectory);
                }
                
                MessageBox.Show("Cache cleared successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error clearing cache: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
