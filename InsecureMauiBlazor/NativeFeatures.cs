using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace InsecureMauiBlazor
{
    public class NativeFeatures
    {
        // VULNERABILITY: Execute arbitrary commands
        public async Task<string> ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return "Empty command";

            try
            {
                string[] parts = command.Split(' ');
                string cmd = parts[0];
                string args = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "";

                StringBuilder output = new StringBuilder();

                // Simulate command execution (in a real vulnerable app, this might actually execute commands)
                if (cmd == "get_files")
                {
                    // Simulate listing files
                    string path = args;
                    if (string.IsNullOrEmpty(path))
                        path = Environment.CurrentDirectory;

                    try
                    {
                        var files = Directory.GetFiles(path);
                        output.AppendLine($"Files in {path}:");
                        foreach (var file in files)
                        {
                            output.AppendLine(Path.GetFileName(file));
                        }
                    }
                    catch
                    {
                        output.AppendLine($"Cannot access {path}");
                    }
                }
                else if (cmd == "read_file")
                {
                    // Simulate reading a file
                    string filepath = args;
                    try
                    {
                        if (File.Exists(filepath))
                        {
                            output.AppendLine($"Content of {filepath}:");
                            output.AppendLine(File.ReadAllText(filepath));
                        }
                        else
                        {
                            output.AppendLine($"File not found: {filepath}");
                        }
                    }
                    catch
                    {
                        output.AppendLine($"Cannot read {filepath}");
                    }
                }
                else if (cmd == "device_info")
                {
                    // Return device information
                    output.AppendLine("Device Information:");
                    output.AppendLine($"Model: {DeviceInfo.Current.Model}");
                    output.AppendLine($"Manufacturer: {DeviceInfo.Current.Manufacturer}");
                    output.AppendLine($"OS Version: {DeviceInfo.Current.VersionString}");
                }
                else
                {
                    output.AppendLine($"Unknown command: {cmd}");
                }

                return output.ToString();
            }
            catch (Exception ex)
            {
                return $"Error executing command: {ex.Message}";
            }
        }

        // VULNERABILITY: Access sensitive device information without permission checks
        public string GetDeviceSensitiveInfo()
        {
            StringBuilder info = new StringBuilder();

            info.AppendLine("Device Sensitive Information:");
            info.AppendLine($"Device ID: {Guid.NewGuid()}"); // Simulated device ID
            info.AppendLine($"Platform: {DeviceInfo.Current.Platform}");

            // In a real app, this might access more sensitive information

            return info.ToString();
        }

        // VULNERABILITY: File operations without proper access control
        public async Task<string> ReadFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return await File.ReadAllTextAsync(path);
                }
                return $"File not found: {path}";
            }
            catch (Exception ex)
            {
                return $"Error reading file: {ex.Message}";
            }
        }

        // VULNERABILITY: Write file without proper access control
        public async Task<string> WriteFile(string path, string content)
        {
            try
            {
                await File.WriteAllTextAsync(path, content);
                return $"Successfully wrote to {path}";
            }
            catch (Exception ex)
            {
                return $"Error writing file: {ex.Message}";
            }
        }
    }
}
