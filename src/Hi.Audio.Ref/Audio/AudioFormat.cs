namespace Hi.Audio
{
    using System;

    public enum AudioFormatEncoding
    {
        Pcm = 1,
        PcmFloat = 3,
    }

    public class AudioFormat
    {
        /// <summary>
        /// 音频格式
        /// </summary>
        public virtual AudioFormatEncoding Encoding { get; }

        /// <summary>
        /// 采样率
        /// </summary>
        /// <remarks>
        /// 1KHz = 1000;
        /// 11025\22050\32000\44100\48000\64000\88200\96000\176400\192000
        /// </remarks>
        public virtual int SampleRate { get; }

        /// <summary>
        /// 通道数
        /// </summary>
        public int Channels { get; }

        /// <summary>
        /// 采样位深 8\16\32
        /// </summary>
        /// <remarks>
        /// 8\16\24\32 bit
        /// </remarks>
        public int BitsPerSample { get; }

        /// <summary>
        /// 录制缓冲ms
        /// </summary>
        /// <remarks>
        /// 默认20ms; 1s = 1000ms
        /// </remarks>
        public int BufferMilliseconds { get; set; } = 20;

        /// <summary>
        /// 播放延迟ms
        /// </summary>
        /// <remarks>
        /// 默认100ms; 1s = 1000ms
        /// </remarks>
        public int DesiredLatency { get; set; } = 100;

        /// <summary>
        /// 块对齐
        /// </summary>
        public short BlockAlign => (short)( Channels * (BitsPerSample / 8));

        /// <summary>
        /// 每秒比特
        /// </summary>
        public int AverageBytesPerSecond => SampleRate * BlockAlign;

        /// <summary>
        /// 块大小
        /// </summary>
        public int ChunkLength => AverageBytesPerSecond / 1000;

        public AudioFormat(int sampleRate, int channels = 1, int bitsPerSample = 16)
        {
            Encoding = AudioFormatEncoding.Pcm;
            SampleRate = sampleRate;
            Channels = channels;
            BitsPerSample = bitsPerSample;
        }

        public AudioFormat(AudioFormatEncoding formatEncoding, int sampleRate, int channels = 1, int bitsPerSample = 16)
        {
            Encoding = formatEncoding;
            SampleRate = sampleRate;
            Channels = channels;
            BitsPerSample = bitsPerSample;
        }
    }
}
