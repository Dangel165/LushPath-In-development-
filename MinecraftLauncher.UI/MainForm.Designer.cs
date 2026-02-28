namespace MinecraftLauncher.UI;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;
    
    // Profile controls
    private ComboBox profileComboBox = null!;
    private Label profileLabel = null!;
    private Button newProfileButton = null!;
    private Button editProfileButton = null!;
    private Button deleteProfileButton = null!;
    private Button customizeButton = null!;
    private Button settingsButton = null!;
    private Button analyzeCrashButton = null!;
    private PictureBox logoPictureBox = null!;
    
    // Launch controls
    private Button launchButton = null!;
    private Button stopButton = null!;
    private ProgressBar progressBar = null!;
    private TextBox usernameTextBox = null!;
    private Label usernameLabel = null!;
    
    // Announcement panel
    private Panel announcementPanel = null!;
    private Label announcementTitleLabel = null!;
    
    // Skin preview panel
    private PictureBox skinPictureBox = null!;
    private Label skinLabel = null!;
    
    // Statistics panel
    private Panel statsPanel = null!;
    private Label statsTitleLabel = null!;
    private Label statsPlaytimeLabel = null!;
    private Label statsKillsLabel = null!;
    private Label statsDeathsLabel = null!;
    private Label statsKDLabel = null!;
    private Label statsAchievementsLabel = null!;
    private Button refreshStatsButton = null!;
    
    // Friend list panel
    private Panel friendPanel = null!;
    private Label friendTitleLabel = null!;
    private ListBox friendListBox = null!;
    private Button addFriendButton = null!;
    private Button removeFriendButton = null!;

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        
        // Initialize controls
        this.profileComboBox = new ComboBox();
        this.profileLabel = new Label();
        this.newProfileButton = new Button();
        this.editProfileButton = new Button();
        this.deleteProfileButton = new Button();
        this.customizeButton = new Button();
        this.settingsButton = new Button();
        this.analyzeCrashButton = new Button();
        this.logoPictureBox = new PictureBox();
        this.launchButton = new Button();
        this.stopButton = new Button();
        this.progressBar = new ProgressBar();
        this.usernameTextBox = new TextBox();
        this.usernameLabel = new Label();
        this.announcementPanel = new Panel();
        this.announcementTitleLabel = new Label();
        this.skinPictureBox = new PictureBox();
        this.skinLabel = new Label();
        this.statsPanel = new Panel();
        this.statsTitleLabel = new Label();
        this.statsPlaytimeLabel = new Label();
        this.statsKillsLabel = new Label();
        this.statsDeathsLabel = new Label();
        this.statsKDLabel = new Label();
        this.statsAchievementsLabel = new Label();
        this.refreshStatsButton = new Button();
        this.friendPanel = new Panel();
        this.friendTitleLabel = new Label();
        this.friendListBox = new ListBox();
        this.addFriendButton = new Button();
        this.removeFriendButton = new Button();
        
        ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
        this.announcementPanel.SuspendLayout();
        this.statsPanel.SuspendLayout();
        this.friendPanel.SuspendLayout();
        this.SuspendLayout();
        
        // 
        // profileLabel
        // 
        this.profileLabel.AutoSize = true;
        this.profileLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.profileLabel.Location = new Point(20, 20);
        this.profileLabel.Name = "profileLabel";
        this.profileLabel.Size = new Size(60, 19);
        this.profileLabel.TabIndex = 0;
        this.profileLabel.Text = "Profile:";
        
        // 
        // profileComboBox
        // 
        this.profileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        this.profileComboBox.Font = new Font("Segoe UI", 10F);
        this.profileComboBox.FormattingEnabled = true;
        this.profileComboBox.Location = new Point(90, 17);
        this.profileComboBox.Name = "profileComboBox";
        this.profileComboBox.Size = new Size(190, 25);
        this.profileComboBox.TabIndex = 1;
        this.profileComboBox.SelectedIndexChanged += new EventHandler(this.profileComboBox_SelectedIndexChanged);
        
        // 
        // newProfileButton
        // 
        this.newProfileButton.Font = new Font("Segoe UI", 9F);
        this.newProfileButton.Location = new Point(310, 15);
        this.newProfileButton.Name = "newProfileButton";
        this.newProfileButton.Size = new Size(80, 28);
        this.newProfileButton.TabIndex = 2;
        this.newProfileButton.Text = "New";
        this.newProfileButton.UseVisualStyleBackColor = true;
        this.newProfileButton.Click += new EventHandler(this.newProfileButton_Click);
        
        // 
        // editProfileButton
        // 
        this.editProfileButton.Font = new Font("Segoe UI", 9F);
        this.editProfileButton.Location = new Point(410, 15);
        this.editProfileButton.Name = "editProfileButton";
        this.editProfileButton.Size = new Size(80, 28);
        this.editProfileButton.TabIndex = 3;
        this.editProfileButton.Text = "Edit";
        this.editProfileButton.UseVisualStyleBackColor = true;
        this.editProfileButton.Click += new EventHandler(this.editProfileButton_Click);
        
        // 
        // deleteProfileButton
        // 
        this.deleteProfileButton.Font = new Font("Segoe UI", 9F);
        this.deleteProfileButton.Location = new Point(510, 15);
        this.deleteProfileButton.Name = "deleteProfileButton";
        this.deleteProfileButton.Size = new Size(80, 28);
        this.deleteProfileButton.TabIndex = 4;
        this.deleteProfileButton.Text = "Delete";
        this.deleteProfileButton.UseVisualStyleBackColor = true;
        this.deleteProfileButton.Click += new EventHandler(this.deleteProfileButton_Click);
        
        // 
        // customizeButton
        // 
        this.customizeButton.Font = new Font("Segoe UI", 9F);
        this.customizeButton.Location = new Point(610, 15);
        this.customizeButton.Name = "customizeButton";
        this.customizeButton.Size = new Size(100, 28);
        this.customizeButton.TabIndex = 5;
        this.customizeButton.Text = "Customize";
        this.customizeButton.UseVisualStyleBackColor = true;
        this.customizeButton.Click += new EventHandler(this.customizeButton_Click);
        
        // 
        // settingsButton
        // 
        this.settingsButton.Font = new Font("Segoe UI", 9F);
        this.settingsButton.Location = new Point(730, 15);
        this.settingsButton.Name = "settingsButton";
        this.settingsButton.Size = new Size(80, 28);
        this.settingsButton.TabIndex = 6;
        this.settingsButton.Text = "Settings";
        this.settingsButton.UseVisualStyleBackColor = true;
        this.settingsButton.Click += new EventHandler(this.settingsButton_Click);
        
        // 
        // analyzeCrashButton
        // 
        this.analyzeCrashButton.Font = new Font("Segoe UI", 9F);
        this.analyzeCrashButton.Location = new Point(830, 15);
        this.analyzeCrashButton.Name = "analyzeCrashButton";
        this.analyzeCrashButton.Size = new Size(120, 28);
        this.analyzeCrashButton.TabIndex = 7;
        this.analyzeCrashButton.Text = "Analyze Crash";
        this.analyzeCrashButton.UseVisualStyleBackColor = true;
        this.analyzeCrashButton.Click += new EventHandler(this.analyzeCrashButton_Click);
        
        // 
        // logoPictureBox
        // 
        this.logoPictureBox.Location = new Point(960, 10);
        this.logoPictureBox.Name = "logoPictureBox";
        this.logoPictureBox.Size = new Size(130, 35);
        this.logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        this.logoPictureBox.TabIndex = 11;
        this.logoPictureBox.TabStop = false;
        
        // 
        // usernameLabel
        // 
        this.usernameLabel.AutoSize = true;
        this.usernameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.usernameLabel.Location = new Point(20, 60);
        this.usernameLabel.Name = "usernameLabel";
        this.usernameLabel.Size = new Size(80, 19);
        this.usernameLabel.TabIndex = 5;
        this.usernameLabel.Text = "Username:";
        
        // 
        // usernameTextBox
        // 
        this.usernameTextBox.Font = new Font("Segoe UI", 10F);
        this.usernameTextBox.Location = new Point(110, 57);
        this.usernameTextBox.Name = "usernameTextBox";
        this.usernameTextBox.Size = new Size(280, 25);
        this.usernameTextBox.TabIndex = 6;
        this.usernameTextBox.Leave += new EventHandler(this.usernameTextBox_Leave);
        
        // 
        // launchButton
        // 
        this.launchButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.launchButton.Location = new Point(20, 100);
        this.launchButton.Name = "launchButton";
        this.launchButton.Size = new Size(300, 50);
        this.launchButton.TabIndex = 7;
        this.launchButton.Text = "Launch Minecraft";
        this.launchButton.UseVisualStyleBackColor = true;
        this.launchButton.Click += new EventHandler(this.launchButton_Click);
        
        // 
        // stopButton
        // 
        this.stopButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        this.stopButton.Location = new Point(330, 100);
        this.stopButton.Name = "stopButton";
        this.stopButton.Size = new Size(60, 50);
        this.stopButton.TabIndex = 8;
        this.stopButton.Text = "Stop";
        this.stopButton.BackColor = ColorTranslator.FromHtml("#1e1e1e");
        this.stopButton.ForeColor = ColorTranslator.FromHtml("#FFFFFF");
        this.stopButton.FlatStyle = FlatStyle.Flat;
        this.stopButton.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#555555");
        this.stopButton.FlatAppearance.BorderSize = 1;
        this.stopButton.UseVisualStyleBackColor = false;
        this.stopButton.Enabled = false;
        this.stopButton.Click += new EventHandler(this.stopButton_Click);
        
        // 
        // progressBar
        // 
        this.progressBar.Location = new Point(20, 160);
        this.progressBar.Name = "progressBar";
        this.progressBar.Size = new Size(370, 23);
        this.progressBar.TabIndex = 9;
        this.progressBar.Visible = false;
        
        // 
        // announcementTitleLabel
        // 
        this.announcementTitleLabel.AutoSize = true;
        this.announcementTitleLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        this.announcementTitleLabel.Location = new Point(10, 10);
        this.announcementTitleLabel.Name = "announcementTitleLabel";
        this.announcementTitleLabel.Size = new Size(120, 20);
        this.announcementTitleLabel.TabIndex = 0;
        this.announcementTitleLabel.Text = "Announcements";
        
        // 
        // announcementPanel
        // 
        this.announcementPanel.AutoScroll = true;
        this.announcementPanel.BorderStyle = BorderStyle.FixedSingle;
        this.announcementPanel.Controls.Add(this.announcementTitleLabel);
        this.announcementPanel.Location = new Point(20, 200);
        this.announcementPanel.Name = "announcementPanel";
        this.announcementPanel.Size = new Size(370, 300);
        this.announcementPanel.TabIndex = 6;
        
        // 
        // skinLabel
        // 
        this.skinLabel.AutoSize = true;
        this.skinLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        this.skinLabel.Location = new Point(420, 55);
        this.skinLabel.Name = "skinLabel";
        this.skinLabel.Size = new Size(100, 20);
        this.skinLabel.TabIndex = 7;
        this.skinLabel.Text = "Skin Preview";
        
        // 
        // skinPictureBox
        // 
        this.skinPictureBox.BorderStyle = BorderStyle.FixedSingle;
        this.skinPictureBox.Location = new Point(420, 50);
        this.skinPictureBox.Name = "skinPictureBox";
        this.skinPictureBox.Size = new Size(200, 250);
        this.skinPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        this.skinPictureBox.TabIndex = 8;
        this.skinPictureBox.TabStop = false;
        this.skinPictureBox.MouseDown += new MouseEventHandler(this.skinPictureBox_MouseDown);
        this.skinPictureBox.MouseMove += new MouseEventHandler(this.skinPictureBox_MouseMove);
        
        // 
        // statsTitleLabel
        // 
        this.statsTitleLabel.AutoSize = true;
        this.statsTitleLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        this.statsTitleLabel.Location = new Point(10, 10);
        this.statsTitleLabel.Name = "statsTitleLabel";
        this.statsTitleLabel.Size = new Size(75, 20);
        this.statsTitleLabel.TabIndex = 0;
        this.statsTitleLabel.Text = "Statistics";
        
        // 
        // statsPlaytimeLabel
        // 
        this.statsPlaytimeLabel.AutoSize = true;
        this.statsPlaytimeLabel.Font = new Font("Segoe UI", 9F);
        this.statsPlaytimeLabel.Location = new Point(10, 40);
        this.statsPlaytimeLabel.Name = "statsPlaytimeLabel";
        this.statsPlaytimeLabel.Size = new Size(80, 15);
        this.statsPlaytimeLabel.TabIndex = 1;
        this.statsPlaytimeLabel.Text = "Playtime: 0h 0m";
        
        // 
        // statsKillsLabel
        // 
        this.statsKillsLabel.AutoSize = true;
        this.statsKillsLabel.Font = new Font("Segoe UI", 9F);
        this.statsKillsLabel.Location = new Point(10, 65);
        this.statsKillsLabel.Name = "statsKillsLabel";
        this.statsKillsLabel.Size = new Size(45, 15);
        this.statsKillsLabel.TabIndex = 2;
        this.statsKillsLabel.Text = "Kills: 0";
        
        // 
        // statsDeathsLabel
        // 
        this.statsDeathsLabel.AutoSize = true;
        this.statsDeathsLabel.Font = new Font("Segoe UI", 9F);
        this.statsDeathsLabel.Location = new Point(10, 90);
        this.statsDeathsLabel.Name = "statsDeathsLabel";
        this.statsDeathsLabel.Size = new Size(60, 15);
        this.statsDeathsLabel.TabIndex = 3;
        this.statsDeathsLabel.Text = "Deaths: 0";
        
        // 
        // statsKDLabel
        // 
        this.statsKDLabel.AutoSize = true;
        this.statsKDLabel.Font = new Font("Segoe UI", 9F);
        this.statsKDLabel.Location = new Point(10, 115);
        this.statsKDLabel.Name = "statsKDLabel";
        this.statsKDLabel.Size = new Size(50, 15);
        this.statsKDLabel.TabIndex = 4;
        this.statsKDLabel.Text = "K/D: 0.00";
        
        // 
        // statsAchievementsLabel
        // 
        this.statsAchievementsLabel.AutoSize = true;
        this.statsAchievementsLabel.Font = new Font("Segoe UI", 9F);
        this.statsAchievementsLabel.Location = new Point(10, 140);
        this.statsAchievementsLabel.Name = "statsAchievementsLabel";
        this.statsAchievementsLabel.Size = new Size(110, 15);
        this.statsAchievementsLabel.TabIndex = 5;
        this.statsAchievementsLabel.Text = "Achievements: 0%";
        
        // 
        // refreshStatsButton
        // 
        this.refreshStatsButton.Font = new Font("Segoe UI", 9F);
        this.refreshStatsButton.Location = new Point(10, 170);
        this.refreshStatsButton.Name = "refreshStatsButton";
        this.refreshStatsButton.Size = new Size(180, 30);
        this.refreshStatsButton.TabIndex = 6;
        this.refreshStatsButton.Text = "Refresh Statistics";
        this.refreshStatsButton.UseVisualStyleBackColor = true;
        this.refreshStatsButton.Click += new EventHandler(this.refreshStatsButton_Click);
        
        // 
        // statsPanel
        // 
        this.statsPanel.BorderStyle = BorderStyle.FixedSingle;
        this.statsPanel.Controls.Add(this.statsTitleLabel);
        this.statsPanel.Controls.Add(this.statsPlaytimeLabel);
        this.statsPanel.Controls.Add(this.statsKillsLabel);
        this.statsPanel.Controls.Add(this.statsDeathsLabel);
        this.statsPanel.Controls.Add(this.statsKDLabel);
        this.statsPanel.Controls.Add(this.statsAchievementsLabel);
        this.statsPanel.Controls.Add(this.refreshStatsButton);
        this.statsPanel.Location = new Point(420, 310);
        this.statsPanel.Name = "statsPanel";
        this.statsPanel.Size = new Size(200, 220);
        this.statsPanel.TabIndex = 9;
        
        // 
        // friendTitleLabel
        // 
        this.friendTitleLabel.AutoSize = true;
        this.friendTitleLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        this.friendTitleLabel.Location = new Point(10, 10);
        this.friendTitleLabel.Name = "friendTitleLabel";
        this.friendTitleLabel.Size = new Size(80, 20);
        this.friendTitleLabel.TabIndex = 0;
        this.friendTitleLabel.Text = "Friend List";
        
        // 
        // friendListBox
        // 
        this.friendListBox.Font = new Font("Segoe UI", 9F);
        this.friendListBox.FormattingEnabled = true;
        this.friendListBox.ItemHeight = 15;
        this.friendListBox.Location = new Point(10, 40);
        this.friendListBox.Name = "friendListBox";
        this.friendListBox.Size = new Size(230, 349);
        this.friendListBox.TabIndex = 1;
        
        // 
        // addFriendButton
        // 
        this.addFriendButton.Font = new Font("Segoe UI", 9F);
        this.addFriendButton.Location = new Point(10, 400);
        this.addFriendButton.Name = "addFriendButton";
        this.addFriendButton.Size = new Size(110, 30);
        this.addFriendButton.TabIndex = 2;
        this.addFriendButton.Text = "Add Friend";
        this.addFriendButton.UseVisualStyleBackColor = true;
        this.addFriendButton.Click += new EventHandler(this.addFriendButton_Click);
        
        // 
        // removeFriendButton
        // 
        this.removeFriendButton.Font = new Font("Segoe UI", 9F);
        this.removeFriendButton.Location = new Point(130, 400);
        this.removeFriendButton.Name = "removeFriendButton";
        this.removeFriendButton.Size = new Size(110, 30);
        this.removeFriendButton.TabIndex = 3;
        this.removeFriendButton.Text = "Remove Friend";
        this.removeFriendButton.UseVisualStyleBackColor = true;
        this.removeFriendButton.Click += new EventHandler(this.removeFriendButton_Click);
        
        // 
        // friendPanel
        // 
        this.friendPanel.BorderStyle = BorderStyle.FixedSingle;
        this.friendPanel.Controls.Add(this.friendTitleLabel);
        this.friendPanel.Controls.Add(this.friendListBox);
        this.friendPanel.Controls.Add(this.addFriendButton);
        this.friendPanel.Controls.Add(this.removeFriendButton);
        this.friendPanel.Location = new Point(640, 50);
        this.friendPanel.Name = "friendPanel";
        this.friendPanel.Size = new Size(250, 450);
        this.friendPanel.TabIndex = 10;
        
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1100, 550);
        this.Controls.Add(this.profileLabel);
        this.Controls.Add(this.profileComboBox);
        this.Controls.Add(this.newProfileButton);
        this.Controls.Add(this.editProfileButton);
        this.Controls.Add(this.deleteProfileButton);
        this.Controls.Add(this.customizeButton);
        this.Controls.Add(this.settingsButton);
        this.Controls.Add(this.analyzeCrashButton);
        this.Controls.Add(this.logoPictureBox);
        this.Controls.Add(this.usernameLabel);
        this.Controls.Add(this.usernameTextBox);
        this.Controls.Add(this.launchButton);
        this.Controls.Add(this.stopButton);
        this.Controls.Add(this.progressBar);
        this.Controls.Add(this.announcementPanel);
        this.Controls.Add(this.skinLabel);
        this.Controls.Add(this.skinPictureBox);
        this.Controls.Add(this.statsPanel);
        this.Controls.Add(this.friendPanel);
        this.Name = "MainForm";
        this.Text = "Lush Path";
        this.Load += new EventHandler(this.MainForm_Load);
        
        ((System.ComponentModel.ISupportInitialize)(this.skinPictureBox)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
        this.announcementPanel.ResumeLayout(false);
        this.announcementPanel.PerformLayout();
        this.statsPanel.ResumeLayout(false);
        this.statsPanel.PerformLayout();
        this.friendPanel.ResumeLayout(false);
        this.friendPanel.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
