namespace MinecraftLauncher.UI;

partial class SettingsDialog
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.GroupBox logsGroupBox;
    private System.Windows.Forms.Button openLogsButton;
    private System.Windows.Forms.Label logsDescriptionLabel;
    private System.Windows.Forms.GroupBox cacheGroupBox;
    private System.Windows.Forms.Button clearCacheButton;
    private System.Windows.Forms.Label cacheDescriptionLabel;
    private System.Windows.Forms.GroupBox languageGroupBox;
    private System.Windows.Forms.ComboBox languageComboBox;
    private System.Windows.Forms.Label languageDescriptionLabel;
    private System.Windows.Forms.GroupBox aboutGroupBox;
    private System.Windows.Forms.Label versionLabel;
    private System.Windows.Forms.Label aboutDescriptionLabel;
    private System.Windows.Forms.Button closeButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.logsGroupBox = new System.Windows.Forms.GroupBox();
        this.openLogsButton = new System.Windows.Forms.Button();
        this.logsDescriptionLabel = new System.Windows.Forms.Label();
        this.cacheGroupBox = new System.Windows.Forms.GroupBox();
        this.clearCacheButton = new System.Windows.Forms.Button();
        this.cacheDescriptionLabel = new System.Windows.Forms.Label();
        this.languageGroupBox = new System.Windows.Forms.GroupBox();
        this.languageComboBox = new System.Windows.Forms.ComboBox();
        this.languageDescriptionLabel = new System.Windows.Forms.Label();
        this.aboutGroupBox = new System.Windows.Forms.GroupBox();
        this.versionLabel = new System.Windows.Forms.Label();
        this.aboutDescriptionLabel = new System.Windows.Forms.Label();
        this.closeButton = new System.Windows.Forms.Button();
        this.logsGroupBox.SuspendLayout();
        this.cacheGroupBox.SuspendLayout();
        this.languageGroupBox.SuspendLayout();
        this.aboutGroupBox.SuspendLayout();
        this.SuspendLayout();
        
        // 
        // logsGroupBox
        // 
        this.logsGroupBox.Controls.Add(this.openLogsButton);
        this.logsGroupBox.Controls.Add(this.logsDescriptionLabel);
        this.logsGroupBox.Location = new System.Drawing.Point(20, 20);
        this.logsGroupBox.Name = "logsGroupBox";
        this.logsGroupBox.Size = new System.Drawing.Size(460, 100);
        this.logsGroupBox.TabIndex = 0;
        this.logsGroupBox.TabStop = false;
        this.logsGroupBox.Text = "Logs";
        
        // 
        // logsDescriptionLabel
        // 
        this.logsDescriptionLabel.AutoSize = true;
        this.logsDescriptionLabel.Location = new System.Drawing.Point(15, 30);
        this.logsDescriptionLabel.Name = "logsDescriptionLabel";
        this.logsDescriptionLabel.Size = new System.Drawing.Size(400, 15);
        this.logsDescriptionLabel.TabIndex = 0;
        this.logsDescriptionLabel.Text = "View launcher logs for troubleshooting and error diagnosis.";
        
        // 
        // openLogsButton
        // 
        this.openLogsButton.Location = new System.Drawing.Point(15, 55);
        this.openLogsButton.Name = "openLogsButton";
        this.openLogsButton.Size = new System.Drawing.Size(150, 30);
        this.openLogsButton.TabIndex = 1;
        this.openLogsButton.Text = "Open Logs Directory";
        this.openLogsButton.UseVisualStyleBackColor = true;
        this.openLogsButton.Click += new System.EventHandler(this.openLogsButton_Click);
        
        // 
        // cacheGroupBox
        // 
        this.cacheGroupBox.Controls.Add(this.clearCacheButton);
        this.cacheGroupBox.Controls.Add(this.cacheDescriptionLabel);
        this.cacheGroupBox.Location = new System.Drawing.Point(20, 130);
        this.cacheGroupBox.Name = "cacheGroupBox";
        this.cacheGroupBox.Size = new System.Drawing.Size(460, 100);
        this.cacheGroupBox.TabIndex = 1;
        this.cacheGroupBox.TabStop = false;
        this.cacheGroupBox.Text = "Cache";
        
        // 
        // cacheDescriptionLabel
        // 
        this.cacheDescriptionLabel.AutoSize = true;
        this.cacheDescriptionLabel.Location = new System.Drawing.Point(15, 30);
        this.cacheDescriptionLabel.Name = "cacheDescriptionLabel";
        this.cacheDescriptionLabel.Size = new System.Drawing.Size(400, 15);
        this.cacheDescriptionLabel.TabIndex = 0;
        this.cacheDescriptionLabel.Text = "Clear cached data including skins, announcements, and statistics.";
        
        // 
        // clearCacheButton
        // 
        this.clearCacheButton.Location = new System.Drawing.Point(15, 55);
        this.clearCacheButton.Name = "clearCacheButton";
        this.clearCacheButton.Size = new System.Drawing.Size(150, 30);
        this.clearCacheButton.TabIndex = 1;
        this.clearCacheButton.Text = "Clear Cache";
        this.clearCacheButton.UseVisualStyleBackColor = true;
        this.clearCacheButton.Click += new System.EventHandler(this.clearCacheButton_Click);
        
        // 
        // languageGroupBox
        // 
        this.languageGroupBox.Controls.Add(this.languageComboBox);
        this.languageGroupBox.Controls.Add(this.languageDescriptionLabel);
        this.languageGroupBox.Location = new System.Drawing.Point(20, 240);
        this.languageGroupBox.Name = "languageGroupBox";
        this.languageGroupBox.Size = new System.Drawing.Size(460, 100);
        this.languageGroupBox.TabIndex = 2;
        this.languageGroupBox.TabStop = false;
        this.languageGroupBox.Text = "Language";
        
        // 
        // languageDescriptionLabel
        // 
        this.languageDescriptionLabel.AutoSize = true;
        this.languageDescriptionLabel.Location = new System.Drawing.Point(15, 30);
        this.languageDescriptionLabel.Name = "languageDescriptionLabel";
        this.languageDescriptionLabel.Size = new System.Drawing.Size(400, 15);
        this.languageDescriptionLabel.TabIndex = 0;
        this.languageDescriptionLabel.Text = "Select launcher language (feature coming soon).";
        
        // 
        // languageComboBox
        // 
        this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.languageComboBox.Enabled = false;
        this.languageComboBox.FormattingEnabled = true;
        this.languageComboBox.Items.AddRange(new object[] { "English (Default)" });
        this.languageComboBox.Location = new System.Drawing.Point(15, 55);
        this.languageComboBox.Name = "languageComboBox";
        this.languageComboBox.Size = new System.Drawing.Size(200, 23);
        this.languageComboBox.TabIndex = 1;
        this.languageComboBox.SelectedIndex = 0;
        
        // 
        // aboutGroupBox
        // 
        this.aboutGroupBox.Controls.Add(this.versionLabel);
        this.aboutGroupBox.Controls.Add(this.aboutDescriptionLabel);
        this.aboutGroupBox.Location = new System.Drawing.Point(20, 350);
        this.aboutGroupBox.Name = "aboutGroupBox";
        this.aboutGroupBox.Size = new System.Drawing.Size(460, 100);
        this.aboutGroupBox.TabIndex = 3;
        this.aboutGroupBox.TabStop = false;
        this.aboutGroupBox.Text = "About";
        
        // 
        // aboutDescriptionLabel
        // 
        this.aboutDescriptionLabel.AutoSize = true;
        this.aboutDescriptionLabel.Location = new System.Drawing.Point(15, 30);
        this.aboutDescriptionLabel.Name = "aboutDescriptionLabel";
        this.aboutDescriptionLabel.Size = new System.Drawing.Size(400, 15);
        this.aboutDescriptionLabel.TabIndex = 0;
        this.aboutDescriptionLabel.Text = "Minecraft Custom Launcher - A community-focused server launcher";
        
        // 
        // versionLabel
        // 
        this.versionLabel.AutoSize = true;
        this.versionLabel.Location = new System.Drawing.Point(15, 55);
        this.versionLabel.Name = "versionLabel";
        this.versionLabel.Size = new System.Drawing.Size(100, 15);
        this.versionLabel.TabIndex = 1;
        this.versionLabel.Text = "Version: 1.0.0";
        
        // 
        // closeButton
        // 
        this.closeButton.Location = new System.Drawing.Point(380, 470);
        this.closeButton.Name = "closeButton";
        this.closeButton.Size = new System.Drawing.Size(100, 35);
        this.closeButton.TabIndex = 4;
        this.closeButton.Text = "Close";
        this.closeButton.UseVisualStyleBackColor = true;
        this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
        
        // 
        // SettingsDialog
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(500, 520);
        this.Controls.Add(this.closeButton);
        this.Controls.Add(this.aboutGroupBox);
        this.Controls.Add(this.languageGroupBox);
        this.Controls.Add(this.cacheGroupBox);
        this.Controls.Add(this.logsGroupBox);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "SettingsDialog";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Settings";
        this.logsGroupBox.ResumeLayout(false);
        this.logsGroupBox.PerformLayout();
        this.cacheGroupBox.ResumeLayout(false);
        this.cacheGroupBox.PerformLayout();
        this.languageGroupBox.ResumeLayout(false);
        this.languageGroupBox.PerformLayout();
        this.aboutGroupBox.ResumeLayout(false);
        this.aboutGroupBox.PerformLayout();
        this.ResumeLayout(false);
    }
}
