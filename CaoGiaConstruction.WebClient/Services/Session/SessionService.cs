using CaoGiaConstruction.Utilities;
using CaoGiaConstruction.WebClient.Installers;

namespace CaoGiaConstruction.WebClient.Services.Session
{
    public interface ISessionService
    {
        void SetSessionValue(string key, object value);

        T GetSessionValue<T>(string key);
    }

    public class SessionService : ISessionService, ITransientService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetSessionValue(string key, object value)
        {
            _httpContextAccessor.HttpContext.Session.SetString(key, value.ToJsonString());
        }

        public T GetSessionValue<T>(string key)
        {
            var value = _httpContextAccessor.HttpContext.Session.GetString(key);
            if (value != null)
            {
                return value.ToJsonObject<T>();
            }
            return default(T);
        }
    }
}