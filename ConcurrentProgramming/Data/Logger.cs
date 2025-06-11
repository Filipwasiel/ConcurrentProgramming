using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace FW_LJ_CP.Data
{
    public class CollisionLogger : IDisposable
    {
        private readonly BlockingCollection<string> _logBuffer;
        private readonly Thread _consumerThread;
        private readonly string _filePath;

        public CollisionLogger(string? customFilePath = null, int bufferSize = 1024)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = customFilePath ?? Path.Combine(GetProjectDataPath(), $"collision_log_{timestamp}.txt");
            _filePath = fileName;

            _logBuffer = new BlockingCollection<string>(bufferSize);
            _consumerThread = new Thread(Consume) { IsBackground = true };
            _consumerThread.Start();
        }

        public void LogBall2BallCollision(
            double x1, double y1, double vx1, double vy1, int ballId1,
            double x2, double y2, double vx2, double vy2, int ballId2)
        {
            string message = $"Wykryto kolizje kula–kula: " +
                             $"Ball1 (ID={ballId1}, X={x1:F2}, Y={y1:F2}, Vx={vx1:F2}, Vy={vy1:F2}) vs " +
                             $"Ball2 (ID={ballId2}, X={x2:F2}, Y={y2:F2}, Vx={vx2:F2}, Vy={vy2:F2})";
            EnqueueLog(message);
        }

        public void LogBall2WallCollision(
            double x, double y, double vx, double vy, int ballId,
            double radius, double tableWidth, double tableHeight)
        {
            string wallDirection;

            if (x - radius <= 0) wallDirection = "ściana lewa";
            else if (x + radius >= tableWidth - radius*2) wallDirection = "ściana prawa";
            else if (y - radius <= 0) wallDirection = "ściana górna";
            else if (y + radius >= tableHeight - radius*2) wallDirection = "ściana dolna";
            else wallDirection = "nieznana";

            string message = $"Wykryto kolizje kuli ze {wallDirection}: Ball (ID={ballId}, X={x:F2}, Y={y:F2}, prędkość x={vx:F2}, prędkość y={vy:F2})";
            EnqueueLog(message);
        }

        private void EnqueueLog(string message)
        {
            string logEntry = $"[{DateTime.Now:HH:mm:ss}] - {message}";
            try
            {
                _logBuffer.Add(logEntry);
            }
            catch (InvalidOperationException)
            {

            }
        }

        private void Consume()
        {
            using var writer = new StreamWriter(_filePath, append: false)
            {
                AutoFlush = true
            };

            foreach (var log in _logBuffer.GetConsumingEnumerable())
            {
                writer.WriteLine(log);
            }
        }

        private string GetProjectDataPath()
        {
            string? baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string? projectDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
            string dataPath = Path.Combine(projectDir, "Data");

            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            return dataPath;
        }

        public void Stop()
        {
            _logBuffer.CompleteAdding();
            _consumerThread.Join();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
