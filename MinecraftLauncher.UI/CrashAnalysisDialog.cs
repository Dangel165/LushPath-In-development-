using MinecraftLauncher.Core.Managers;

namespace MinecraftLauncher.UI;

public partial class CrashAnalysisDialog : Form
{
    private readonly CrashAnalysisResult _result;

    public CrashAnalysisDialog(CrashAnalysisResult result)
    {
        _result = result ?? throw new ArgumentNullException(nameof(result));
        
        InitializeComponent();
        InitializeDarkMode();
        LoadCrashData();
    }

    private void InitializeDarkMode()
    {
        this.BackColor = ColorTranslator.FromHtml("#1e1e1e");
        this.ForeColor = ColorTranslator.FromHtml("#ffffff");
        
        ApplyDarkModeToControls(this.Controls);
    }

    private void ApplyDarkModeToControls(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {
            if (control is Button || control is TextBox || control is Label)
            {
                control.BackColor = ColorTranslator.FromHtml("#2d2d30");
                control.ForeColor = ColorTranslator.FromHtml("#ffffff");
            }
            else if (control is Panel panel)
            {
                panel.BackColor = ColorTranslator.FromHtml("#252526");
                panel.ForeColor = ColorTranslator.FromHtml("#ffffff");
            }
            
            if (control.HasChildren)
            {
                ApplyDarkModeToControls(control.Controls);
            }
        }
    }

    private void LoadCrashData()
    {
        // Set crash information
        crashTimeLabel.Text = $"Crash Time: {_result.CrashTime:yyyy-MM-dd HH:mm:ss}";
        crashCauseLabel.Text = $"Cause: {_result.CrashCause}";
        minecraftVersionLabel.Text = $"Minecraft: {_result.MinecraftVersion}";
        javaVersionLabel.Text = $"Java: {_result.JavaVersion}";
        
        // Load mods involved
        if (_result.ModsInvolved.Any())
        {
            modsTextBox.Text = string.Join(Environment.NewLine, _result.ModsInvolved);
        }
        else
        {
            modsTextBox.Text = "No mods detected";
        }
        
        // Load possible causes
        if (_result.PossibleCauses.Any())
        {
            causesTextBox.Text = string.Join(Environment.NewLine, _result.PossibleCauses.Select((c, i) => $"{i + 1}. {c}"));
        }
        else
        {
            causesTextBox.Text = "Unknown";
        }
        
        // Load solutions
        if (_result.SuggestedSolutions.Any())
        {
            solutionsTextBox.Text = string.Join(Environment.NewLine + Environment.NewLine, 
                _result.SuggestedSolutions.Select((s, i) => $"{i + 1}. {s}"));
        }
        else
        {
            solutionsTextBox.Text = "No solutions available";
        }
        
        // Load stack trace
        stackTraceTextBox.Text = _result.StackTrace;
    }

    private void closeButton_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void copyButton_Click(object sender, EventArgs e)
    {
        try
        {
            var report = $@"Crash Analysis Report
===================
Crash Time: {_result.CrashTime:yyyy-MM-dd HH:mm:ss}
Minecraft Version: {_result.MinecraftVersion}
Java Version: {_result.JavaVersion}

Crash Cause:
{_result.CrashCause}

Mods Involved:
{string.Join(Environment.NewLine, _result.ModsInvolved)}

Possible Causes:
{string.Join(Environment.NewLine, _result.PossibleCauses.Select((c, i) => $"{i + 1}. {c}"))}

Suggested Solutions:
{string.Join(Environment.NewLine, _result.SuggestedSolutions.Select((s, i) => $"{i + 1}. {s}"))}

Stack Trace:
{_result.StackTrace}
";
            
            Clipboard.SetText(report);
            MessageBox.Show("Crash report copied to clipboard!", "Success", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
