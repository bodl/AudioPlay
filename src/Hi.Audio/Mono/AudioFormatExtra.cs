#if MONOANDROID
namespace Hi.Audio
{
    using Android.Media;
    using System;
    
    public static class AudioFormatExtra
    {
        public static ChannelIn GetChannelIn(this Hi.Audio.AudioFormat audioFormat)
        {
            switch (audioFormat.Channels)
            {
                case 1:
                    return ChannelIn.Mono;
                default:
                    return ChannelIn.Stereo;
            }
        }
        public static ChannelOut GetChannelOut(this Hi.Audio.AudioFormat audioFormat)
        {
            switch (audioFormat.Channels)
            {
                case 1:
                    return ChannelOut.Mono;
                default:
                    return ChannelOut.Stereo;
            }
        }

        public static Android.Media.Encoding GetEncoding(this Hi.Audio.AudioFormat audioFormat)
        {
            switch(audioFormat.Encoding)
            {
                case AudioFormatEncoding.Pcm:
                    if (audioFormat.BitsPerSample <= 8)
                    {
                        return Encoding.Pcm8bit;
                    }
                    else
                    {
                        return Encoding.Pcm16bit;
                    }
                default:
                    return Encoding.PcmFloat;
            }
        }

        public static int GetBufferSizeInBytes(this Hi.Audio.AudioFormat audioFormat)
        {
            return audioFormat.BufferMilliseconds * audioFormat.ChunkLength;
        }
        public static int GetDesiredLatencySizeInBytes(this Hi.Audio.AudioFormat audioFormat)
        {
            return audioFormat.DesiredLatency * audioFormat.ChunkLength;
        }
    }
}
#endif