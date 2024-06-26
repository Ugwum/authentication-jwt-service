﻿using Microsoft.Extensions.Logging;
using AuthService.Core.DataAccess.Data;
using AuthService.Core.Infrastructure;
using AuthService.Core.Infrastructure.Abstractions;

namespace AuthService.Core.DataAccess.Repository
{
    public class CustomUserRepository : Repository<CustomUser>
    {
        private ILogger<CustomUserRepository> _logger;
        private readonly ICacheProvider _cacheProvider;
        public CustomUserRepository(DBContext context,ICacheProvider cacheProvider, ILogger<CustomUserRepository> logger) : base(context)
        {
            _logger = logger;
            _cacheProvider = cacheProvider;
        }

        public async Task<CustomUser> GetCustomUserAsync(string username)
        {
            try
            {
                return await new CacheExtensionDataManager<CustomUser>(_cacheProvider).GetAsync(username, async () =>
                {
                    return Query(c => c.email.ToLower() == username.ToLower()).SingleOrDefault();
                });

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred. {ex.Message}, {ex.StackTrace}");
                throw ex;
            }
        }

    }
}

