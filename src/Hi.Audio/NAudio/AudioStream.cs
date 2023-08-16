#if NET6_0 || NET6_0_WINDOWS7_0 || NETCOREAPP3_1
namespace Hi.Audio
{
    using System;
    using System.IO;
    using NAudio.Wave;

    public class AudioStream : WaveStream
    {
        private WaveStream sr;
        private MemoryStream ms;
        private WaveFormat waveFormat;
        public AudioStream(WaveFormat waveFormat)
        {
            ms = new MemoryStream();
            this.waveFormat = waveFormat;
            sr = new RawSourceWaveStream(ms, waveFormat);
        }

        public override WaveFormat WaveFormat => this.waveFormat;

        public override long Length => sr.Length;

        public override long Position { get => sr.Position; set => sr.Position = value; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return sr.Read(buffer, offset, count);
        }

        public void Add(byte[] buffer)
        {
            var pos = ms.Position;
            ms.Position = ms.Length;
            ms.Write(buffer);
            ms.Position = pos;
            sr.Flush();
        }
    }
}
#endif
