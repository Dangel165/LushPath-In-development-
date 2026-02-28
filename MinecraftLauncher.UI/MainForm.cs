using MinecraftLauncher.Core.Interfaces;
using MinecraftLauncher.Core.Logging;
using MinecraftLauncher.Core.Managers;
using MinecraftLauncher.Core.Models;
using MinecraftLauncher.Core.Validators;
using System.IO;

namespace MinecraftLauncher.UI;

public partial class MainForm : Form
{
    private readonly IProfileManager _profileManager;
    private readonly IMinecraftLauncher _minecraftLauncher;
    private readonly IAnnouncementManager _announcementManager;
    private readonly ISkinRenderer _skinRenderer;
    private readonly IStatisticsManager _statisticsManager;
    private readonly IFriendManager _friendManager;
    private readonly IPValidator _ipValidator;
    private readonly ConfigurationManager _configManager;
    private readonly ErrorLogger _errorLogger;
    private readonly CrashAnalyzer _crashAnalyzer;
    
    private Profile? _currentProfile;
    private UICustomization? _currentCustomization;
    private float _skinRotationX = 0f;
    private float _skinRotationY = 0f;
    private System.Windows.Forms.Timer? _friendStatusTimer;

    public MainForm(
        IProfileManager profileManager,
        IMinecraftLauncher minecraftLauncher,
        IAnnouncementManager announcementManager,
        ISkinRenderer skinRenderer,
        IStatisticsManager statisticsManager,
        IFriendManager friendManager,
        IPValidator ipValidator,
        ConfigurationManager configManager,
        ErrorLogger errorLogger,
        CrashAnalyzer crashAnalyzer)
    {
        _profileManager = profileManager;
        _minecraftLauncher = minecraftLauncher;
        _announcementManager = announcementManager;
        _skinRenderer = skinRenderer;
        _statisticsManager = statisticsManager;
        _friendManager = friendManager;
        _ipValidator = ipValidator;
        _configManager = configManager;
        _errorLogger = errorLogger;
        _crashAnalyzer = crashAnalyzer;
        
        InitializeComponent();
        InitializeDarkMode();
        InitializeFriendStatusTimer();
        
        // Custom paint for disabled Stop button to show white text
        stopButton.Paint += StopButton_Paint;
        
        // Register FormClosing event for cleanup
        this.FormClosing += MainForm_FormClosing;
    }
    
    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        try
        {
            // Save all profiles (already persisted by ProfileManager, but ensure final save)
            var profiles = _profileManager.GetAllProfiles();
            foreach (var profile in profiles)
            {
                _profileManager.UpdateProfile(profile);
            }
            
            // Save customization settings for current profile
            if (_currentProfile != null && _currentCustomization != null)
            {
                _configManager.SaveCustomization(_currentCustomization);
            }
            
            // Save last selected profile (already done on selection, but ensure final save)
            if (_currentProfile != null)
            {
                _profileManager.SetLastUsedProfile(_currentProfile.Id);
            }
            
            // Save friend list (FriendManager handles persistence internally)
            // Friends are already saved when added/removed, no additional action needed
            
            // Flush logs
            Serilog.Log.CloseAndFlush();
        }
        catch (Exception ex)
        {
            // Log error but don't prevent shutdown
            _errorLogger.LogError("Error during application shutdown", ex);
        }
    }

    private void InitializeDarkMode()
    {
        // Apply dark mode colors
        this.BackColor = ColorTranslator.FromHtml("#1e1e1e");
        this.ForeColor = ColorTranslator.FromHtml("#ffffff");
        
        // Apply to all child controls
        ApplyDarkModeToControls(this.Controls);
    }

    private void StopButton_Paint(object? sender, PaintEventArgs e)
    {
        // Custom paint to show white text even when disabled
        var button = sender as Button;
        if (button == null) return;

        // Fill background
        using (var brush = new SolidBrush(button.BackColor))
        {
            e.Graphics.FillRectangle(brush, button.ClientRectangle);
        }

        // Draw border
        using (var pen = new Pen(ColorTranslator.FromHtml("#555555"), 1))
        {
            e.Graphics.DrawRectangle(pen, 0, 0, button.Width - 1, button.Height - 1);
        }

        // Draw text in white regardless of enabled state
        using (var brush = new SolidBrush(Color.White))
        {
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            e.Graphics.DrawString(button.Text, button.Font, brush, button.ClientRectangle, sf);
        }
    }

    private void ApplyDarkModeToControls(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {
            if (control is Button button)
            {
                // 더 어두운 배경색으로 변경하여 흰색 텍스트가 잘 보이도록
                button.BackColor = ColorTranslator.FromHtml("#1e1e1e");
                button.ForeColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#555555");
                button.FlatAppearance.BorderSize = 1;
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.BackColor = ColorTranslator.FromHtml("#2d2d30");
                comboBox.ForeColor = Color.White;
                comboBox.FlatStyle = FlatStyle.Flat;
            }
            else if (control is TextBox textBox)
            {
                textBox.BackColor = ColorTranslator.FromHtml("#2d2d30");
                textBox.ForeColor = Color.White;
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is Panel panel)
            {
                panel.BackColor = ColorTranslator.FromHtml("#252526");
                panel.ForeColor = Color.White;
            }
            else if (control is Label label)
            {
                label.ForeColor = Color.White;
            }
            else if (control is ListBox listBox)
            {
                listBox.BackColor = ColorTranslator.FromHtml("#2d2d30");
                listBox.ForeColor = Color.White;
            }
            
            if (control.HasChildren)
            {
                ApplyDarkModeToControls(control.Controls);
            }
        }
    }

    private void InitializeFriendStatusTimer()
    {
        _friendStatusTimer = new System.Windows.Forms.Timer();
        _friendStatusTimer.Interval = 60000; // 60 seconds
        _friendStatusTimer.Tick += async (s, e) => await RefreshFriendStatuses();
        _friendStatusTimer.Start();
    }

    private async void MainForm_Load(object sender, EventArgs e)
    {
        try
        {
            // Load configuration
            var config = _configManager.LoadConfiguration();
            
            // Load profiles and select last used profile
            await LoadProfiles();
            
            // Fetch announcements from server (with fallback to cache)
            await FetchAnnouncementsFromServer();
            
            // Load friends and check their statuses
            await LoadFriends();
            await RefreshFriendStatuses();
        }
        catch (Exception ex)
        {
            _errorLogger.LogError("Error during application startup", ex);
            MessageBox.Show("Some features may not be available due to startup errors. Check logs for details.", 
                "Startup Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
    
    private async Task FetchAnnouncementsFromServer()
    {
        try
        {
            // Try to fetch from server (assuming server URL is available)
            // For now, we'll use cached announcements as the server API is not implemented
            await LoadAnnouncements();
        }
        catch (Exception ex)
        {
            _errorLogger.LogError("Error fetching announcements from server", ex);
            // Fall back to cached announcements
            await LoadAnnouncements();
        }
    }

    private async Task LoadProfiles()
    {
        try
        {
            var profiles = _profileManager.GetAllProfiles();
            profileComboBox.Items.Clear();
            profileComboBox.DisplayMember = "Name";
            
            foreach (var profile in profiles)
            {
                profileComboBox.Items.Add(profile);
            }
            
            // Load last used profile
            var lastProfile = _profileManager.GetLastUsedProfile();
            if (lastProfile != null)
            {
                // Find and select the matching profile
                for (int i = 0; i < profileComboBox.Items.Count; i++)
                {
                    if (profileComboBox.Items[i] is Profile p && p.Id == lastProfile.Id)
                    {
                        profileComboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
            else if (profileComboBox.Items.Count > 0)
            {
                profileComboBox.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading profiles: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void profileComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (profileComboBox.SelectedItem is Profile profile)
        {
            _currentProfile = profile;
            _profileManager.SetLastUsedProfile(profile.Id);
            _ = LoadProfileData(profile);
        }
    }

    private async Task LoadProfileData(Profile profile)
    {
        // Load customization for this profile
        LoadCustomization(profile.Id);
        
        // Load statistics if username is available
        if (!string.IsNullOrEmpty(usernameTextBox.Text))
        {
            await LoadStatistics(usernameTextBox.Text);
        }
    }

    private void LoadCustomization(string profileId)
    {
        try
        {
            _currentCustomization = _configManager.LoadCustomization(profileId);
            ApplyCustomization(_currentCustomization);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading customization: {ex.Message}");
            // Use default customization on error
        }
    }

    private void ApplyCustomization(UICustomization customization)
    {
        // Apply custom logo if available
        if (!string.IsNullOrEmpty(customization.LogoPath) && File.Exists(customization.LogoPath))
        {
            try
            {
                if (logoPictureBox != null)
                {
                    logoPictureBox.Image?.Dispose();
                    logoPictureBox.Image = Image.FromFile(customization.LogoPath);
                    logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading custom logo: {ex.Message}");
            }
        }

        // Apply custom background if available
        if (!string.IsNullOrEmpty(customization.BackgroundPath) && File.Exists(customization.BackgroundPath))
        {
            try
            {
                this.BackgroundImage?.Dispose();
                this.BackgroundImage = Image.FromFile(customization.BackgroundPath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading custom background: {ex.Message}");
            }
        }
        else
        {
            // Clear background if no custom background
            this.BackgroundImage?.Dispose();
            this.BackgroundImage = null;
        }
    }

    private async void launchButton_Click(object sender, EventArgs e)
    {
        if (_currentProfile == null)
        {
            MessageBox.Show("Please select a profile first.", "No Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(usernameTextBox.Text))
        {
            MessageBox.Show("Please enter your Minecraft username.", "No Username", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            launchButton.Enabled = false;
            launchButton.Text = "Launching...";
            progressBar.Visible = true;
            progressBar.Value = 0;

            var progress = new Progress<int>(value =>
            {
                progressBar.Value = Math.Min(value, 100);
            });

            var result = await _minecraftLauncher.LaunchMinecraftAsync(_currentProfile, usernameTextBox.Text);

            if (result.Success)
            {
                // Running 상태: 초록색 배경, 흰색 텍스트
                launchButton.Text = "Running";
                launchButton.BackColor = ColorTranslator.FromHtml("#28a745"); // 초록색
                launchButton.ForeColor = ColorTranslator.FromHtml("#FFFFFF"); // 흰색
                launchButton.Enabled = false;
                
                // Stop 버튼: 빨간색 배경, 흰색 텍스트
                stopButton.BackColor = ColorTranslator.FromHtml("#dc3545"); // 빨간색
                stopButton.ForeColor = ColorTranslator.FromHtml("#FFFFFF"); // 흰색
                stopButton.Enabled = true;
                
                // Ask user if they want to close the launcher
                var closeResult = MessageBox.Show(
                    "Minecraft has been launched successfully!\n\nDo you want to close the launcher?",
                    "Launch Successful",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );
                
                if (closeResult == DialogResult.Yes)
                {
                    // Close the launcher completely
                    Application.Exit();
                }
                else
                {
                    // Just minimize
                    this.WindowState = FormWindowState.Minimized;
                }
            }
            else
            {
                ShowErrorDialog("Failed to launch Minecraft", result.ErrorMessage ?? "Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _errorLogger.LogError("Error launching Minecraft", ex);
            ShowErrorDialog("Error launching Minecraft", ex.ToString());
        }
        finally
        {
            // Only reset if launch failed
            if (launchButton.Text == "Launching...")
            {
                launchButton.Enabled = true;
                launchButton.Text = "Launch Minecraft";
                launchButton.ForeColor = ColorTranslator.FromHtml("#FFFFFF"); // 흰색
                stopButton.Enabled = false;
            }
            progressBar.Visible = false;
        }
    }

    private void stopButton_Click(object sender, EventArgs e)
    {
        try
        {
            var result = MessageBox.Show(
                "Are you sure you want to stop Minecraft?\n\nThis will forcefully terminate the game process.",
                "Stop Minecraft",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                _minecraftLauncher.KillMinecraftProcess();
                
                // Reset UI - 다크모드 색상으로 복원, 흰색 텍스트
                launchButton.Text = "Launch Minecraft";
                launchButton.BackColor = ColorTranslator.FromHtml("#1e1e1e"); // 다크 배경
                launchButton.ForeColor = ColorTranslator.FromHtml("#FFFFFF"); // 흰색
                launchButton.Enabled = true;
                
                stopButton.BackColor = ColorTranslator.FromHtml("#1e1e1e"); // 다크 배경
                stopButton.ForeColor = ColorTranslator.FromHtml("#FFFFFF"); // 흰색
                stopButton.Enabled = false;
                
                MessageBox.Show(
                    "Minecraft has been stopped.",
                    "Stopped",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        catch (Exception ex)
        {
            _errorLogger.LogError("Error stopping Minecraft", ex);
            ShowErrorDialog("Error stopping Minecraft", ex.ToString());
        }
    }

    private void ShowErrorDialog(string message, string technicalDetails)
    {
        try
        {
            using var dialog = new ErrorDialog(message, technicalDetails, _errorLogger);
            dialog.ShowDialog(this);
        }
        catch (Exception ex)
        {
            // Fallback to MessageBox if ErrorDialog fails
            MessageBox.Show($"{message}\n\nDetails: {technicalDetails}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadAnnouncements()
    {
        try
        {
            // For now, use cached announcements
            var announcements = _announcementManager.GetCachedAnnouncements();
            DisplayAnnouncements(announcements);
        }
        catch (Exception ex)
        {
            // Log error but don't show to user
            Console.WriteLine($"Error loading announcements: {ex.Message}");
        }
    }

    private void DisplayAnnouncements(List<Announcement> announcements)
    {
        announcementPanel.Controls.Clear();
        
        int yOffset = 10;
        foreach (var announcement in announcements.Take(5)) // Show top 5
        {
            var announcementControl = CreateAnnouncementControl(announcement);
            announcementControl.Location = new Point(10, yOffset);
            announcementPanel.Controls.Add(announcementControl);
            yOffset += announcementControl.Height + 10;
        }
    }

    private Panel CreateAnnouncementControl(Announcement announcement)
    {
        var panel = new Panel
        {
            Width = announcementPanel.Width - 30,
            Height = 80,
            BackColor = announcement.IsRead ? ColorTranslator.FromHtml("#2d2d30") : ColorTranslator.FromHtml("#3e3e42"),
            BorderStyle = BorderStyle.FixedSingle
        };

        var titleLabel = new Label
        {
            Text = announcement.Title,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(10, 10),
            AutoSize = true
        };

        var contentLabel = new Label
        {
            Text = announcement.Content.Length > 100 ? announcement.Content.Substring(0, 100) + "..." : announcement.Content,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.LightGray,
            Location = new Point(10, 35),
            Width = panel.Width - 20,
            Height = 35
        };

        panel.Controls.Add(titleLabel);
        panel.Controls.Add(contentLabel);
        
        panel.Click += (s, e) => MarkAnnouncementAsRead(announcement);

        return panel;
    }

    private void MarkAnnouncementAsRead(Announcement announcement)
    {
        if (!announcement.IsRead)
        {
            _announcementManager.MarkAnnouncementAsRead(announcement.Id);
            _ = LoadAnnouncements(); // Refresh display
        }
    }

    private async void usernameTextBox_Leave(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(usernameTextBox.Text))
        {
            await LoadSkin(usernameTextBox.Text);
            await LoadStatistics(usernameTextBox.Text);
        }
    }

    private async Task LoadSkin(string username)
    {
        try
        {
            var skin = await _skinRenderer.FetchSkinAsync(username);
            if (skin != null)
            {
                var rendered = _skinRenderer.RenderSkin3D(skin, _skinRotationX, _skinRotationY);
                skinPictureBox.Image = rendered;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading skin: {ex.Message}");
            // Show default skin on error
        }
    }

    private void skinPictureBox_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            skinPictureBox.Tag = e.Location;
        }
    }

    private void skinPictureBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && skinPictureBox.Tag is Point startPoint)
        {
            var dx = e.X - startPoint.X;
            var dy = e.Y - startPoint.Y;
            
            _skinRotationY += dx * 0.5f;
            _skinRotationX += dy * 0.5f;
            
            skinPictureBox.Tag = e.Location;
            
            if (!string.IsNullOrWhiteSpace(usernameTextBox.Text))
            {
                _ = LoadSkin(usernameTextBox.Text);
            }
        }
    }

    private async Task LoadStatistics(string username)
    {
        try
        {
            // Use cached statistics for now
            var stats = await _statisticsManager.GetCachedStatsAsync(username);
            if (stats != null)
            {
                DisplayStatistics(stats);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading statistics: {ex.Message}");
        }
    }

    private void DisplayStatistics(PlayerStats stats)
    {
        statsPlaytimeLabel.Text = $"Playtime: {stats.TotalPlaytime.Hours}h {stats.TotalPlaytime.Minutes}m";
        statsKillsLabel.Text = $"Kills: {stats.Kills}";
        statsDeathsLabel.Text = $"Deaths: {stats.Deaths}";
        statsKDLabel.Text = $"K/D: {stats.KDRatio:F2}";
        statsAchievementsLabel.Text = $"Achievements: {stats.AchievementCompletionPercentage}%";
    }

    private async void refreshStatsButton_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(usernameTextBox.Text))
        {
            await LoadStatistics(usernameTextBox.Text);
        }
    }

    private async Task LoadFriends()
    {
        try
        {
            var friends = _friendManager.GetAllFriends();
            DisplayFriends(friends);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading friends: {ex.Message}");
        }
    }

    private void DisplayFriends(List<Friend> friends)
    {
        friendListBox.Items.Clear();
        
        foreach (var friend in friends)
        {
            var status = friend.IsOnline ? "Online" : "Offline";
            var display = $"{friend.Username} - {status}";
            if (friend.IsOnline && !string.IsNullOrEmpty(friend.CurrentServer))
            {
                display += $" ({friend.CurrentServer})";
            }
            friendListBox.Items.Add(display);
        }
    }

    private async Task RefreshFriendStatuses()
    {
        await LoadFriends();
    }

    private void addFriendButton_Click(object sender, EventArgs e)
    {
        var friendName = Microsoft.VisualBasic.Interaction.InputBox("Enter friend's username:", "Add Friend", "");
        if (!string.IsNullOrWhiteSpace(friendName))
        {
            _friendManager.AddFriend(friendName);
            _ = LoadFriends();
        }
    }

    private void removeFriendButton_Click(object sender, EventArgs e)
    {
        if (friendListBox.SelectedItem != null)
        {
            var selectedText = friendListBox.SelectedItem.ToString();
            var username = selectedText?.Split('-')[0].Trim();
            
            if (!string.IsNullOrEmpty(username))
            {
                var result = MessageBox.Show($"Remove {username} from friends?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    _friendManager.RemoveFriend(username);
                    _ = LoadFriends();
                }
            }
        }
    }

    private void newProfileButton_Click(object sender, EventArgs e)
    {
        try
        {
            using var dialog = new ProfileDialog(_ipValidator, Serilog.Log.Logger);
            if (dialog.ShowDialog(this) == DialogResult.OK && dialog.Profile != null)
            {
                var profile = dialog.Profile;
                _profileManager.CreateProfile(
                    profile.Name,
                    profile.ServerIp,
                    profile.MinecraftVersion,
                    profile.ModLoader,
                    profile.ServerType
                );
                
                _ = LoadProfiles();
                MessageBox.Show("Profile created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            _errorLogger.LogError("Error creating profile", ex);
            ShowErrorDialog("Error creating profile", ex.ToString());
        }
    }

    private void editProfileButton_Click(object sender, EventArgs e)
    {
        if (_currentProfile == null)
        {
            MessageBox.Show("Please select a profile to edit.", "No Profile Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            using var dialog = new ProfileDialog(_ipValidator, Serilog.Log.Logger, _currentProfile);
            if (dialog.ShowDialog(this) == DialogResult.OK && dialog.Profile != null)
            {
                _profileManager.UpdateProfile(dialog.Profile);
                _ = LoadProfiles();
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            _errorLogger.LogError("Error updating profile", ex);
            ShowErrorDialog("Error updating profile", ex.ToString());
        }
    }

    private void deleteProfileButton_Click(object sender, EventArgs e)
    {
        if (_currentProfile == null)
        {
            MessageBox.Show("Please select a profile to delete.", "No Profile Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Are you sure you want to delete the profile '{_currentProfile.Name}'?\n\nThis will remove all associated data and cannot be undone.",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (result == DialogResult.Yes)
        {
            try
            {
                var profileId = _currentProfile.Id;
                _currentProfile = null;
                _profileManager.DeleteProfile(profileId);
                _ = LoadProfiles();
                MessageBox.Show("Profile deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _errorLogger.LogError("Error deleting profile", ex);
                ShowErrorDialog("Error deleting profile", ex.ToString());
            }
        }
    }

    private void customizeButton_Click(object sender, EventArgs e)
    {
        if (_currentProfile == null)
        {
            MessageBox.Show("Please select a profile to customize.", "No Profile Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            using var dialog = new CustomizationDialog(_currentProfile.Id, _configManager);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                // Reload customization
                LoadCustomization(_currentProfile.Id);
                MessageBox.Show("Customization applied successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            _errorLogger.LogError("Error applying customization", ex);
            ShowErrorDialog("Error applying customization", ex.ToString());
        }
    }

    private void settingsButton_Click(object sender, EventArgs e)
    {
        try
        {
            using var dialog = new SettingsDialog(_errorLogger);
            dialog.ShowDialog(this);
        }
        catch (Exception ex)
        {
            _errorLogger.LogError("Error opening settings", ex);
            ShowErrorDialog("Error opening settings", ex.ToString());
        }
    }

    private async void analyzeCrashButton_Click(object sender, EventArgs e)
    {
        try
        {
            analyzeCrashButton.Enabled = false;
            analyzeCrashButton.Text = "Analyzing...";
            
            var result = await _crashAnalyzer.AnalyzeLatestCrashAsync();
            
            if (result.Success)
            {
                using var dialog = new CrashAnalysisDialog(result);
                dialog.ShowDialog(this);
            }
            else
            {
                MessageBox.Show(
                    result.ErrorMessage ?? "No crash reports found",
                    "Crash Analysis",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        catch (Exception ex)
        {
            _errorLogger.LogError("Error analyzing crash", ex);
            ShowErrorDialog("Error analyzing crash", ex.ToString());
        }
        finally
        {
            analyzeCrashButton.Enabled = true;
            analyzeCrashButton.Text = "Analyze Crash";
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _friendStatusTimer?.Stop();
            _friendStatusTimer?.Dispose();
            
            // Dispose of custom images
            logoPictureBox?.Image?.Dispose();
            this.BackgroundImage?.Dispose();
        }
        base.Dispose(disposing);
    }
}
