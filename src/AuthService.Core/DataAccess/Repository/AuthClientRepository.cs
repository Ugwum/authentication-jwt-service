using Microsoft.Extensions.Logging;
using AuthService.Core.DataAccess.Data;
using AuthService.Core.Infrastructure;
using AuthService.Core.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.DataAccess.Repository
{
    public class AuthClientRepository : Repository<AuthClient>
    {
        private ILogger<AuthClientRepository> _logger;
        private readonly ICacheProvider _cacheProvider;
        public AuthClientRepository(DBContext context,ICacheProvider cacheProvider, ILogger<AuthClientRepository> logger) : base(context)
        {
            _logger = logger;
            _cacheProvider = cacheProvider;
        }

        public async Task<AuthClient> GetAuthClientAsync(string clientId) 
        {
            try
            {
               return await new CacheExtensionDataManager<AuthClient>(_cacheProvider).GetAsync(clientId, async() =>
                {
                    return Query(c => c.secretId == clientId && c.isactive == true).SingleOrDefault();
                });

            }
            catch(Exception ex)
            {
                _logger.LogError($"An error occurred. {ex.Message}, {ex.StackTrace}");
                throw ex;
            }
        }

    }
}

