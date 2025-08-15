using ddla.ITApplication.Database;
using ITAsset_DDLA.Database.Models.DomainModels;
using ITAsset_DDLA.Services.Abstract;

namespace ITAsset_DDLA.Services.Concrete;

public class ActivityLogger : IActivityLogger
{
    private readonly ddlaAppDBContext _context;
    public ActivityLogger(ddlaAppDBContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string userFullName, string action)
    {
        var log = new ActivityLog
        {
            UserFullName = userFullName,
            Action = action,
            CreatedAt = DateTime.Now
        };
        _context.ActivityLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}