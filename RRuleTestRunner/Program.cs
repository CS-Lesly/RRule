using System.Diagnostics;
using System.IO;

var assemblyDirectory = AppContext.BaseDirectory;
var repoRoot = Path.GetFullPath(Path.Combine(assemblyDirectory, "..", "..", "..", ".."));
var testProject = Path.Combine(repoRoot, "RRule.Tests.Unit", "Tests", "RRule.Tests.csproj");

if (!File.Exists(testProject))
{
    Console.Error.WriteLine($"Test project not found: {testProject}");
    Environment.Exit(1);
}

var startInfo = new ProcessStartInfo("dotnet", $"test \"{testProject}\" --no-restore")
{
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true
};

using var process = Process.Start(startInfo);
if (process is null)
{
    Console.Error.WriteLine("Unable to start unittest!");
    Environment.Exit(1);
}

Console.WriteLine($"Running unittests from {testProject}...");

process.OutputDataReceived += (_, args) => { if (args.Data is not null) Console.WriteLine(args.Data); };
process.ErrorDataReceived += (_, args) => { if (args.Data is not null) Console.Error.WriteLine(args.Data); };
process.BeginOutputReadLine();
process.BeginErrorReadLine();
process.WaitForExit();
Environment.ExitCode = process.ExitCode;