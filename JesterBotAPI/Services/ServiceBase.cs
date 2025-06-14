namespace DellArteAPI.Services;

public class ServiceBase(
    AppContext context,
    ILogger<ServiceBase> logger
)
{
    public readonly AppContext _context = context;
    public readonly ILogger<ServiceBase> _logger = logger;
}
