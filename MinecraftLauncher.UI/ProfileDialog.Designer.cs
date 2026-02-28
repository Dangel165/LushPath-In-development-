namespace MinecraftLauncher.UI;

partial class ProfileDialog
{
    private System.ComponentModel.IContainer components = null;
    
    private Label nameLabel = null!;
    private TextBox nameTextBox = null!;
    private Label serverIpLabel = null!;
    private TextBox serverIpTextBox = null!;
    private Label versionLabel = null!;
    private ComboBox versionComboBox = null!;
    private Label modLoaderLabel = null!;
    private ComboBox modLoaderComboBox = null!;
    private Label serverTypeLabel = null!;
    private ComboBox serverTypeComboBox = null!;
    private Label minMemoryLabel = null!;
    private TextBox minMemoryTextBox = null!;
    private Label maxMemoryLabel = null!;
    private TextBox maxMemoryTextBox = null!;
    private Button installModLoaderButton = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;
    private Label errorLabel = null!;

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
        this.components = new System.ComponentModel.Container();
        
        this.nameLabel = new Label();
        this.nameTextBox = new TextBox();
        this.serverIpLabel = new Label();
        this.serverIpTextBox = new TextBox();
        this.versionLabel = new Label();
        this.versionComboBox = new ComboBox();
        this.modLoaderLabel = new Label();
        this.modLoaderComboBox = new ComboBox();
        this.serverTypeLabel = new Label();
        this.serverTypeComboBox = new ComboBox();
        this.minMemoryLabel = new Label();
        this.minMemoryTextBox = new TextBox();
        this.maxMemoryLabel = new Label();
        this.maxMemoryTextBox = new TextBox();
        this.installModLoaderButton = new Button();
        this.saveButton = new Button();
        this.cancelButton = new Button();
        this.errorLabel = new Label();
        
        this.SuspendLayout();
        
        // 
        // nameLabel
        // 
        this.nameLabel.AutoSize = true;
        this.nameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.nameLabel.Location = new Point(20, 20);
        this.nameLabel.Name = "nameLabel";
        this.nameLabel.Size = new Size(100, 19);
        this.nameLabel.TabIndex = 0;
        this.nameLabel.Text = "Profile Name:";
        
        // 
        // nameTextBox
        // 
        this.nameTextBox.Font = new Font("Segoe UI", 10F);
        this.nameTextBox.Location = new Point(150, 17);
        this.nameTextBox.Name = "nameTextBox";
        this.nameTextBox.Size = new Size(300, 25);
        this.nameTextBox.TabIndex = 1;
        this.nameTextBox.MaxLength = 50;
        
        // 
        // serverIpLabel
        // 
        this.serverIpLabel.AutoSize = true;
        this.serverIpLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.serverIpLabel.Location = new Point(20, 60);
        this.serverIpLabel.Name = "serverIpLabel";
        this.serverIpLabel.Size = new Size(80, 19);
        this.serverIpLabel.TabIndex = 2;
        this.serverIpLabel.Text = "Server IP:";
        
        // 
        // serverIpTextBox
        // 
        this.serverIpTextBox.Font = new Font("Segoe UI", 10F);
        this.serverIpTextBox.Location = new Point(150, 57);
        this.serverIpTextBox.Name = "serverIpTextBox";
        this.serverIpTextBox.Size = new Size(300, 25);
        this.serverIpTextBox.TabIndex = 3;
        this.serverIpTextBox.PlaceholderText = "e.g., 192.168.1.1 or play.example.com";
        
        // 
        // versionLabel
        // 
        this.versionLabel.AutoSize = true;
        this.versionLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.versionLabel.Location = new Point(20, 100);
        this.versionLabel.Name = "versionLabel";
        this.versionLabel.Size = new Size(130, 19);
        this.versionLabel.TabIndex = 4;
        this.versionLabel.Text = "Minecraft Version:";
        
        // 
        // versionComboBox
        // 
        this.versionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        this.versionComboBox.Font = new Font("Segoe UI", 10F);
        this.versionComboBox.FormattingEnabled = true;
        this.versionComboBox.Location = new Point(150, 97);
        this.versionComboBox.Name = "versionComboBox";
        this.versionComboBox.Size = new Size(300, 25);
        this.versionComboBox.TabIndex = 5;
        
        // 
        // modLoaderLabel
        // 
        this.modLoaderLabel.AutoSize = true;
        this.modLoaderLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.modLoaderLabel.Location = new Point(20, 140);
        this.modLoaderLabel.Name = "modLoaderLabel";
        this.modLoaderLabel.Size = new Size(95, 19);
        this.modLoaderLabel.TabIndex = 6;
        this.modLoaderLabel.Text = "Mod Loader:";
        
        // 
        // modLoaderComboBox
        // 
        this.modLoaderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        this.modLoaderComboBox.Font = new Font("Segoe UI", 10F);
        this.modLoaderComboBox.FormattingEnabled = true;
        this.modLoaderComboBox.Location = new Point(150, 137);
        this.modLoaderComboBox.Name = "modLoaderComboBox";
        this.modLoaderComboBox.Size = new Size(300, 25);
        this.modLoaderComboBox.TabIndex = 7;
        
        // 
        // serverTypeLabel
        // 
        this.serverTypeLabel.AutoSize = true;
        this.serverTypeLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.serverTypeLabel.Location = new Point(20, 180);
        this.serverTypeLabel.Name = "serverTypeLabel";
        this.serverTypeLabel.Size = new Size(95, 19);
        this.serverTypeLabel.TabIndex = 8;
        this.serverTypeLabel.Text = "Server Type:";
        
        // 
        // serverTypeComboBox
        // 
        this.serverTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        this.serverTypeComboBox.Font = new Font("Segoe UI", 10F);
        this.serverTypeComboBox.FormattingEnabled = true;
        this.serverTypeComboBox.Location = new Point(150, 177);
        this.serverTypeComboBox.Name = "serverTypeComboBox";
        this.serverTypeComboBox.Size = new Size(300, 25);
        this.serverTypeComboBox.TabIndex = 9;
        
        // 
        // minMemoryLabel
        // 
        this.minMemoryLabel.AutoSize = true;
        this.minMemoryLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.minMemoryLabel.Location = new Point(20, 220);
        this.minMemoryLabel.Name = "minMemoryLabel";
        this.minMemoryLabel.Size = new Size(120, 19);
        this.minMemoryLabel.TabIndex = 10;
        this.minMemoryLabel.Text = "Min Memory (MB):";
        
        // 
        // minMemoryTextBox
        // 
        this.minMemoryTextBox.Font = new Font("Segoe UI", 10F);
        this.minMemoryTextBox.Location = new Point(150, 217);
        this.minMemoryTextBox.Name = "minMemoryTextBox";
        this.minMemoryTextBox.Size = new Size(100, 25);
        this.minMemoryTextBox.TabIndex = 11;
        this.minMemoryTextBox.Text = "512";
        this.minMemoryTextBox.TextAlign = HorizontalAlignment.Right;
        this.minMemoryTextBox.PlaceholderText = "512";
        
        // 
        // maxMemoryLabel
        // 
        this.maxMemoryLabel.AutoSize = true;
        this.maxMemoryLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.maxMemoryLabel.Location = new Point(260, 220);
        this.maxMemoryLabel.Name = "maxMemoryLabel";
        this.maxMemoryLabel.Size = new Size(125, 19);
        this.maxMemoryLabel.TabIndex = 12;
        this.maxMemoryLabel.Text = "Max Memory (MB):";
        
        // 
        // maxMemoryTextBox
        // 
        this.maxMemoryTextBox.Font = new Font("Segoe UI", 10F);
        this.maxMemoryTextBox.Location = new Point(390, 217);
        this.maxMemoryTextBox.Name = "maxMemoryTextBox";
        this.maxMemoryTextBox.Size = new Size(100, 25);
        this.maxMemoryTextBox.TabIndex = 13;
        this.maxMemoryTextBox.Text = "2048";
        this.maxMemoryTextBox.TextAlign = HorizontalAlignment.Right;
        this.maxMemoryTextBox.PlaceholderText = "2048";
        
        // 
        // installModLoaderButton
        // 
        this.installModLoaderButton.Font = new Font("Segoe UI", 9F);
        this.installModLoaderButton.Location = new Point(20, 260);
        this.installModLoaderButton.Name = "installModLoaderButton";
        this.installModLoaderButton.Size = new Size(150, 30);
        this.installModLoaderButton.TabIndex = 14;
        this.installModLoaderButton.Text = "Install Mod Loader";
        this.installModLoaderButton.UseVisualStyleBackColor = true;
        this.installModLoaderButton.Click += new EventHandler(this.installModLoaderButton_Click);
        
        // 
        // errorLabel
        // 
        this.errorLabel.AutoSize = false;
        this.errorLabel.Font = new Font("Segoe UI", 9F);
        this.errorLabel.ForeColor = Color.Red;
        this.errorLabel.Location = new Point(20, 300);
        this.errorLabel.Name = "errorLabel";
        this.errorLabel.Size = new Size(470, 40);
        this.errorLabel.TabIndex = 15;
        this.errorLabel.Text = "";
        this.errorLabel.Visible = false;
        
        // 
        // saveButton
        // 
        this.saveButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.saveButton.Location = new Point(280, 350);
        this.saveButton.Name = "saveButton";
        this.saveButton.Size = new Size(100, 35);
        this.saveButton.TabIndex = 16;
        this.saveButton.Text = "Save";
        this.saveButton.UseVisualStyleBackColor = true;
        this.saveButton.Click += new EventHandler(this.saveButton_Click);
        
        // 
        // cancelButton
        // 
        this.cancelButton.Font = new Font("Segoe UI", 10F);
        this.cancelButton.Location = new Point(390, 350);
        this.cancelButton.Name = "cancelButton";
        this.cancelButton.Size = new Size(100, 35);
        this.cancelButton.TabIndex = 17;
        this.cancelButton.Text = "Cancel";
        this.cancelButton.UseVisualStyleBackColor = true;
        this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
        
        // 
        // ProfileDialog
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(520, 410);
        this.Controls.Add(this.nameLabel);
        this.Controls.Add(this.nameTextBox);
        this.Controls.Add(this.serverIpLabel);
        this.Controls.Add(this.serverIpTextBox);
        this.Controls.Add(this.versionLabel);
        this.Controls.Add(this.versionComboBox);
        this.Controls.Add(this.modLoaderLabel);
        this.Controls.Add(this.modLoaderComboBox);
        this.Controls.Add(this.serverTypeLabel);
        this.Controls.Add(this.serverTypeComboBox);
        this.Controls.Add(this.minMemoryLabel);
        this.Controls.Add(this.minMemoryTextBox);
        this.Controls.Add(this.maxMemoryLabel);
        this.Controls.Add(this.maxMemoryTextBox);
        this.Controls.Add(this.installModLoaderButton);
        this.Controls.Add(this.errorLabel);
        this.Controls.Add(this.saveButton);
        this.Controls.Add(this.cancelButton);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "ProfileDialog";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Profile";
        
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
