using System.Threading.Tasks;
using Application.Common.Models;

namespace Application.Common.Interfaces
{
    public interface IJobResponseService
    {
        Task Submit(string token, JobResponsePayload payload);
    }
}