using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Utils
{
    public interface IGuard
    {
        void Against(bool condition, string errorMsg, ILogger logger = null, bool throws = true);

        void Against(bool condition, string errorCode, string errorMsg, ILogger logger = null, bool throws = true);

        void Against<T>(bool condition, string errorMsg, ILogger logger = null, bool throws = true) where T : Exception, new();
    }

    public class Guard : IGuard
    {
        private readonly ILogger<Guard> _logger;

        public Guard(ILogger<Guard> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void Against(bool condition, string errorMsg, ILogger logger = null, bool throws = true)
        {
            if (condition)
            {
                var ex = new Exception(errorMsg);
                (_logger ?? logger).LogError(ex.Message, ex);
                if (throws)
                {
                    throw ex;
                }
            }
        }

        public void Against(bool condition, string errorCode, string errorMsg, ILogger logger = null, bool throws = true)
        {
            if (condition)
            {
                var ex = new CustomException(errorCode, errorMsg);
                (_logger ?? logger).LogError(ex.Message, ex);
                if (throws)
                {
                    throw ex;
                }
            }
        }

        public void Against<T>(bool condition, string errorMsg, ILogger logger = null, bool throws = true) where T : Exception, new()
        {
            if (condition)
            {
                var ex = (T)Activator.CreateInstance(typeof(T), errorMsg);
                (_logger ?? logger).LogError(ex.Message, ex);
                if (throws)
                {
                    throw ex;
                }
            }
        }
    }
}
