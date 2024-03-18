using System;
using System.Threading.Tasks;

namespace Weather.Api.Services;

public interface IRateLimiterService
{
    public Task<bool> IsRateLimited(string client, TimeSpan limit);
}
