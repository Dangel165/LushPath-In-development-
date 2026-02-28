using Serilog;
using System.Text;
using System.Text.RegularExpressions;

namespace MinecraftLauncher.Core.Managers;

/// <summary>
/// Analyzes Minecraft crash reports and provides solutions
/// </summary>
public class CrashAnalyzer
{
    private readonly ILogger _logger;
    
    public CrashAnalyzer(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <summary>
    /// Analyzes the most recent crash report
    /// </summary>
    public async Task<CrashAnalysisResult> AnalyzeLatestCrashAsync()
    {
        var crashReportsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ".minecraft", "crash-reports");
        
        if (!Directory.Exists(crashReportsDir))
        {
            return new CrashAnalysisResult
            {
                Success = false,
                ErrorMessage = "No crash reports directory found"
            };
        }
        
        var crashFiles = Directory.GetFiles(crashReportsDir, "*.txt")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .ToList();
        
        if (!crashFiles.Any())
        {
            return new CrashAnalysisResult
            {
                Success = false,
                ErrorMessage = "No crash reports found"
            };
        }
        
        var latestCrash = crashFiles.First();
        return await AnalyzeCrashFileAsync(latestCrash);
    }
    
    /// <summary>
    /// Analyzes a specific crash report file
    /// </summary>
    public async Task<CrashAnalysisResult> AnalyzeCrashFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new CrashAnalysisResult
            {
                Success = false,
                ErrorMessage = "Crash report file not found"
            };
        }
        
        try
        {
            var content = await File.ReadAllTextAsync(filePath);
            var result = new CrashAnalysisResult
            {
                Success = true,
                CrashFilePath = filePath,
                CrashTime = File.GetLastWriteTime(filePath)
            };
            
            // Extract crash information
            result.CrashCause = ExtractCrashCause(content);
            result.MinecraftVersion = ExtractMinecraftVersion(content);
            result.JavaVersion = ExtractJavaVersion(content);
            result.ModsInvolved = ExtractModsInvolved(content);
            result.StackTrace = ExtractStackTrace(content);
            
            // Analyze and provide solutions
            result.PossibleCauses = AnalyzePossibleCauses(content);
            result.SuggestedSolutions = GenerateSolutions(content, result);
            
            _logger.Information("Crash analysis completed for {FilePath}", filePath);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error analyzing crash file {FilePath}", filePath);
            return new CrashAnalysisResult
            {
                Success = false,
                ErrorMessage = $"Error analyzing crash: {ex.Message}"
            };
        }
    }
    
    private string ExtractCrashCause(string content)
    {
        // Look for common crash patterns
        var patterns = new[]
        {
            @"Description: (.+)",
            @"java\.lang\.(\w+Exception): (.+)",
            @"Caused by: (.+)",
            @"Exception in thread ""(.+)"" (.+)"
        };
        
        foreach (var pattern in patterns)
        {
            var match = Regex.Match(content, pattern);
            if (match.Success)
            {
                return match.Groups[match.Groups.Count - 1].Value.Trim();
            }
        }
        
        return "Unknown crash cause";
    }
    
    private string ExtractMinecraftVersion(string content)
    {
        var match = Regex.Match(content, @"Minecraft Version: ([\d\.]+)");
        return match.Success ? match.Groups[1].Value : "Unknown";
    }
    
    private string ExtractJavaVersion(string content)
    {
        var match = Regex.Match(content, @"Java Version: (.+)");
        return match.Success ? match.Groups[1].Value.Trim() : "Unknown";
    }
    
    private List<string> ExtractModsInvolved(string content)
    {
        var mods = new List<string>();
        
        // Look for mod mentions in stack trace
        var modPattern = @"at\s+([a-zA-Z0-9_\.]+)\.";
        var matches = Regex.Matches(content, modPattern);
        
        var modPackages = new HashSet<string>();
        foreach (Match match in matches)
        {
            var package = match.Groups[1].Value;
            // Filter out common Minecraft/Java packages
            if (!package.StartsWith("net.minecraft") &&
                !package.StartsWith("java.") &&
                !package.StartsWith("sun.") &&
                !package.StartsWith("com.mojang"))
            {
                var rootPackage = package.Split('.')[0];
                modPackages.Add(rootPackage);
            }
        }
        
        mods.AddRange(modPackages.Take(5)); // Limit to top 5
        
        return mods;
    }
    
    private string ExtractStackTrace(string content)
    {
        var lines = content.Split('\n');
        var stackTrace = new StringBuilder();
        var inStackTrace = false;
        var lineCount = 0;
        
        foreach (var line in lines)
        {
            if (line.Contains("at ") || line.Contains("Caused by:"))
            {
                inStackTrace = true;
            }
            
            if (inStackTrace)
            {
                stackTrace.AppendLine(line);
                lineCount++;
                
                if (lineCount > 20) // Limit stack trace length
                    break;
            }
            
            if (inStackTrace && string.IsNullOrWhiteSpace(line))
            {
                break;
            }
        }
        
        return stackTrace.ToString();
    }
    
    private List<string> AnalyzePossibleCauses(string content)
    {
        var causes = new List<string>();
        
        // Memory issues
        if (content.Contains("OutOfMemoryError") || content.Contains("Java heap space"))
        {
            causes.Add("메모리 부족 (Out of Memory)");
        }
        
        // Mod conflicts
        if (content.Contains("ClassNotFoundException") || content.Contains("NoClassDefFoundError"))
        {
            causes.Add("모드 충돌 또는 누락된 라이브러리");
        }
        
        // Graphics issues
        if (content.Contains("OpenGL") || content.Contains("LWJGL") || content.Contains("Renderer"))
        {
            causes.Add("그래픽 드라이버 문제");
        }
        
        // Corrupted world
        if (content.Contains("Chunk") || content.Contains("World") || content.Contains("Region"))
        {
            causes.Add("손상된 월드 데이터");
        }
        
        // Mod loading issues
        if (content.Contains("FML") || content.Contains("Forge") || content.Contains("Fabric"))
        {
            causes.Add("모드 로더 문제");
        }
        
        // Network issues
        if (content.Contains("SocketException") || content.Contains("Connection"))
        {
            causes.Add("네트워크 연결 문제");
        }
        
        if (!causes.Any())
        {
            causes.Add("알 수 없는 원인");
        }
        
        return causes;
    }
    
    private List<string> GenerateSolutions(string content, CrashAnalysisResult result)
    {
        var solutions = new List<string>();
        
        // Memory solutions
        if (content.Contains("OutOfMemoryError") || content.Contains("Java heap space"))
        {
            solutions.Add("런처 설정에서 메모리 할당량을 늘려보세요 (최소 4GB 권장)");
            solutions.Add("불필요한 모드를 제거하여 메모리 사용량을 줄이세요");
        }
        
        // Mod conflict solutions
        if (content.Contains("ClassNotFoundException") || content.Contains("NoClassDefFoundError"))
        {
            solutions.Add("모든 모드가 현재 마인크래프트 버전과 호환되는지 확인하세요");
            solutions.Add("최근에 추가한 모드를 제거하고 다시 시도해보세요");
            solutions.Add("모드 로더(Forge/Fabric)가 올바르게 설치되었는지 확인하세요");
        }
        
        // Graphics solutions
        if (content.Contains("OpenGL") || content.Contains("LWJGL") || content.Contains("Renderer"))
        {
            solutions.Add("그래픽 드라이버를 최신 버전으로 업데이트하세요");
            solutions.Add("비디오 설정에서 VBO를 비활성화해보세요");
            solutions.Add("셰이더나 리소스팩을 제거해보세요");
        }
        
        // World corruption solutions
        if (content.Contains("Chunk") || content.Contains("World") || content.Contains("Region"))
        {
            solutions.Add("월드 백업이 있다면 복원해보세요");
            solutions.Add("문제가 있는 청크를 삭제하거나 월드를 새로 생성하세요");
        }
        
        // Mod loader solutions
        if (content.Contains("FML") || content.Contains("Forge") || content.Contains("Fabric"))
        {
            solutions.Add("모드 로더를 재설치해보세요");
            solutions.Add("모든 모드가 같은 모드 로더 버전을 사용하는지 확인하세요");
        }
        
        // Network solutions
        if (content.Contains("SocketException") || content.Contains("Connection"))
        {
            solutions.Add("인터넷 연결을 확인하세요");
            solutions.Add("방화벽이 마인크래프트를 차단하고 있지 않은지 확인하세요");
        }
        
        // Generic solutions
        if (!solutions.Any())
        {
            solutions.Add("마인크래프트를 재설치해보세요");
            solutions.Add("Java를 최신 버전으로 업데이트하세요");
            solutions.Add("모든 모드를 제거하고 바닐라 상태에서 실행해보세요");
        }
        
        // Add mod-specific solutions
        if (result.ModsInvolved.Any())
        {
            solutions.Add($"관련 모드 확인: {string.Join(", ", result.ModsInvolved)}");
        }
        
        return solutions;
    }
}

/// <summary>
/// Result of crash analysis
/// </summary>
public class CrashAnalysisResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CrashFilePath { get; set; }
    public DateTime CrashTime { get; set; }
    public string CrashCause { get; set; } = "";
    public string MinecraftVersion { get; set; } = "";
    public string JavaVersion { get; set; } = "";
    public List<string> ModsInvolved { get; set; } = new();
    public string StackTrace { get; set; } = "";
    public List<string> PossibleCauses { get; set; } = new();
    public List<string> SuggestedSolutions { get; set; } = new();
}
