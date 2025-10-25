namespace Server
{
    internal class Assemble
    {
        public int TotalPackets { get; }
        public int PacketsReceived { get; private set; }
        public Dictionary<int, byte[]> Packets { get; }
        public DateTime LastPacketTime { get; set; }

        public Assemble(int total)
        {
            TotalPackets = total;
            PacketsReceived = 0;
            Packets = new Dictionary<int, byte[]>();
            LastPacketTime = DateTime.UtcNow;
        }

        public bool AddPacket(int index, byte[] data)
        {
            if (!Packets.ContainsKey(index))
            {
                Packets[index] = data;
                PacketsReceived++;
            }
            LastPacketTime = DateTime.UtcNow;
            return PacketsReceived == TotalPackets;
        }
    }
}
