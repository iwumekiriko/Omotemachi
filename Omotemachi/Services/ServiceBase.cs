using Omotemachi.Infrastructure.Persistance.AppContext;

namespace Omotemachi.Services;

public class ServiceBase(
    AppDbContext context,
    ILogger<ServiceBase> logger
)
{
    public readonly AppDbContext _context = context;
    public readonly ILogger<ServiceBase> _logger = logger;
}
