using System;
using System.Net;
using System.Threading.Tasks;
using Application.Common.Interfaces;

namespace Infrastructure.Services
{
    public class WebFileLoader : IFileLoader
    {
        public async Task Download(string url, string fileName)
        {
            using var client = new WebClient();

            await client.DownloadFileTaskAsync(new Uri(url), fileName);
        }
    }
}