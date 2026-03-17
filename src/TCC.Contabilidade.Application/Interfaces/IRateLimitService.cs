using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC.Contabilidade.Application.Interfaces
{
    public interface IRateLimitService
    {
        Task<bool> IsRequestAllowedAsync(string key, int limit, TimeSpan window);
    }
}
