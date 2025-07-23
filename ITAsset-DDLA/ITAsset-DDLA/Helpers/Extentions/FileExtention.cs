namespace ddla.ITApplication.Helpers.Extentions;

public static class FileExtention
{
    public static string CreateImageFile(this IFormFile file, string webRootPath, string folderName)
    {
        if (!IsValidImageFile(file)) return String.Empty;
        string fileName = Guid.NewGuid().ToString() + file.FileName;
        string path = Path.Combine(webRootPath, folderName);
        using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
        {
            file.CopyTo(stream);
        }
        return fileName;
    }

    public static string CreateFile(this IFormFile file, string webRootPath, string folderName)
    {
        if (!IsValidFile(file)) return String.Empty;
        string fileName = Guid.NewGuid().ToString() + file.FileName;
        string path = Path.Combine(webRootPath, folderName);
        using (var stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
        {
            file.CopyTo(stream);
        }
        return fileName;
    }

    public static string UpdateImageFile(this IFormFile file, string webRootPath, string folderName, string oldUrl)
    {
        if (!IsValidImageFile(file)) return String.Empty;
        RemoveFile(Path.Combine(webRootPath, folderName, oldUrl));
        return file.CreateFile(webRootPath, folderName);
    }
    public static string UpdateFile(this IFormFile file, string webRootPath, string folderName, string oldUrl)
    {
        if (!IsValidFile(file)) return String.Empty;
        RemoveFile(Path.Combine(webRootPath, folderName, oldUrl));
        return file.CreateFile(webRootPath, folderName);
    }

    public static void RemoveFile(string path)
    {
        System.IO.File.Delete(path);
    }

    public static bool IsValidImageFile(IFormFile file)
    {
        if (file is null) return false;
        if (!file.ContentType.Contains("image")) return false;
        if (file.Length == 0 || file.Length > 2 * 1024 * 1024) return false;

        return true;
    }

    public static bool IsValidFile(IFormFile file)
    {
        const long maxFileSize = 5 * 1024 * 1024;

        return file != null && file.Length > 0 && file.Length <= maxFileSize;
    }

}