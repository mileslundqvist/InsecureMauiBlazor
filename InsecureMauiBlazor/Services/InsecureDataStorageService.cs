using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsecureMauiBlazor.Services
{
    // VULNERABILITY: Model with sensitive data but no encryption
    [Table("sensitive_data")]
    public class SensitiveData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public string Key { get; set; }

        // VULNERABILITY: No encryption for values
        public string Value { get; set; }

        // VULNERABILITY: Stores when the data was created (helps attackers identify recent entries)
        public DateTime CreatedAt { get; set; }
    }

    public class InsecureDataStorageService : IDataStorageService, IDisposable
    {
        // Database connection
        private readonly SQLiteConnection _database;

        // VULNERABILITY: Public static paths make it easier for attackers to find files
        public static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "sensitive.db");

        public static readonly string FileStoragePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "sensitive_data");

        // VULNERABILITY: External storage path (accessible to other apps on older Android versions)
        public static readonly string ExternalStoragePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Personal),
            "../Download/sensitive_data");

        public InsecureDataStorageService()
        {
            // VULNERABILITY: No encryption on the database file
            _database = new SQLiteConnection(DbPath);

            // Create tables
            _database.CreateTable<SensitiveData>();

            // Create directories if they don't exist
            Directory.CreateDirectory(FileStoragePath);

            try
            {
                // VULNERABILITY: Attempt to write to external storage
                Directory.CreateDirectory(ExternalStoragePath);
            }
            catch (Exception ex)
            {
                // Just log but continue - not essential if it fails
                Console.WriteLine($"Failed to create external directory: {ex.Message}");
            }

            // VULNERABILITY: Log database creation/connection
            Console.WriteLine($"Opened sensitive database at {DbPath}");
        }

        /// <summary>
        /// Save data insecurely to multiple storage locations
        /// </summary>
        public void SaveInsecurely(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) return;

            try
            {
                // VULNERABILITY: Log sensitive data
                Console.WriteLine($"Saving sensitive data: {key}={value}");

                // VULNERABILITY: Store in SharedPreferences (plain text)
                Preferences.Default.Set(key, value);

                // VULNERABILITY: Store in SQLite database (plain text)
                SaveToDatabase(key, value);

                // VULNERABILITY: Store in local files (plain text)
                SaveToFile(key, value);

                // VULNERABILITY: Store in external storage if available
                SaveToExternalStorage(key, value);

                // VULNERABILITY: Also store data in reverse for certain keys (makes recovery trivial)
                if (IsSensitiveKey(key))
                {
                    var reversedValue = new string(value.ToCharArray().Reverse().ToArray());
                    Preferences.Default.Set($"backup_{key}", reversedValue);
                }
            }
            catch (Exception ex)
            {
                // VULNERABILITY: Error message might reveal sensitive information
                Console.WriteLine($"Error saving {key}={value}: {ex.Message}");
            }
        }

        /// <summary>
        /// Get data from insecure storage
        /// </summary>
        public string GetInsecurely(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            try
            {
                // Try different storage mechanisms in order

                // 1. First check SharedPreferences
                var fromPrefs = Preferences.Default.Get<string>(key, null);
                if (!string.IsNullOrEmpty(fromPrefs))
                {
                    // VULNERABILITY: Log sensitive data retrieval
                    Console.WriteLine($"Retrieved from preferences: {key}={fromPrefs}");
                    return fromPrefs;
                }

                // 2. Then check database
                var fromDb = GetFromDatabase(key);
                if (!string.IsNullOrEmpty(fromDb))
                {
                    // VULNERABILITY: Log sensitive data retrieval
                    Console.WriteLine($"Retrieved from database: {key}={fromDb}");
                    return fromDb;
                }

                // 3. Then check files
                var fromFile = GetFromFile(key);
                if (!string.IsNullOrEmpty(fromFile))
                {
                    // VULNERABILITY: Log sensitive data retrieval
                    Console.WriteLine($"Retrieved from file: {key}={fromFile}");
                    return fromFile;
                }

                // 4. Finally check external storage
                var fromExternal = GetFromExternalStorage(key);
                if (!string.IsNullOrEmpty(fromExternal))
                {
                    // VULNERABILITY: Log sensitive data retrieval
                    Console.WriteLine($"Retrieved from external storage: {key}={fromExternal}");
                    return fromExternal;
                }

                // 5. Check backup values
                if (IsSensitiveKey(key))
                {
                    var reversedValue = Preferences.Default.Get<string>($"backup_{key}", null);
                    if (!string.IsNullOrEmpty(reversedValue))
                    {
                        var originalValue = new string(reversedValue.ToCharArray().Reverse().ToArray());
                        return originalValue;
                    }
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                // VULNERABILITY: Error message might reveal sensitive information
                Console.WriteLine($"Error retrieving {key}: {ex.Message}");
                return string.Empty;
            }
        }

        // VULNERABILITY: Check if key holds sensitive information (helps attackers target valuable data)
        private bool IsSensitiveKey(string key)
        {
            var sensitiveKeys = new[] { "password", "credit", "token", "secret", "key", "auth" };
            return sensitiveKeys.Any(k => key.ToLowerInvariant().Contains(k));
        }

        #region SQLite Database Operations

        // Save to SQLite database
        private void SaveToDatabase(string key, string value)
        {
            try
            {
                // Check if record exists
                var existingRecord = _database.Table<SensitiveData>()
                    .FirstOrDefault(d => d.Key == key);

                if (existingRecord != null)
                {
                    // Update existing record
                    existingRecord.Value = value;
                    existingRecord.CreatedAt = DateTime.UtcNow;
                    _database.Update(existingRecord);
                }
                else
                {
                    // Create new record
                    var newRecord = new SensitiveData
                    {
                        Key = key,
                        Value = value,
                        CreatedAt = DateTime.UtcNow
                    };
                    _database.Insert(newRecord);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
        }

        // Get from SQLite database
        private string GetFromDatabase(string key)
        {
            try
            {
                var record = _database.Table<SensitiveData>()
                    .FirstOrDefault(d => d.Key == key);

                return record?.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database read error: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region File Operations

        // Save to local file
        private void SaveToFile(string key, string value)
        {
            try
            {
                // VULNERABILITY: Store in plain text file with obvious filename
                string filePath = Path.Combine(FileStoragePath, $"{key}.txt");
                File.WriteAllText(filePath, value);

                // VULNERABILITY: For very sensitive data, store a backup file with minimal obfuscation
                if (IsSensitiveKey(key))
                {
                    string backupPath = Path.Combine(FileStoragePath, $".{key}_backup");
                    File.WriteAllText(backupPath, $"SENSITIVE:{value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File write error: {ex.Message}");
            }
        }

        // Get from local file
        private string GetFromFile(string key)
        {
            try
            {
                string filePath = Path.Combine(FileStoragePath, $"{key}.txt");
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }

                // Try backup file
                string backupPath = Path.Combine(FileStoragePath, $".{key}_backup");
                if (File.Exists(backupPath))
                {
                    string content = File.ReadAllText(backupPath);
                    if (content.StartsWith("SENSITIVE:"))
                    {
                        return content.Substring(10);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File read error: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region External Storage Operations

        // Save to external storage
        private void SaveToExternalStorage(string key, string value)
        {
            try
            {
                // VULNERABILITY: Write to external storage accessible to other apps
                string filePath = Path.Combine(ExternalStoragePath, $"{key}.data");
                File.WriteAllText(filePath, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"External write error: {ex.Message}");
            }
        }

        // Get from external storage
        private string GetFromExternalStorage(string key)
        {
            try
            {
                string filePath = Path.Combine(ExternalStoragePath, $"{key}.data");
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"External read error: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Cache Operations

        // VULNERABILITY: Cache sensitive data
        public void CacheSensitiveData(Dictionary<string, string> data)
        {
            try
            {
                // Create a cache file with multiple sensitive values
                var cacheBuilder = new StringBuilder();

                foreach (var item in data)
                {
                    cacheBuilder.AppendLine($"{item.Key}={item.Value}");
                }

                // VULNERABILITY: Write to a cache file without encryption
                string cachePath = Path.Combine(FileStoragePath, "data_cache.txt");
                File.WriteAllText(cachePath, cacheBuilder.ToString());

                // VULNERABILITY: Also write to temp directory, which might be more accessible
                string tempPath = Path.Combine(Path.GetTempPath(), "temp_cache.dat");
                File.WriteAllText(tempPath, cacheBuilder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cache write error: {ex.Message}");
            }
        }

        // VULNERABILITY: Return all cached data at once
        public Dictionary<string, string> GetAllCachedData()
        {
            var result = new Dictionary<string, string>();

            try
            {
                string cachePath = Path.Combine(FileStoragePath, "data_cache.txt");
                if (File.Exists(cachePath))
                {
                    string[] lines = File.ReadAllLines(cachePath);
                    foreach (var line in lines)
                    {
                        var parts = line.Split(new[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            result[parts[0]] = parts[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cache read error: {ex.Message}");
            }

            return result;
        }

        #endregion

        // VULNERABILITY: "Delete" function that doesn't actually remove data securely
        public void DeleteInsecurely(string key)
        {
            try
            {
                // Remove from preferences
                Preferences.Default.Remove(key);

                // "Delete" from database but actually just mark as deleted
                var record = _database.Table<SensitiveData>().FirstOrDefault(d => d.Key == key);
                if (record != null)
                {
                    // VULNERABILITY: Don't actually delete, just mark with special value
                    record.Value = "**DELETED**";
                    _database.Update(record);
                }

                // VULNERABILITY: Don't delete the file, just clear its contents
                string filePath = Path.Combine(FileStoragePath, $"{key}.txt");
                if (File.Exists(filePath))
                {
                    // VULNERABILITY: Write empty string instead of deleting
                    File.WriteAllText(filePath, string.Empty);
                }

                // VULNERABILITY: Don't even try to delete from external storage
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete error: {ex.Message}");
            }
        }

        // Dispose the database connection properly
        public void Dispose()
        {
            _database?.Close();
            _database?.Dispose();
        }
    }
}
