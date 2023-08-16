namespace Hi.Audio.Ref
{
    using System;
    using System.IO;
    using CUETools.Codecs;
    using CUETools.Codecs.FLAKE;

    /// <summary>
    /// Flac编解码器
    /// </summary>
    /// <see cref="https://github.com/EntangledBits/CUETools.Codecs"/>
    /// <remarks>
    /// GPL License
    /// </remarks>
    public class FlacCodec : IAudioCodec, IDisposable
    {
        AudioFormat AudioFormat { get; }
        FlakeWriter FlakeWriter { get; }
        FlakeReader FlakeReader { get; }
        MemoryStream stream;
        AudioPCMConfig PCMConfig { get; }
        public FlacCodec(AudioFormat audioFormat)
        {
            AudioFormat = audioFormat;
            stream = new MemoryStream();
            PCMConfig = new AudioPCMConfig(AudioFormat.BitsPerSample, AudioFormat.Channels, AudioFormat.SampleRate);

            FlakeWriter = new FlakeWriter(null, stream, PCMConfig);
            FlakeReader = new FlakeReader(PCMConfig);
            FlakeWriter.Init_Header();
        }

        public byte[] Encode(AudioChunk chunk)
        {
            var bytes = chunk.GetDataAsBytes();
            var buff = new AudioBuffer(PCMConfig, bytes, bytes.Length / AudioFormat.BlockAlign);
            var pos = stream.Position;
            FlakeWriter.Write(buff);
            if (stream.Length > pos)
            {
                var buffer = new byte[stream.Length - pos];
                stream.Position = pos;
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
            return new byte[]{ };
        }

        public AudioChunk Decode(byte[] inputPacket)
        {
            FlakeReader.Clear();
            var length = FlakeReader.DecodeFrame(inputPacket, 0, inputPacket.Length);
            var buff = new AudioBuffer(PCMConfig, length);
            length = FlakeReader.Read(buff, length);
            var buffer = new byte[length * AudioFormat.BlockAlign];
            Array.Copy(buff.Bytes, 0, buffer, 0, buffer.Length);

            var chunk = new AudioChunk(buffer, PCMConfig.SampleRate);
            return chunk;
        }

        public void Dispose()
        {
            if (stream != null) { stream.Dispose(); }
            if (FlakeWriter != null) { FlakeWriter.Dispose(); }
        }
    }
}
