using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Services
{
    public interface IFileSavePickerService
    {
        Task<string> SaveFileAsync(byte[] fileBytes, string suggestedFileName, string fileType);
    }
}
