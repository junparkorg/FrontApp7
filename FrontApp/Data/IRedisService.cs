namespace FrontApp.Data
{
    public interface IRedisService
    {
        Task<string> GetAsync(string key);

        Task<bool> SetAsync(string key, string data, TimeSpan expirationTime);
    }

    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public RedisService(IDatabase db) => _database = db;


        public async Task<string> GetAsync(string key)
        {
            string result = string.Empty;

            var retryPolicy = GetRetryPolicy();

            if (_database != null && _database.IsConnected(key))
            {
                result = await retryPolicy.ExecuteAsync(async () => await _database.StringGetAsync(key));
            }
            return result;
        }

        private AsyncRetryPolicy GetRetryPolicy()
        {
            var retryPolicy = Policy.Handle<StackExchange.Redis.RedisTimeoutException>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                },
                (ex, ts) =>
                {
                    Debug.WriteLine(ex.Message);
                });
            return retryPolicy;
        }



        public async Task<bool> SetAsync(string key, string data, TimeSpan expirationTime)
        {
            bool result = false;

            var retryPolicy = GetRetryPolicy();


            if (_database != null && _database.IsConnected(key))
            {
                result = await retryPolicy.ExecuteAsync(async () => await _database.StringSetAsync(key, data, expirationTime));
            }

            return result;
        }
    }
}
