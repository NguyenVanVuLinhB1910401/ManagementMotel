namespace ManagementMotel.UtilityServices
{
    public interface IFileService
    {
        Task<bool> uploadFile(IFormFile file);
    }
}
