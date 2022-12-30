namespace FrontApp.Util
{
    public interface ICacheTelemetry
    {
        void Start();
        void End(string eventName, string key);
    }

    public class WebCacheTelemetry : ICacheTelemetry
    {
        private readonly TelemetryClient _telemetryClient;
        private Stopwatch _stopwatch;

        public WebCacheTelemetry(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void Start()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void End(string eventName, string key)
        {
            _stopwatch?.Stop();
            var dependencyTelemetry = new DependencyTelemetry();
            dependencyTelemetry.Name = eventName;
            dependencyTelemetry.Properties.Add("Key", key);
            dependencyTelemetry.Duration = TimeSpan.FromMilliseconds(_stopwatch == null?0:
                _stopwatch.ElapsedMilliseconds);
            dependencyTelemetry.Type = "CacheOperation";
            _telemetryClient.TrackDependency(dependencyTelemetry);
        }
    }


    public static class CacheTelemetryNames
    {

        public const string SetRedis = "Set_Ok";
        public const string GetRedisHit = "Get_Cache_Hit";
        public const string GetRedisMiss = "Get_Cache_Miss";
        public const string GetRedisError = "Get_Cache_Error";
        public const string SetRedisError = "Set_Error";
        public const string RedisConnectionError = "Connection_Error";
    }
}
