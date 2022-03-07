
using System.Threading.Tasks;
using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IJobService
    {
        Task EnqueueJobAsync(EnqueueJobModel model);
    }
}