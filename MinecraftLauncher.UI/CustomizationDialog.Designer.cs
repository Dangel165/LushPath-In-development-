namespace MinecraftLauncher.UI
{
    partial class CustomizationDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.labelLogo = new System.Windows.Forms.Label();
            this.btnUploadLogo = new System.Windows.Forms.Button();
            this.pictureBoxLogoPreview = new System.Windows.Forms.PictureBox();
            this.labelBackground = new System.Windows.Forms.Label();
            this.btnUploadBackground = new System.Windows.Forms.Button();
            this.pictureBoxBackgroundPreview = new System.Windows.Forms.PictureBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogoPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackgroundPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // labelLogo
            // 
            this.labelLogo.AutoSize = true;
            this.labelLogo.ForeColor = System.Drawing.Color.White;
            this.labelLogo.Location = new System.Drawing.Point(20, 50);
            this.labelLogo.Name = "labelLogo";
            this.labelLogo.Size = new System.Drawing.Size(37, 15);
            this.labelLogo.TabIndex = 0;
            this.labelLogo.Text = "Logo:";
            // 
            // btnUploadLogo
            // 
            this.btnUploadLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnUploadLogo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUploadLogo.ForeColor = System.Drawing.Color.White;
            this.btnUploadLogo.Location = new System.Drawing.Point(20, 70);
            this.btnUploadLogo.Name = "btnUploadLogo";
            this.btnUploadLogo.Size = new System.Drawing.Size(120, 30);
            this.btnUploadLogo.TabIndex = 1;
            this.btnUploadLogo.Text = "Upload Logo";
            this.btnUploadLogo.UseVisualStyleBackColor = false;
            this.btnUploadLogo.Click += new System.EventHandler(this.btnUploadLogo_Click);
            // 
            // pictureBoxLogoPreview
            // 
            this.pictureBoxLogoPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pictureBoxLogoPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxLogoPreview.Location = new System.Drawing.Point(150, 50);
            this.pictureBoxLogoPreview.Name = "pictureBoxLogoPreview";
            this.pictureBoxLogoPreview.Size = new System.Drawing.Size(200, 150);
            this.pictureBoxLogoPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxLogoPreview.TabIndex = 2;
            this.pictureBoxLogoPreview.TabStop = false;
            // 
            // labelBackground
            // 
            this.labelBackground.AutoSize = true;
            this.labelBackground.ForeColor = System.Drawing.Color.White;
            this.labelBackground.Location = new System.Drawing.Point(20, 220);
            this.labelBackground.Name = "labelBackground";
            this.labelBackground.Size = new System.Drawing.Size(76, 15);
            this.labelBackground.TabIndex = 3;
            this.labelBackground.Text = "Background:";
            // 
            // btnUploadBackground
            // 
            this.btnUploadBackground.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnUploadBackground.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUploadBackground.ForeColor = System.Drawing.Color.White;
            this.btnUploadBackground.Location = new System.Drawing.Point(20, 240);
            this.btnUploadBackground.Name = "btnUploadBackground";
            this.btnUploadBackground.Size = new System.Drawing.Size(120, 30);
            this.btnUploadBackground.TabIndex = 4;
            this.btnUploadBackground.Text = "Upload Background";
            this.btnUploadBackground.UseVisualStyleBackColor = false;
            this.btnUploadBackground.Click += new System.EventHandler(this.btnUploadBackground_Click);
            // 
            // pictureBoxBackgroundPreview
            // 
            this.pictureBoxBackgroundPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pictureBoxBackgroundPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxBackgroundPreview.Location = new System.Drawing.Point(150, 220);
            this.pictureBoxBackgroundPreview.Name = "pictureBoxBackgroundPreview";
            this.pictureBoxBackgroundPreview.Size = new System.Drawing.Size(200, 150);
            this.pictureBoxBackgroundPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxBackgroundPreview.TabIndex = 5;
            this.pictureBoxBackgroundPreview.TabStop = false;
            // 
            // btnApply
            // 
            this.btnApply.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.Location = new System.Drawing.Point(150, 400);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 35);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(260, 400);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 35);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.ForeColor = System.Drawing.Color.Gray;
            this.labelInfo.Location = new System.Drawing.Point(20, 10);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(330, 30);
            this.labelInfo.TabIndex = 8;
            this.labelInfo.Text = "Customize your launcher appearance. Supported formats: PNG, JPG (max 10MB)";
            // 
            // CustomizationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(380, 460);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.pictureBoxBackgroundPreview);
            this.Controls.Add(this.btnUploadBackground);
            this.Controls.Add(this.labelBackground);
            this.Controls.Add(this.pictureBoxLogoPreview);
            this.Controls.Add(this.btnUploadLogo);
            this.Controls.Add(this.labelLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomizationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Customize Launcher";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogoPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackgroundPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelLogo;
        private System.Windows.Forms.Button btnUploadLogo;
        private System.Windows.Forms.PictureBox pictureBoxLogoPreview;
        private System.Windows.Forms.Label labelBackground;
        private System.Windows.Forms.Button btnUploadBackground;
        private System.Windows.Forms.PictureBox pictureBoxBackgroundPreview;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelInfo;
    }
}
