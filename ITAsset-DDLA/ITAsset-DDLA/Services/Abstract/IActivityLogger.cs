namespace ITAsset_DDLA.Services.Abstract;

public interface IActivityLogger
{
    Task LogAsync(string userFullName, string action);
}
