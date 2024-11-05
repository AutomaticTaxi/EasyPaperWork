namespace EasyPaperWork.Services
{
    public interface IFileSavePickerService
    {
        Task<string> SaveFileAsync(byte[] fileBytes, string suggestedFileName, string fileType);
    }
}
