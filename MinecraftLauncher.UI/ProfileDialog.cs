using MinecraftLauncher.Core.Models;
using MinecraftLauncher.Core.Validators;
using MinecraftLauncher.Core.Managers;
using MinecraftLauncher.Core.Services;
using Serilog;

namespace MinecraftLauncher.UI;

public partial class ProfileDialog : Form
{
    private readonly IPValidator _ipValidator;
    private readonly ILogger _logger;
    private Profile? _profile;
    private bool _isEditMode;
    private ModLoaderManager? _modLoaderManager;

    public Profile? Profile => _profile;

    public ProfileDialog(IPValidator ipValidator, ILogger logger, Profile? existingProfile = null)
    {
        _ipValidator = ipValidator ?? throw new ArgumentNullException(nameof(ipValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _isEditMode = existingProfile != null;
        _profile = existingProfile;
        
        // Initialize mod loader manager for installation
        var httpClient = new HttpClientService();
        var fileDownloader = new FileDownloadManager(httpClient);
        _modLoaderManager = new ModLoaderManager(httpClient, fileDownloader);
        
        InitializeComponent();
        InitializeDarkMode();
        InitializeDropdowns();
        
        if (existingProfile != null)
        {
            LoadProfile(existingProfile);
            this.Text = "Edit Profile";
        }
        else
        {
            this.Text = "New Profile";
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

    private void ApplyDarkModeToControls(Control.ControlCollection controls)
    {
        foreach (Control control in controls)
        {
            if (control is Button || control is ComboBox || control is TextBox || control is Label)
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

    private void InitializeDropdowns()
    {
        // Populate Minecraft version dropdown
        versionComboBox.Items.Clear();
        versionComboBox.Items.Add("1.21.11");
        versionComboBox.Items.Add("1.21.1");
        versionComboBox.Items.Add("1.20.1");
        versionComboBox.SelectedIndex = 0;
        
        // Populate Mod Loader dropdown
        modLoaderComboBox.Items.Clear();
        modLoaderComboBox.Items.Add(ModLoaderType.Vanilla);
        modLoaderComboBox.Items.Add(ModLoaderType.Forge);
        modLoaderComboBox.Items.Add(ModLoaderType.Fabric);
        modLoaderComboBox.Items.Add(ModLoaderType.Paper);
        modLoaderComboBox.SelectedIndex = 0;
        
        // Populate Server Type dropdown
        serverTypeComboBox.Items.Clear();
        serverTypeComboBox.Items.Add(ServerType.PluginServer);
        serverTypeComboBox.Items.Add(ServerType.ModServer);
        serverTypeComboBox.SelectedIndex = 0;
        
        // Add event handler for mod loader change
        modLoaderComboBox.SelectedIndexChanged += ModLoaderComboBox_SelectedIndexChanged;
        
        // Initialize server type based on default mod loader
        UpdateServerTypeAvailability();
    }
    
    private void ModLoaderComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateServerTypeAvailability();
    }
    
    private void UpdateServerTypeAvailability()
    {
        if (modLoaderComboBox.SelectedItem == null)
            return;
            
        var selectedModLoader = (ModLoaderType)modLoaderComboBox.SelectedItem;
        
        // Vanilla and Paper only support PluginServer
        if (selectedModLoader == ModLoaderType.Vanilla || selectedModLoader == ModLoaderType.Paper)
        {
            serverTypeComboBox.Items.Clear();
            serverTypeComboBox.Items.Add(ServerType.PluginServer);
            serverTypeComboBox.SelectedIndex = 0;
            serverTypeComboBox.Enabled = false;
        }
        else
        {
            // Forge and Fabric support both server types
            serverTypeComboBox.Enabled = true;
            
            // Preserve current selection if possible
            var currentSelection = serverTypeComboBox.SelectedItem;
            
            serverTypeComboBox.Items.Clear();
            serverTypeComboBox.Items.Add(ServerType.PluginServer);
            serverTypeComboBox.Items.Add(ServerType.ModServer);
            
            // Restore selection or default to PluginServer
            if (currentSelection != null && serverTypeComboBox.Items.Contains(currentSelection))
            {
                serverTypeComboBox.SelectedItem = currentSelection;
            }
            else
            {
                serverTypeComboBox.SelectedIndex = 0;
            }
        }
    }

    private void LoadProfile(Profile profile)
    {
        nameTextBox.Text = profile.Name;
        serverIpTextBox.Text = profile.ServerIp;
        
        // Set version
        int versionIndex = versionComboBox.Items.IndexOf(profile.MinecraftVersion);
        if (versionIndex >= 0)
        {
            versionComboBox.SelectedIndex = versionIndex;
        }
        
        // Set mod loader (this will trigger UpdateServerTypeAvailability)
        modLoaderComboBox.SelectedItem = profile.ModLoader;
        
        // Set server type (after mod loader is set)
        serverTypeComboBox.SelectedItem = profile.ServerType;
        
        // Set memory settings
        minMemoryTextBox.Text = (profile.MinMemory ?? 512).ToString();
        maxMemoryTextBox.Text = (profile.MaxMemory ?? 2048).ToString();
    }

    private void saveButton_Click(object sender, EventArgs e)
    {
        if (ValidateInputs())
        {
            _profile = new Profile
            {
                Id = _isEditMode && _profile != null ? _profile.Id : Guid.NewGuid().ToString(),
                Name = nameTextBox.Text.Trim(),
                ServerIp = serverIpTextBox.Text.Trim(),
                MinecraftVersion = versionComboBox.SelectedItem?.ToString() ?? "1.21.1",
                ModLoader = (ModLoaderType)(modLoaderComboBox.SelectedItem ?? ModLoaderType.Vanilla),
                ServerType = (ServerType)(serverTypeComboBox.SelectedItem ?? ServerType.PluginServer),
                MinMemory = int.TryParse(minMemoryTextBox.Text, out int minMem) ? minMem : 512,
                MaxMemory = int.TryParse(maxMemoryTextBox.Text, out int maxMem) ? maxMem : 2048,
                CreatedAt = _isEditMode && _profile != null ? _profile.CreatedAt : DateTime.UtcNow,
                LastUsed = DateTime.UtcNow
            };
            
            _logger.Information("Profile {Action}: {ProfileName}", _isEditMode ? "updated" : "created", _profile.Name);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }

    private bool ValidateInputs()
    {
        // Clear previous error
        errorLabel.Text = string.Empty;
        errorLabel.Visible = false;
        
        // Validate name
        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            ShowError("Profile name is required.");
            nameTextBox.Focus();
            return false;
        }
        
        if (nameTextBox.Text.Trim().Length > 50)
        {
            ShowError("Profile name must be 50 characters or less.");
            nameTextBox.Focus();
            return false;
        }
        
        // Validate server IP
        if (string.IsNullOrWhiteSpace(serverIpTextBox.Text))
        {
            ShowError("Server IP is required.");
            serverIpTextBox.Focus();
            return false;
        }
        
        if (!_ipValidator.ValidateServerAddress(serverIpTextBox.Text.Trim()))
        {
            ShowError("Invalid server IP or domain name. Please enter a valid IPv4 address or domain name.");
            serverIpTextBox.Focus();
            return false;
        }
        
        // Validate version
        if (versionComboBox.SelectedItem == null)
        {
            ShowError("Please select a Minecraft version.");
            versionComboBox.Focus();
            return false;
        }
        
        // Validate mod loader
        if (modLoaderComboBox.SelectedItem == null)
        {
            ShowError("Please select a mod loader type.");
            modLoaderComboBox.Focus();
            return false;
        }
        
        // Validate server type
        if (serverTypeComboBox.SelectedItem == null)
        {
            ShowError("Please select a server type.");
            serverTypeComboBox.Focus();
            return false;
        }
        
        // Validate memory settings
        if (!int.TryParse(minMemoryTextBox.Text, out int minMem) || minMem < 256)
        {
            ShowError("Minimum memory must be at least 256 MB.");
            minMemoryTextBox.Focus();
            return false;
        }
        
        if (!int.TryParse(maxMemoryTextBox.Text, out int maxMem) || maxMem < 512)
        {
            ShowError("Maximum memory must be at least 512 MB.");
            maxMemoryTextBox.Focus();
            return false;
        }
        
        if (minMem > maxMem)
        {
            ShowError("Minimum memory cannot be greater than maximum memory.");
            minMemoryTextBox.Focus();
            return false;
        }
        
        return true;
    }

    private void ShowError(string message)
    {
        errorLabel.Text = message;
        errorLabel.Visible = true;
        _logger.Warning("Profile validation error: {ErrorMessage}", message);
    }

    private async void installModLoaderButton_Click(object sender, EventArgs e)
    {
        // Validate inputs first
        if (versionComboBox.SelectedItem == null)
        {
            ShowError("Please select a Minecraft version first.");
            versionComboBox.Focus();
            return;
        }

        if (modLoaderComboBox.SelectedItem == null)
        {
            ShowError("Please select a mod loader type first.");
            modLoaderComboBox.Focus();
            return;
        }

        var version = versionComboBox.SelectedItem.ToString();
        var modLoader = (ModLoaderType)modLoaderComboBox.SelectedItem;

        if (modLoader == ModLoaderType.Vanilla)
        {
            MessageBox.Show("Vanilla doesn't require mod loader installation.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        if (modLoader == ModLoaderType.Paper)
        {
            var result = MessageBox.Show(
                $"Paper will be automatically downloaded and configured.\n\n" +
                $"This will:\n" +
                $"1. Download Paper JAR for Minecraft {version}\n" +
                $"2. Create a server directory\n" +
                $"3. Generate start.bat file\n" +
                $"4. Accept EULA automatically\n\n" +
                $"Continue?",
                "Install Paper",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    installModLoaderButton.Enabled = false;
                    installModLoaderButton.Text = "Downloading...";
                    
                    await DownloadAndSetupPaperAsync(version!);
                    
                    MessageBox.Show(
                        "Paper server setup complete!\n\n" +
                        $"Server directory: paper_server_{version}\n" +
                        "Run 'start.bat' to start the server.",
                        "Setup Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to setup Paper server");
                    MessageBox.Show(
                        $"Failed to setup Paper server:\n\n{ex.Message}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                finally
                {
                    installModLoaderButton.Enabled = true;
                    installModLoaderButton.Text = "Install Mod Loader";
                }
            }
            return;
        }

        // For Forge, just open the download page
        if (modLoader == ModLoaderType.Forge)
        {
            var forgeUrl = "https://files.minecraftforge.net/net/minecraftforge/forge/";
            
            var result = MessageBox.Show(
                $"Forge installer download page will open in your browser.\n\n" +
                $"Please:\n" +
                $"1. Select Minecraft version {version}\n" +
                $"2. Download the Installer\n" +
                $"3. Run the downloaded file\n" +
                $"4. Select 'Install client' and click OK\n\n" +
                $"Open download page now?",
                "Install Forge",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = forgeUrl,
                        UseShellExecute = true
                    });
                    
                    _logger.Information("Opened Forge download page: {Url}", forgeUrl);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to open Forge download page");
                    MessageBox.Show(
                        $"Failed to open browser.\n\n" +
                        $"Please manually visit:\n{forgeUrl}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            return;
        }

        // For Fabric, download and install automatically
        var fabricResult = MessageBox.Show(
            $"This will download and install Fabric for Minecraft {version}.\n\n" +
            $"The installation will run automatically.\n\n" +
            $"Do you want to continue?",
            "Install Fabric",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (fabricResult != DialogResult.Yes)
            return;

        try
        {
            installModLoaderButton.Enabled = false;
            installModLoaderButton.Text = "Installing...";
            errorLabel.Visible = false;

            _logger.Information("Installing Fabric for version {Version}", version);

            // Download and run the official installer
            var success = await DownloadAndRunInstallerAsync(version!, modLoader);

            if (!success)
            {
                ShowError($"Failed to install {modLoader}. Check logs for details.");
                _logger.Error("{ModLoader} installation failed", modLoader);
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error downloading mod loader installer");
            ShowError($"Error: {ex.Message}");
            MessageBox.Show(
                $"An error occurred while downloading the {modLoader} installer:\n\n{ex.Message}",
                "Download Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        finally
        {
            installModLoaderButton.Enabled = true;
            installModLoaderButton.Text = "Install Mod Loader";
        }
    }

    private async Task<bool> DownloadAndRunInstallerAsync(string version, ModLoaderType modLoader)
    {
        try
        {
            string installerUrl;
            string installerFileName;

            if (modLoader == ModLoaderType.Fabric)
            {
                // Fabric installer URL (latest version 1.1.1)
                installerUrl = "https://maven.fabricmc.net/net/fabricmc/fabric-installer/1.1.1/fabric-installer-1.1.1.jar";
                installerFileName = "fabric-installer-1.1.1.jar";
            }
            else if (modLoader == ModLoaderType.Forge)
            {
                // Forge installer URL (version-specific)
                // For Forge, we'll use the files.minecraftforge.net download page
                installerUrl = $"https://maven.minecraftforge.net/net/minecraftforge/forge/{version}-latest/forge-{version}-latest-installer.jar";
                installerFileName = $"forge-{version}-installer.jar";
            }
            else
            {
                return false;
            }

            // Download to temp directory
            var tempPath = Path.GetTempPath();
            var installerPath = Path.Combine(tempPath, installerFileName);

            _logger.Information("Downloading installer from {Url} to {Path}", installerUrl, installerPath);

            // Download the installer
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(5);
                
                try
                {
                    var response = await httpClient.GetAsync(installerUrl);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.Error("Failed to download installer: {StatusCode}", response.StatusCode);
                        MessageBox.Show(
                            $"Failed to download {modLoader} installer.\n\n" +
                            $"HTTP Status: {response.StatusCode}\n\n" +
                            $"Please download manually from:\n" +
                            $"Fabric: https://fabricmc.net/use/installer\n" +
                            $"Forge: https://files.minecraftforge.net/net/minecraftforge/forge/",
                            "Download Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return false;
                    }

                    var content = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(installerPath, content);
                    
                    _logger.Information("Downloaded {Size} bytes to {Path}", content.Length, installerPath);
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    _logger.Error(ex, "Network error downloading installer");
                    MessageBox.Show(
                        $"Network error downloading {modLoader} installer:\n\n{ex.Message}\n\n" +
                        $"Please check your internet connection and try again.",
                        "Network Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return false;
                }
            }

            _logger.Information("Installer downloaded successfully, launching...");

            // Check if Java is available
            if (!IsJavaAvailable())
            {
                _logger.Error("Java not found in PATH");
                MessageBox.Show(
                    "Java is not installed or not in your system PATH.\n\n" +
                    "Please install Java 8 or higher and try again.\n\n" +
                    "Download Java from: https://adoptium.net/",
                    "Java Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }

            // Run the installer
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "java",
                UseShellExecute = false,  // Changed to false to capture output
                CreateNoWindow = true,     // Hide console window
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // For Fabric, add command-line arguments for automatic installation
            if (modLoader == ModLoaderType.Fabric)
            {
                var minecraftDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ".minecraft"
                );

                startInfo.Arguments = $"-jar \"{installerPath}\" client -mcversion {version} -dir \"{minecraftDir}\"";
                
                _logger.Information("Running Fabric installer with arguments: {Args}", startInfo.Arguments);
            }
            else
            {
                // For Forge, just run the GUI installer
                startInfo.Arguments = $"-jar \"{installerPath}\"";
                startInfo.UseShellExecute = true;  // GUI needs shell execute
                startInfo.CreateNoWindow = false;
                startInfo.RedirectStandardOutput = false;
                startInfo.RedirectStandardError = false;
                _logger.Information("Running Forge installer GUI");
            }

            var process = System.Diagnostics.Process.Start(startInfo);

            if (process == null)
            {
                _logger.Error("Failed to start installer process");
                MessageBox.Show(
                    "Failed to start the installer.\n\n" +
                    $"You can manually run it from:\n{installerPath}",
                    "Launch Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }

            // For Fabric, wait for installation to complete
            if (modLoader == ModLoaderType.Fabric)
            {
                _logger.Information("Waiting for Fabric installation to complete...");
                
                // Read output in real-time
                var output = new System.Text.StringBuilder();
                var error = new System.Text.StringBuilder();
                
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        output.AppendLine(e.Data);
                        _logger.Information("Fabric Installer: {Output}", e.Data);
                    }
                };
                
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        error.AppendLine(e.Data);
                        _logger.Warning("Fabric Installer Error: {Error}", e.Data);
                    }
                };
                
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                
                // Wait for process to complete (max 5 minutes)
                var waitTask = Task.Run(() => process.WaitForExit(300000));
                
                // Show progress while waiting
                var progressForm = new Form
                {
                    Text = "Installing Fabric",
                    Width = 400,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = ColorTranslator.FromHtml("#1e1e1e")
                };
                
                var progressLabel = new Label
                {
                    Text = "Installing Fabric, please wait...\n\nThis may take a few minutes.",
                    AutoSize = false,
                    Width = 360,
                    Height = 80,
                    Left = 20,
                    Top = 20,
                    ForeColor = ColorTranslator.FromHtml("#ffffff"),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                
                progressForm.Controls.Add(progressLabel);
                progressForm.FormClosing += (s, e) => e.Cancel = !waitTask.IsCompleted;
                
                // Show progress form and wait
                var timer = new System.Windows.Forms.Timer { Interval = 100 };
                timer.Tick += (s, e) =>
                {
                    if (waitTask.IsCompleted)
                    {
                        timer.Stop();
                        progressForm.Close();
                    }
                };
                timer.Start();
                
                progressForm.ShowDialog();
                
                if (!waitTask.Result)
                {
                    _logger.Error("Fabric installation timed out");
                    process.Kill();
                    MessageBox.Show(
                        "Fabric installation timed out.\n\n" +
                        "The installation took too long. Please try again or install manually.",
                        "Installation Timeout",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return false;
                }
                
                var exitCode = process.ExitCode;
                _logger.Information("Fabric installer exited with code: {ExitCode}", exitCode);
                
                if (exitCode != 0)
                {
                    _logger.Error("Fabric installation failed with exit code: {ExitCode}", exitCode);
                    MessageBox.Show(
                        $"Fabric installation failed (Exit code: {exitCode})\n\n" +
                        $"Error output:\n{error}\n\n" +
                        $"Please check the logs or try manual installation.",
                        "Installation Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return false;
                }
                
                _logger.Information("Fabric installation completed successfully");
                
                MessageBox.Show(
                    "Fabric installation completed successfully!\n\n" +
                    $"Fabric has been installed for Minecraft {version}.\n" +
                    "You can now launch the game with Fabric mods.",
                    "Installation Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in DownloadAndRunInstallerAsync");
            MessageBox.Show(
                $"Unexpected error:\n\n{ex.Message}\n\n" +
                $"Please check the logs for more details.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            return false;
        }
    }

    private bool IsJavaAvailable()
    {
        try
        {
            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "java",
                Arguments = "-version",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = System.Diagnostics.Process.Start(startInfo))
            {
                if (process == null)
                    return false;

                process.WaitForExit(3000); // Wait max 3 seconds
                return process.ExitCode == 0;
            }
        }
        catch
        {
            return false;
        }
    }

    private async Task DownloadAndSetupPaperAsync(string version)
    {
        _logger.Information("Setting up Paper server for version {Version}", version);

        // Create server directory
        var serverDir = Path.Combine(Environment.CurrentDirectory, $"paper_server_{version}");
        Directory.CreateDirectory(serverDir);
        _logger.Information("Created server directory: {ServerDir}", serverDir);

        // Get Paper API to find latest build
        var apiUrl = $"https://api.papermc.io/v2/projects/paper/versions/{version}";
        
        using (var httpClient = new System.Net.Http.HttpClient())
        {
            httpClient.Timeout = TimeSpan.FromMinutes(10);

            // Get latest build number
            _logger.Information("Fetching Paper builds from API: {ApiUrl}", apiUrl);
            var buildsJson = await httpClient.GetStringAsync(apiUrl);
            var buildsDoc = System.Text.Json.JsonDocument.Parse(buildsJson);
            var builds = buildsDoc.RootElement.GetProperty("builds").EnumerateArray().ToList();
            var latestBuild = builds.Last().GetInt32();
            
            _logger.Information("Latest Paper build: {Build}", latestBuild);

            // Download Paper JAR
            var downloadUrl = $"https://api.papermc.io/v2/projects/paper/versions/{version}/builds/{latestBuild}/downloads/paper-{version}-{latestBuild}.jar";
            var jarPath = Path.Combine(serverDir, $"paper-{version}-{latestBuild}.jar");
            
            _logger.Information("Downloading Paper from: {Url}", downloadUrl);
            
            var jarBytes = await httpClient.GetByteArrayAsync(downloadUrl);
            await File.WriteAllBytesAsync(jarPath, jarBytes);
            
            _logger.Information("Downloaded Paper JAR: {Size} bytes", jarBytes.Length);

            // Create eula.txt
            var eulaPath = Path.Combine(serverDir, "eula.txt");
            await File.WriteAllTextAsync(eulaPath, "eula=true\n");
            _logger.Information("Created eula.txt");

            // Get memory settings from textboxes
            int minMem = int.TryParse(minMemoryTextBox.Text, out int min) ? min : 512;
            int maxMem = int.TryParse(maxMemoryTextBox.Text, out int max) ? max : 2048;

            // Create start.bat
            var startBatPath = Path.Combine(serverDir, "start.bat");
            var startBatContent = $@"@echo off
title Paper Server - Minecraft {version}
echo Starting Paper server...
echo.

java -Xms{minMem}M -Xmx{maxMem}M -jar paper-{version}-{latestBuild}.jar --nogui

echo.
echo Server stopped.
pause
";
            await File.WriteAllTextAsync(startBatPath, startBatContent);
            _logger.Information("Created start.bat with memory: Min={MinMem}M, Max={MaxMem}M", minMem, maxMem);

            // Create server.properties with basic settings
            var serverPropsPath = Path.Combine(serverDir, "server.properties");
            var serverProps = $@"#Minecraft server properties
#Generated by MinecraftLauncher
server-port=25565
gamemode=survival
difficulty=normal
max-players=20
online-mode=false
pvp=true
motd=Paper Server - {version}
";
            await File.WriteAllTextAsync(serverPropsPath, serverProps);
            _logger.Information("Created server.properties");

            // Create README
            var readmePath = Path.Combine(serverDir, "README.txt");
            var readmeContent = $@"Paper Server - Minecraft {version}
================================

Setup completed successfully!

Files:
- paper-{version}-{latestBuild}.jar : Paper server JAR
- start.bat : Start the server
- eula.txt : EULA accepted
- server.properties : Server configuration

How to start:
1. Double-click 'start.bat'
2. Wait for server to start
3. Connect to: localhost:25565

Memory Settings:
- Min: {minMem}MB
- Max: {maxMem}MB

To change memory, edit start.bat and modify -Xms and -Xmx values.

Server Directory: {serverDir}
";
            await File.WriteAllTextAsync(readmePath, readmeContent);
            _logger.Information("Created README.txt");
        }

        _logger.Information("Paper server setup completed successfully");
    }
}
