using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IFileLoader
    {
        Task Download(string url, string fileName);
    }
}