using IcdMapper_Excel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcdMapper_Excel.Services.Interfaces
{
    public interface IMappingProfileService
    {
        List<MappingProfile> LoadAll();

        void Save(MappingProfile profile);

        void Delete(string profileName);
    }
}