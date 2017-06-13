using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StarlightCallCenter.Controllers
{
    public static class FFmpegServer
    {
        private static string FfmpegPath = Path.Combine(
            Path.GetDirectoryName(typeof(Program).GetTypeInfo().Assembly.Location),
            @"ffmpeg\ffmpeg.exe");

        private static string Run(string arguments)
        {
            var ps = Process.Start(new ProcessStartInfo(FfmpegPath, arguments)
            {
                RedirectStandardOutput = true,
            });
            return ps.StandardOutput.ReadToEnd();
        }

        public static Stream ChangeWavRate(Stream fileIn)
        {
            var tempFileName = Path.GetTempFileName();
            using (var file = File.OpenWrite(tempFileName))
            {
                fileIn.CopyTo(file);
            }

            var tempOutFileName = tempFileName + ".wav";
            var info = Run($"-i \"{tempFileName}\" -ar 16000 \"{tempOutFileName}\"");

            File.Delete(tempFileName);
            var bytes = File.ReadAllBytes(tempOutFileName);
            File.Delete(tempOutFileName);

            return new MemoryStream(bytes);
        }
    }
}
