namespace MinecraftLauncher.UI;

partial class ErrorDialog
{
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.PictureBox errorIconPictureBox;
    private System.Windows.Forms.Label errorTitleLabel;
    private System.Windows.Forms.Label errorMessageLabel;
    private System.Windows.Forms.Button toggleDetailsButton;
    private System.Windows.Forms.Label technicalDetailsLabel;
    private System.Windows.Forms.TextBox technicalDetailsTextBox;
    private System.Windows.Forms.Button copyToClipboardButton;
    private System.Windows.Forms.Button openLogsButton;
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
        this.errorIconPictureBox = new System.Windows.Forms.PictureBox();
        this.errorTitleLabel = new System.Windows.Forms.Label();
        this.errorMessageLabel = new System.Windows.Forms.Label();
        this.toggleDetailsButton = new System.Windows.Forms.Button();
        this.technicalDetailsLabel = new System.Windows.Forms.Label();
        this.technicalDetailsTextBox = new System.Windows.Forms.TextBox();
        this.copyToClipboardButton = new System.Windows.Forms.Button();
        this.openLogsButton = new System.Windows.Forms.Button();
        this.closeButton = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.errorIconPictureBox)).BeginInit();
        this.SuspendLayout();
        
        // 
        // errorIconPictureBox
        // 
        this.errorIconPictureBox.Location = new System.Drawing.Point(20, 20);
        this.errorIconPictureBox.Name = "errorIconPictureBox";
        this.errorIconPictureBox.Size = new System.Drawing.Size(48, 48);
        this.errorIconPictureBox.TabIndex = 0;
        this.errorIconPictureBox.TabStop = false;
        this.errorIconPictureBox.Image = SystemIcons.Error.ToBitmap();
        this.errorIconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        
        // 
        // errorTitleLabel
        // 
        this.errorTitleLabel.AutoSize = true;
        this.errorTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
        this.errorTitleLabel.ForeColor = System.Drawing.Color.FromArgb(255, 100, 100);
        this.errorTitleLabel.Location = new System.Drawing.Point(80, 20);
        this.errorTitleLabel.Name = "errorTitleLabel";
        this.errorTitleLabel.Size = new System.Drawing.Size(150, 21);
        this.errorTitleLabel.TabIndex = 1;
        this.errorTitleLabel.Text = "An Error Occurred";
        
        // 
        // errorMessageLabel
        // 
        this.errorMessageLabel.Location = new System.Drawing.Point(80, 50);
        this.errorMessageLabel.Name = "errorMessageLabel";
        this.errorMessageLabel.Size = new System.Drawing.Size(470, 60);
        this.errorMessageLabel.TabIndex = 2;
        this.errorMessageLabel.Text = "Error message will appear here.";
        
        // 
        // toggleDetailsButton
        // 
        this.toggleDetailsButton.Location = new System.Drawing.Point(20, 120);
        this.toggleDetailsButton.Name = "toggleDetailsButton";
        this.toggleDetailsButton.Size = new System.Drawing.Size(120, 30);
        this.toggleDetailsButton.TabIndex = 3;
        this.toggleDetailsButton.Text = "Show Details";
        this.toggleDetailsButton.UseVisualStyleBackColor = true;
        this.toggleDetailsButton.Click += new System.EventHandler(this.toggleDetailsButton_Click);
        
        // 
        // technicalDetailsLabel
        // 
        this.technicalDetailsLabel.AutoSize = true;
        this.technicalDetailsLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        this.technicalDetailsLabel.Location = new System.Drawing.Point(20, 160);
        this.technicalDetailsLabel.Name = "technicalDetailsLabel";
        this.technicalDetailsLabel.Size = new System.Drawing.Size(130, 19);
        this.technicalDetailsLabel.TabIndex = 4;
        this.technicalDetailsLabel.Text = "Technical Details:";
        
        // 
        // technicalDetailsTextBox
        // 
        this.technicalDetailsTextBox.Font = new System.Drawing.Font("Consolas", 9F);
        this.technicalDetailsTextBox.Location = new System.Drawing.Point(20, 185);
        this.technicalDetailsTextBox.Multiline = true;
        this.technicalDetailsTextBox.Name = "technicalDetailsTextBox";
        this.technicalDetailsTextBox.ReadOnly = true;
        this.technicalDetailsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.technicalDetailsTextBox.Size = new System.Drawing.Size(530, 220);
        this.technicalDetailsTextBox.TabIndex = 5;
        
        // 
        // copyToClipboardButton
        // 
        this.copyToClipboardButton.Location = new System.Drawing.Point(150, 120);
        this.copyToClipboardButton.Name = "copyToClipboardButton";
        this.copyToClipboardButton.Size = new System.Drawing.Size(150, 30);
        this.copyToClipboardButton.TabIndex = 6;
        this.copyToClipboardButton.Text = "Copy to Clipboard";
        this.copyToClipboardButton.UseVisualStyleBackColor = true;
        this.copyToClipboardButton.Click += new System.EventHandler(this.copyToClipboardButton_Click);
        
        // 
        // openLogsButton
        // 
        this.openLogsButton.Location = new System.Drawing.Point(310, 120);
        this.openLogsButton.Name = "openLogsButton";
        this.openLogsButton.Size = new System.Drawing.Size(120, 30);
        this.openLogsButton.TabIndex = 7;
        this.openLogsButton.Text = "Open Logs";
        this.openLogsButton.UseVisualStyleBackColor = true;
        this.openLogsButton.Click += new System.EventHandler(this.openLogsButton_Click);
        
        // 
        // closeButton
        // 
        this.closeButton.Location = new System.Drawing.Point(450, 120);
        this.closeButton.Name = "closeButton";
        this.closeButton.Size = new System.Drawing.Size(100, 30);
        this.closeButton.TabIndex = 8;
        this.closeButton.Text = "Close";
        this.closeButton.UseVisualStyleBackColor = true;
        this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
        
        // 
        // ErrorDialog
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(570, 220);
        this.Controls.Add(this.closeButton);
        this.Controls.Add(this.openLogsButton);
        this.Controls.Add(this.copyToClipboardButton);
        this.Controls.Add(this.technicalDetailsTextBox);
        this.Controls.Add(this.technicalDetailsLabel);
        this.Controls.Add(this.toggleDetailsButton);
        this.Controls.Add(this.errorMessageLabel);
        this.Controls.Add(this.errorTitleLabel);
        this.Controls.Add(this.errorIconPictureBox);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "ErrorDialog";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Error";
        ((System.ComponentModel.ISupportInitialize)(this.errorIconPictureBox)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
