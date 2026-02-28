using MinecraftLauncher.Core.Logging;
using System.Diagnostics;

namespace MinecraftLauncher.UI;

/// <summary>
/// Error dialog for displaying user-friendly error messages
/// </summary>
public partial class ErrorDialog : Form
{
    private readonly ErrorLogger _errorLogger;
    private readonly string _errorMessage;
    private readonly string _technicalDetails;
    private bool _detailsExpanded = false;

    public ErrorDialog(string errorMessage, string technicalDetails, ErrorLogger errorLogger)
    {
        _errorMessage = errorMessage ?? "An unexpected error occurred.";
        _technicalDetails = technicalDetails ?? "No additional details available.";
        _errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
        
        InitializeComponent();
        InitializeDarkMode();
        LoadErrorDetails();
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
            if (control is Button || control is TextBox || control is Panel || control is GroupBox)
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

    private void LoadErrorDetails()
    {
        errorMessageLabel.Text = _errorMessage;
        technicalDetailsTextBox.Text = _technicalDetails;
        
        // Initially hide technical details
        technicalDetailsTextBox.Visible = false;
        technicalDetailsLabel.Visible = false;
        
        // Adjust form height
        this.Height = 220;
    }

    private void toggleDetailsButton_Click(object sender, EventArgs e)
    {
        _detailsExpanded = !_detailsExpanded;
        
        if (_detailsExpanded)
        {
            // Show technical details
            technicalDetailsTextBox.Visible = true;
            technicalDetailsLabel.Visible = true;
            toggleDetailsButton.Text = "Hide Details";
            this.Height = 500;
        }
        else
        {
            // Hide technical details
            technicalDetailsTextBox.Visible = false;
            technicalDetailsLabel.Visible = false;
            toggleDetailsButton.Text = "Show Details";
            this.Height = 220;
        }
    }

    private void copyToClipboardButton_Click(object sender, EventArgs e)
    {
        try
        {
            var clipboardText = $"Error Message:\n{_errorMessage}\n\nTechnical Details:\n{_technicalDetails}";
            Clipboard.SetText(clipboardText);
            MessageBox.Show("Error details copied to clipboard.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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

    private void closeButton_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
