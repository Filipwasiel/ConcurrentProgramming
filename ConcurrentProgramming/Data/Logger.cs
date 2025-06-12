using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace FW_LJ_CP.Data
{
    internal class CollisionLogger : IDisposable
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

        public void LogBall2BallCollision(Ball ball1, Ball ball2)
        {
            string message = $"Wykryto kolizje kula–kula: " +
                             $"Ball1 (ID={ball1.ballId}, X={ball1.Position.x:F2}, Y={ball1.Position.y:F2}, Vx={ball1.Velocity.x:F2}, Vy={ball1.Velocity.y:F2}) vs " +
                             $"Ball2 (ID={ball2.ballId}, X={ball2.Position.x:F2}, Y={ball2.Position.y:F2}, Vx={ball2.Velocity.x:F2}, Vy={ball2.Velocity.y:F2})";
            EnqueueLog(message);
        }

        public void LogBall2WallCollision(Ball ball, double tableWidth, double tableHeight)
        {
            string wallDirection;

            if (ball.Position.x - ball.Diameter/2 <= 0) wallDirection = "ściana lewa";
            else if (ball.Position.x + ball.Diameter/2 >= tableWidth - ball.Diameter) wallDirection = "ściana prawa";
            else if (ball.Position.y - ball.Diameter <= 0) wallDirection = "ściana górna";
            else if (ball.Position.y + ball.Diameter >= tableHeight - ball.Diameter) wallDirection = "ściana dolna";
            else wallDirection = "nieznana";

            string message = $"Wykryto kolizje kuli ze {wallDirection}: Ball (ID={ball.ballId}, X={ball.Position.x:F2}, Y={ball.Position.y:F2}, prędkość x={ball.Velocity.x:F2}, prędkość y={ball.Velocity.y:F2})";
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
