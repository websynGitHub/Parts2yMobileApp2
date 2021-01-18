using System.Threading.Tasks;

namespace YPS.Helpers
{
    public interface IRootDetection
    {
        Task<bool> CheckIfRooted();
    }
}
