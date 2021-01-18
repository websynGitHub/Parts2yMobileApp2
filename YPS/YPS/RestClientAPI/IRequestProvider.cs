using System.Threading.Tasks;

namespace YPS.RestClientAPI
{
    public interface IRequestProvider
    {
        Task<TResult> PostAsync<TResult>(string url);
        Task<TResult> PostAsync<TResult>(string url, TResult data);
    }
}
