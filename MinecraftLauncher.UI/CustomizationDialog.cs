using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MinecraftLauncher.Core;
using MinecraftLauncher.Core.Managers;
using MinecraftLauncher.Core.Models;

namespace MinecraftLauncher.UI
{
    public partial class CustomizationDialog : Form
    {
        private readonly ConfigurationManager _configManager;
        private UICustomization _customization;
        private string _logoTempPath = string.Empty;
        private string _backgroundTempPath = string.Empty;

        public UICustomization Customization => _customization;

        public CustomizationDialog(string profileId, ConfigurationManager configManager)
        {
            _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
            _customization = _configManager.LoadCustomization(profileId);
            InitializeComponent();
            LoadCustomization();
        }

        private void LoadCustomization()
        {
            // Load logo preview
            if (!string.IsNullOrEmpty(_customization.LogoPath) && File.Exists(_customization.LogoPath))
            {
                try
                {
                    pictureBoxLogoPreview.Image = Image.FromFile(_customization.LogoPath);
                }
                catch
                {
                    pictureBoxLogoPreview.Image = null;
                }
            }

            // Load background preview
            if (!string.IsNullOrEmpty(_customization.BackgroundPath) && File.Exists(_customization.BackgroundPath))
            {
                try
                {
                    pictureBoxBackgroundPreview.Image = Image.FromFile(_customization.BackgroundPath);
                }
                catch
                {
                    pictureBoxBackgroundPreview.Image = null;
                }
            }
        }

        private void btnUploadLogo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg";
                dialog.Title = "Select Logo Image";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (!ValidateImageFile(dialog.FileName))
                    {
                        MessageBox.Show("Invalid image file. Please select a PNG or JPG file under 10MB.", 
                            "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _logoTempPath = dialog.FileName;
                    pictureBoxLogoPreview.Image = Image.FromFile(dialog.FileName);
                }
            }
        }

        private void btnUploadBackground_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg";
                dialog.Title = "Select Background Image";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (!ValidateImageFile(dialog.FileName))
                    {
                        MessageBox.Show("Invalid image file. Please select a PNG or JPG file under 10MB.", 
                            "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _backgroundTempPath = dialog.FileName;
                    pictureBoxBackgroundPreview.Image = Image.FromFile(dialog.FileName);
                }
            }
        }

        private bool ValidateImageFile(string filePath)
        {
            // Check file exists
            if (!File.Exists(filePath))
                return false;

            // Check file extension
            string extension = Path.GetExtension(filePath).ToLower();
            if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
                return false;

            // Check file size (max 10MB)
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > 10 * 1024 * 1024)
                return false;

            // Try to load as image
            try
            {
                using (Image img = Image.FromFile(filePath))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the customization directory for this profile
                string customizationDir = LauncherPaths.GetProfileCustomizationDirectory(_customization.ProfileId);
                Directory.CreateDirectory(customizationDir);

                // Copy logo to launcher directory if a new one was selected
                if (!string.IsNullOrEmpty(_logoTempPath))
                {
                    string logoFileName = "logo" + Path.GetExtension(_logoTempPath);
                    string logoDestPath = Path.Combine(customizationDir, logoFileName);
                    
                    // Copy the file
                    File.Copy(_logoTempPath, logoDestPath, true);
                    _customization.LogoPath = logoDestPath;
                }

                // Copy background to launcher directory if a new one was selected
                if (!string.IsNullOrEmpty(_backgroundTempPath))
                {
                    string backgroundFileName = "background" + Path.GetExtension(_backgroundTempPath);
                    string backgroundDestPath = Path.Combine(customizationDir, backgroundFileName);
                    
                    // Copy the file
                    File.Copy(_backgroundTempPath, backgroundDestPath, true);
                    _customization.BackgroundPath = backgroundDestPath;
                }

                // Save customization using ConfigurationManager
                _configManager.SaveCustomization(_customization);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save customization: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
