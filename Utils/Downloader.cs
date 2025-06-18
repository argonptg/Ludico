using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace LudicoGTK.Utils;

public class Downloader
{
    public static async Task DownloadFileAsync(string url, string destination, IProgress<double> progress = null)
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient
            .GetAsync(url, HttpCompletionOption.ResponseContentRead);

        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
        var canReportProgress = totalBytes != -1 && progress != null;

        using var contentStream = await response.Content.ReadAsStreamAsync();
        using var fileStream = new FileStream(
            destination,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            8192,
            true
        );

        var buffer = new byte[8192];
        long totalRead = 0;
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
            totalRead += bytesRead;

            if (canReportProgress)
                progress.Report((double)totalRead / totalBytes * 100);
        }
    }
}
