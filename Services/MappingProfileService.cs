using IcdMapper_Excel.Models;
using IcdMapper_Excel.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IcdMapper_Excel.Services
{
    public class MappingProfileService : IMappingProfileService
    {
        private static readonly JsonSerializerOptions _opts = new() { WriteIndented = true };

        private readonly string _dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "IcdMapper_Excel",
            "MappingProfiles"
        );

        public MappingProfileService() => Directory.CreateDirectory(_dir);

        public List<MappingProfile> LoadAll()
        {
            var result = new List<MappingProfile>();

            foreach (var file in Directory.GetFiles(_dir, "*.json"))
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var profile = JsonSerializer.Deserialize<MappingProfile>(json);
                    if (profile is not null)
                    {
                        result.Add(profile);
                    }
                }
                catch { }
            }
            return result.OrderBy(p => p.ProfileName).ToList();
        }

        public void Save(MappingProfile profile)
        {
            profile.LastModified = DateTime.Now;
            var path = ProfilePath(profile.ProfileName);
            File.WriteAllText(path, JsonSerializer.Serialize(profile, _opts));
        }

        public void Delete(string profileName)
        {
            var path = ProfilePath(profileName);
            if (File.Exists(path)) File.Delete(path);
        }

        private string ProfilePath(string name)
            => Path.Combine(_dir, $"{SanitizeName(name)}.json");

        private static string SanitizeName(string name)
            => string.Concat(name.Split(Path.GetInvalidFileNameChars()));
    }
}