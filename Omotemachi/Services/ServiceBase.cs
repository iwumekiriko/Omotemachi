using Omotemachi.Infrastructure.Persistance.AppContext;
using Omotemachi.Tools;

namespace Omotemachi.Services;

public class ServiceBase<T>
{
    public readonly AppDbContext _context;
    public readonly ILogger<T> _logger;

    protected ServiceBase(AppDbContext context, ILogger<T> logger)
    {
        _context = context;
        _logger = logger;

        _logger.LogDebug("{serviceName} initialized", typeof(T).Name);
    }
}
