using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Server
{
    internal class ServerControler : IDisposable
    {
        private UdpClient server;
        private const int PORT = 8888;
        private readonly SynchronizationContext ctx;
        private CancellationTokenSource cansel;

        private const int HEADER_SIZE = 24;
        private ConcurrentDictionary<Guid, Assemble> incompleteFrames = new ConcurrentDictionary<Guid, Assemble>();

        public event Action<Bitmap> FrameReady;
        public event Action<string> LogEvent;

        public ServerControler(SynchronizationContext context)
        {
            ctx = context;
        }

        public void StartListening()
        {
            if (server != null) return;

            try
            {
                server = new UdpClient(PORT);
                cansel = new CancellationTokenSource();
                Log($"Server is on {PORT}...");

                Task.Run(() => ReceiveLoopAsync(cansel.Token));
                Task.Run(() => CleanupLoopAsync(cansel.Token));
            }
            catch (Exception ex)
            {
                Log($"{ex.Message}");
            }
        }

        private async Task ReceiveLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    UdpReceiveResult result = await server.ReceiveAsync(token);
                    byte[] data = result.Buffer;

                    if (data.Length <= HEADER_SIZE) continue;
                    Guid frameId = new Guid(new ReadOnlySpan<byte>(data, 0, 16));
                    int packetIndex = BitConverter.ToInt32(data, 16);
                    int totalPackets = BitConverter.ToInt32(data, 20);

                    Assemble assembler = incompleteFrames.GetOrAdd(frameId, (id) => new Assemble(totalPackets));

                    byte[] payload = new byte[data.Length - HEADER_SIZE];
                    Array.Copy(data, HEADER_SIZE, payload, 0, payload.Length);

                    bool frameComplete = assembler.AddPacket(packetIndex, payload);

                    if (frameComplete)
                    {
                        await AssembleFrameAsync(assembler, frameId);
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex) { Log($"Ошибка приема: {ex.Message}"); }
            }
        }

        private async Task AssembleFrameAsync(Assemble assembler, Guid frameId)
        {
            try
            {
                
                await Task.Run(() =>
                {
                    // Собираем все куски
                    using (MemoryStream ms = new MemoryStream())
                    {
                        for (int i = 0; i < assembler.TotalPackets; i++)
                        {
                            if (!assembler.Packets.ContainsKey(i))
                            {
                                ctx.Post(_ => Log($"error with packet {i}"), null);
                                return;
                            }
                            ms.Write(assembler.Packets[i], 0, assembler.Packets[i].Length);
                        }

                        byte[] fullImageData = ms.ToArray();

                        // в Bitmap
                        using (MemoryStream imgStream = new MemoryStream(fullImageData))
                        {
                            Bitmap frame = new Bitmap(imgStream);
                            // клон потому что иначе у меня падает форма из за потоков
                            LogPicture((Bitmap)frame.Clone());
                        }
                    }
                }); 
            }
            catch (Exception ex)
            {
                Log(ex.Message);
            }
            finally
            {
                incompleteFrames.TryRemove(frameId, out _);
            }
        }

        private async Task CleanupLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(2000, token); // Проверяем каждые 2 сек
                if (token.IsCancellationRequested) break;

                var staleFrames = incompleteFrames
                    .Where(kvp => (DateTime.UtcNow - kvp.Value.LastPacketTime).TotalSeconds > 15) // "Мертвые" > 15 сек
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in staleFrames)
                {
                    if (incompleteFrames.TryRemove(key, out _))
                    {
                        Log($"deleted lost pack {key}.");
                    }
                }
            }
        }


        private void Log(string message)
        {
            if (ctx != null)
            {
                ctx.Post(d => LogEvent?.Invoke(message), null);
                return;
            }
            LogEvent?.Invoke(message);
        }
        private void LogPicture(Bitmap pict)
        {
            if (ctx != null)
            {
                ctx.Post(d => FrameReady?.Invoke(pict), null);
                return;
            }
            FrameReady?.Invoke(pict);
        }
        public void Dispose()
        {
            cansel?.Cancel();
            server?.Close();
        }
    }
}