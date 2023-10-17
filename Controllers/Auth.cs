using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace SK_API.Controllers
{
    public class Auth
    {
        private readonly ILogger<Auth> _logger;
        private readonly IConfiguration _configuration;

        public Auth(ILogger<Auth> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public int Authenticate(string secretToken)
        {
            var secretKey = _configuration["SECRET_TOKEN"];

            if (string.IsNullOrWhiteSpace(secretToken))
            {
                return 400; // Bad Request
            }

            if (secretKey != secretToken)
            {
                return 403; // Forbidden
            }
            else
            {
                return 200; // OK
            }
        }
    }
}
