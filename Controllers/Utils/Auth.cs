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

        public bool Authenticate(string secretToken)
        {
            var secretKey = _configuration["SECRET_TOKEN"];

            if (secretKey != secretToken)
            {
                return false; // Forbidden
            }
            else
            {
                return true; // OK
            }
        }
    }
}
