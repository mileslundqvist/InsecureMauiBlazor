using Android.Content;
using Android.Database;
using Android.OS;
using Android.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uri = Android.Net.Uri;

namespace InsecureMauiBlazor.Platforms.Android
{
    [ContentProvider(new string[] { "com.companyname.insecuremauiblazor.sensitiveprovider" }, Exported = true)]
    public class SensitiveDataProvider : ContentProvider
    {
        // In-memory store for demonstration purposes
        private static Dictionary<string, string> sensitiveData = new Dictionary<string, string>()
        {
            { "sample_username", "admin" },
            { "sample_password", "password123" },
            { "sample_apikey", "sk_live_abcdefghijklmnopqrstuvwxyz" },
            { "sample_token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIn0.Gfx6VO9tcxwk6xqP-5T-PZc-JFoJG3n6JxNi9-yV6ks" }
        };

        // VULNERABILITY: No permission checks
        public override bool OnCreate()
        {
            // VULNERABILITY: Log when provider is created
            Console.WriteLine("SensitiveDataProvider created");
            return true;
        }

        // VULNERABILITY: Insecure query method with no authentication/authorization
        public override ICursor Query(Uri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            // VULNERABILITY: Log all query attempts, potentially leaking sensitive operations
            Console.WriteLine($"Query received for: {uri}");

            MatrixCursor cursor = new MatrixCursor(new string[] { "key", "value" });

            // VULNERABILITY: No validation of the query parameters
            if (string.IsNullOrEmpty(selection))
            {
                // Return all data if no selection criteria
                foreach (var item in sensitiveData)
                {
                    cursor.AddRow(new Java.Lang.Object[] {
                            new Java.Lang.String(item.Key),
                            new Java.Lang.String(item.Value)
                        });
                }
            }
            else
            {
                // VULNERABILITY: No sanitization of selection string
                // If selection matches a key, return it
                if (sensitiveData.ContainsKey(selection))
                {
                    cursor.AddRow(new Java.Lang.Object[] {
                            new Java.Lang.String(selection),
                            new Java.Lang.String(sensitiveData[selection])
                        });
                }
            }

            return cursor;
        }

        // VULNERABILITY: Allows insertion of new data with no access controls
        public override Uri Insert(Uri uri, ContentValues values)
        {
            // VULNERABILITY: Log all insertion attempts
            Console.WriteLine($"Insert received for: {uri}");

            string key = values.GetAsString("key");
            string value = values.GetAsString("value");

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                // VULNERABILITY: No validation of data being inserted
                sensitiveData[key] = value;
                Console.WriteLine($"Inserted: {key}={value}");
            }

            // VULNERABILITY: Broadcast that data has changed, potentially leaking operation
            Context.ContentResolver.NotifyChange(uri, null);

            return Uri.Parse($"{uri}/{key}");
        }

        // VULNERABILITY: Allows deletion of data with no access controls
        public override int Delete(Uri uri, string selection, string[] selectionArgs)
        {
            // VULNERABILITY: Log all deletion attempts
            Console.WriteLine($"Delete received for: {uri}, selection: {selection}");

            int count = 0;

            // VULNERABILITY: No validation of deletion criteria
            if (string.IsNullOrEmpty(selection))
            {
                // Delete all if no selection criteria
                count = sensitiveData.Count;
                sensitiveData.Clear();
            }
            else if (sensitiveData.ContainsKey(selection))
            {
                sensitiveData.Remove(selection);
                count = 1;
            }

            // VULNERABILITY: Broadcast that data has changed
            Context.ContentResolver.NotifyChange(uri, null);

            return count;
        }

        // VULNERABILITY: Allows data updates with no access controls
        public override int Update(Uri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            // VULNERABILITY: Log all update attempts
            Console.WriteLine($"Update received for: {uri}, selection: {selection}");

            int count = 0;
            string value = values.GetAsString("value");

            if (!string.IsNullOrEmpty(value))
            {
                // VULNERABILITY: No validation of update criteria
                if (string.IsNullOrEmpty(selection))
                {
                    // Update all if no selection criteria
                    foreach (var key in sensitiveData.Keys.ToList())
                    {
                        sensitiveData[key] = value;
                        count++;
                    }
                }
                else if (sensitiveData.ContainsKey(selection))
                {
                    sensitiveData[selection] = value;
                    count = 1;
                }
            }

            // VULNERABILITY: Broadcast that data has changed
            Context.ContentResolver.NotifyChange(uri, null);

            return count;
        }

        // VULNERABILITY: Insecure implementation of getType
        public override string GetType(Uri uri)
        {
            return "vnd.android.cursor.dir/vnd.insecuremauiblazor.data";
        }

        // VULNERABILITY: Create a custom file access mechanism that bypasses permissions
        // This method allows reading any file the app has access to
        public override ParcelFileDescriptor OpenFile(Uri uri, string mode)
        {
            try
            {
                // VULNERABILITY: Log file access attempts
                Console.WriteLine($"File access attempt: {uri.Path}");

                // VULNERABILITY: Extract file path with no validation
                string filePath = uri.Path.Replace("/file/", "");

                // VULNERABILITY: No path traversal protection
                Java.IO.File file = new Java.IO.File(filePath);

                // VULNERABILITY: Open the file with the requested mode with no validation
                ParcelFileMode fileMode = ParcelFileMode.ReadOnly;

                // Determine the correct file mode based on the requested mode string
                if (mode.Contains("w"))
                {
                    fileMode = ParcelFileMode.WriteOnly;

                    if (mode.Contains("r"))
                    {
                        fileMode = ParcelFileMode.ReadWrite;
                    }
                }

                return ParcelFileDescriptor.Open(file, fileMode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening file: {ex.Message}");
                throw;
            }
        }
    }
}