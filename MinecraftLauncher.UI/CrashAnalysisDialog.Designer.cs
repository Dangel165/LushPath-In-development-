namespace MinecraftLauncher.UI;

partial class CrashAnalysisDialog
{
    private System.ComponentModel.IContainer components = null;
    
    private Label titleLabel = null!;
    private Label crashTimeLabel = null!;
    private Label crashCauseLabel = null!;
    private Label minecraftVersionLabel = null!;
    private Label javaVersionLabel = null!;
    
    private Label modsLabel = null!;
    private TextBox modsTextBox = null!;
    
    private Label causesLabel = null!;
    private TextBox causesTextBox = null!;
    
    private Label solutionsLabel = null!;
    private TextBox solutionsTextBox = null!;
    
    private Label stackTraceLabel = null!;
    private TextBox stackTraceTextBox = null!;
    
    private Button closeButton = null!;
    private Button copyButton = null!;

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
        
        this.titleLabel = new Label();
        this.crashTimeLabel = new Label();
        this.crashCauseLabel = new Label();
        this.minecraftVersionLabel = new Label();
        this.javaVersionLabel = new Label();
        this.modsLabel = new Label();
        this.modsTextBox = new TextBox();
        this.causesLabel = new Label();
        this.causesTextBox = new TextBox();
        this.solutionsLabel = new Label();
        this.solutionsTextBox = new TextBox();
        this.stackTraceLabel = new Label();
        this.stackTraceTextBox = new TextBox();
        this.closeButton = new Button();
        this.copyButton = new Button();
        
        this.SuspendLayout();
        
        // 
        // titleLabel
        // 
        this.titleLabel.AutoSize = true;
        this.titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        this.titleLabel.Location = new Point(20, 20);
        this.titleLabel.Name = "titleLabel";
        this.titleLabel.Size = new Size(200, 25);
        this.titleLabel.TabIndex = 0;
        this.titleLabel.Text = "Crash Analysis Report";
        
        // 
        // crashTimeLabel
        // 
        this.crashTimeLabel.AutoSize = true;
        this.crashTimeLabel.Font = new Font("Segoe UI", 10F);
        this.crashTimeLabel.Location = new Point(20, 60);
        this.crashTimeLabel.Name = "crashTimeLabel";
        this.crashTimeLabel.Size = new Size(150, 19);
        this.crashTimeLabel.TabIndex = 1;
        this.crashTimeLabel.Text = "Crash Time: Unknown";
        
        // 
        // crashCauseLabel
        // 
        this.crashCauseLabel.AutoSize = true;
        this.crashCauseLabel.Font = new Font("Segoe UI", 10F);
        this.crashCauseLabel.Location = new Point(20, 85);
        this.crashCauseLabel.Name = "crashCauseLabel";
        this.crashCauseLabel.Size = new Size(120, 19);
        this.crashCauseLabel.TabIndex = 2;
        this.crashCauseLabel.Text = "Cause: Unknown";
        
        // 
        // minecraftVersionLabel
        // 
        this.minecraftVersionLabel.AutoSize = true;
        this.minecraftVersionLabel.Font = new Font("Segoe UI", 9F);
        this.minecraftVersionLabel.Location = new Point(20, 110);
        this.minecraftVersionLabel.Name = "minecraftVersionLabel";
        this.minecraftVersionLabel.Size = new Size(150, 15);
        this.minecraftVersionLabel.TabIndex = 3;
        this.minecraftVersionLabel.Text = "Minecraft: Unknown";
        
        // 
        // javaVersionLabel
        // 
        this.javaVersionLabel.AutoSize = true;
        this.javaVersionLabel.Font = new Font("Segoe UI", 9F);
        this.javaVersionLabel.Location = new Point(20, 130);
        this.javaVersionLabel.Name = "javaVersionLabel";
        this.javaVersionLabel.Size = new Size(100, 15);
        this.javaVersionLabel.TabIndex = 4;
        this.javaVersionLabel.Text = "Java: Unknown";
        
        // 
        // modsLabel
        // 
        this.modsLabel.AutoSize = true;
        this.modsLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.modsLabel.Location = new Point(20, 160);
        this.modsLabel.Name = "modsLabel";
        this.modsLabel.Size = new Size(110, 19);
        this.modsLabel.TabIndex = 5;
        this.modsLabel.Text = "Mods Involved:";
        
        // 
        // modsTextBox
        // 
        this.modsTextBox.Font = new Font("Consolas", 9F);
        this.modsTextBox.Location = new Point(20, 185);
        this.modsTextBox.Multiline = true;
        this.modsTextBox.Name = "modsTextBox";
        this.modsTextBox.ReadOnly = true;
        this.modsTextBox.ScrollBars = ScrollBars.Vertical;
        this.modsTextBox.Size = new Size(760, 60);
        this.modsTextBox.TabIndex = 6;
        
        // 
        // causesLabel
        // 
        this.causesLabel.AutoSize = true;
        this.causesLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.causesLabel.Location = new Point(20, 260);
        this.causesLabel.Name = "causesLabel";
        this.causesLabel.Size = new Size(120, 19);
        this.causesLabel.TabIndex = 7;
        this.causesLabel.Text = "Possible Causes:";
        
        // 
        // causesTextBox
        // 
        this.causesTextBox.Font = new Font("Segoe UI", 9F);
        this.causesTextBox.Location = new Point(20, 285);
        this.causesTextBox.Multiline = true;
        this.causesTextBox.Name = "causesTextBox";
        this.causesTextBox.ReadOnly = true;
        this.causesTextBox.ScrollBars = ScrollBars.Vertical;
        this.causesTextBox.Size = new Size(760, 80);
        this.causesTextBox.TabIndex = 8;
        
        // 
        // solutionsLabel
        // 
        this.solutionsLabel.AutoSize = true;
        this.solutionsLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.solutionsLabel.Location = new Point(20, 380);
        this.solutionsLabel.Name = "solutionsLabel";
        this.solutionsLabel.Size = new Size(140, 19);
        this.solutionsLabel.TabIndex = 9;
        this.solutionsLabel.Text = "Suggested Solutions:";
        
        // 
        // solutionsTextBox
        // 
        this.solutionsTextBox.Font = new Font("Segoe UI", 9F);
        this.solutionsTextBox.Location = new Point(20, 405);
        this.solutionsTextBox.Multiline = true;
        this.solutionsTextBox.Name = "solutionsTextBox";
        this.solutionsTextBox.ReadOnly = true;
        this.solutionsTextBox.ScrollBars = ScrollBars.Vertical;
        this.solutionsTextBox.Size = new Size(760, 120);
        this.solutionsTextBox.TabIndex = 10;
        
        // 
        // stackTraceLabel
        // 
        this.stackTraceLabel.AutoSize = true;
        this.stackTraceLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.stackTraceLabel.Location = new Point(20, 540);
        this.stackTraceLabel.Name = "stackTraceLabel";
        this.stackTraceLabel.Size = new Size(90, 19);
        this.stackTraceLabel.TabIndex = 11;
        this.stackTraceLabel.Text = "Stack Trace:";
        
        // 
        // stackTraceTextBox
        // 
        this.stackTraceTextBox.Font = new Font("Consolas", 8F);
        this.stackTraceTextBox.Location = new Point(20, 565);
        this.stackTraceTextBox.Multiline = true;
        this.stackTraceTextBox.Name = "stackTraceTextBox";
        this.stackTraceTextBox.ReadOnly = true;
        this.stackTraceTextBox.ScrollBars = ScrollBars.Both;
        this.stackTraceTextBox.Size = new Size(760, 150);
        this.stackTraceTextBox.TabIndex = 12;
        this.stackTraceTextBox.WordWrap = false;
        
        // 
        // copyButton
        // 
        this.copyButton.Font = new Font("Segoe UI", 10F);
        this.copyButton.Location = new Point(580, 730);
        this.copyButton.Name = "copyButton";
        this.copyButton.Size = new Size(100, 35);
        this.copyButton.TabIndex = 13;
        this.copyButton.Text = "Copy Report";
        this.copyButton.UseVisualStyleBackColor = true;
        this.copyButton.Click += new EventHandler(this.copyButton_Click);
        
        // 
        // closeButton
        // 
        this.closeButton.Font = new Font("Segoe UI", 10F);
        this.closeButton.Location = new Point(690, 730);
        this.closeButton.Name = "closeButton";
        this.closeButton.Size = new Size(90, 35);
        this.closeButton.TabIndex = 14;
        this.closeButton.Text = "Close";
        this.closeButton.UseVisualStyleBackColor = true;
        this.closeButton.Click += new EventHandler(this.closeButton_Click);
        
        // 
        // CrashAnalysisDialog
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(800, 780);
        this.Controls.Add(this.titleLabel);
        this.Controls.Add(this.crashTimeLabel);
        this.Controls.Add(this.crashCauseLabel);
        this.Controls.Add(this.minecraftVersionLabel);
        this.Controls.Add(this.javaVersionLabel);
        this.Controls.Add(this.modsLabel);
        this.Controls.Add(this.modsTextBox);
        this.Controls.Add(this.causesLabel);
        this.Controls.Add(this.causesTextBox);
        this.Controls.Add(this.solutionsLabel);
        this.Controls.Add(this.solutionsTextBox);
        this.Controls.Add(this.stackTraceLabel);
        this.Controls.Add(this.stackTraceTextBox);
        this.Controls.Add(this.copyButton);
        this.Controls.Add(this.closeButton);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "CrashAnalysisDialog";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Crash Analysis";
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
