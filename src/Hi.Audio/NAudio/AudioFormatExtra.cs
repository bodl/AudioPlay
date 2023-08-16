#if NET6_0 || NET6_0_WINDOWS7_0 || NETCOREAPP3_1
namespace Hi.Audio
{
    using System;
    using NAudio.Wave;

    public static class AudioFormatExtra
    {
        public static WaveFormat ToWaveFormat(this AudioFormat audioFormat)
        {
            var waveFormat = WaveFormat.CreateCustomFormat(ToWaveFormatEncoding(audioFormat.Encoding), audioFormat.SampleRate, audioFormat.Channels, audioFormat.AverageBytesPerSecond, audioFormat.BlockAlign, audioFormat.BitsPerSample);
            return waveFormat;
        }

        public static WaveFormatEncoding ToWaveFormatEncoding(this AudioFormatEncoding audioFormatEncoding)
        {
            switch (audioFormatEncoding)
            {
                case AudioFormatEncoding.PcmFloat:
                    return WaveFormatEncoding.IeeeFloat;
                case AudioFormatEncoding.Pcm:
                default:
                    return WaveFormatEncoding.Pcm;
            }
        }
    }
}
#endif
