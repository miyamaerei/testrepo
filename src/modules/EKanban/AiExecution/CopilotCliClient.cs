using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VOL.Core.Extensions.AutofacManager;

namespace EKanban.AiExecution;

public class CopilotCliClient : ICopilotCliClient, IDependency
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<CopilotCliClient> _logger;
    private readonly string _copilotPath;

    public CopilotCliClient(
        IConfiguration configuration,
        ILogger<CopilotCliClient> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _copilotPath = _configuration["AiExecution:CopilotPath"] ?? "copilot";
    }

    public async Task<string> ExecutePromptAsync(string prompt, string workingDirectory = null)
    {
        _logger.LogInformation("Executing prompt via Copilot CLI");

        var tempFile = System.IO.Path.GetTempFileName();
        await System.IO.File.WriteAllTextAsync(tempFile, prompt, Encoding.UTF8);

        // 使用参数指定的工作目录，或使用默认目录
        string workingDir = workingDirectory ?? AppContext.BaseDirectory;

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _copilotPath,
                Arguments = $"-p \"$(cat {tempFile})\" --allow-all-tools",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDir
            },
            EnableRaisingEvents = true
        };

        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        try
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var error = errorBuilder.ToString();
                _logger.LogError("Copilot CLI execution failed with exit code {ExitCode}: {Error}", process.ExitCode, error);
                throw new InvalidOperationException($"Copilot CLI failed: {error}");
            }

            var output = outputBuilder.ToString().Trim();
            _logger.LogInformation("Copilot CLI execution completed, output length: {Length} characters", output.Length);

            return output;
        }
        finally
        {
            try
            {
                System.IO.File.Delete(tempFile);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
