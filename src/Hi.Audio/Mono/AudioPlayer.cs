#if MONOANDROID
namespace Hi.Audio
{
    using Android.Media;
    using System;
    public class AudioPlayer : IAudioPlayer
    {
        public AudioFormat AudioFormat { get; }
        public float Volume { get { if (AudioTrack != null) return AudioTrack.MaxVolume; else return 0; } set => AudioTrack?.SetVolume(value); }
        // 使用NULL会抛出空引用异常
        public AudioTrack? AudioTrack;
        public AudioPlayer(AudioFormat audioFormat)
        {
            AudioFormat = audioFormat;
        }

        public async void Add(byte[] buffer)
        {
            if (AudioTrack == null) return;
            await AudioTrack.WriteAsync(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            if (AudioTrack != null)
            {
                if (AudioTrack.State == AudioTrackState.Initialized)
                {
                    AudioTrack.Release();
                }
                AudioTrack.Dispose();
            }
        }

        public void Pause()
        {
            if (AudioTrack == null) return;
            else if (AudioTrack.PlayState != PlayState.Playing) return;
            AudioTrack.Pause();
        }

        public void Play()
        {
            if (AudioTrack == null)
            {
                AudioTrack = createTrack();
            }
            if (AudioTrack == null) return;
            else if (AudioTrack.PlayState == PlayState.Playing) return;
            AudioTrack.Play();
        }

        public void Stop()
        {
            if (AudioTrack == null) return;
            if (AudioTrack.PlayState == PlayState.Stopped) return;
            // 没有初始化会抛出 未初始化异常.
            if (AudioTrack.State != AudioTrackState.Initialized) return;
            AudioTrack.Stop();
        }

        /// <summary>
        /// 用法
        /// </summary>
        public virtual AudioUsageKind AudioUsageKind { get; set; } = AudioUsageKind.Media;
        /// <summary>
        /// 标识
        /// </summary>
        public virtual AudioFlags AudioFlags { get; set; } = AudioFlags.HwAvSync;
        /// <summary>
        /// 流类型
        /// </summary>
        public virtual Android.Media.Stream AudioStream { get; set; } = Android.Media.Stream.Music;
        /// <summary>
        /// 内容类型
        /// </summary>
        public virtual AudioContentType AudioContentType { get; set; } = AudioContentType.Music;
        /// <summary>
        /// 性能模式
        /// </summary>
        public virtual AudioTrackPerformanceMode AudioTrackPerformanceMode { get; set; } = AudioTrackPerformanceMode.None;
        /// <summary>
        /// 传输模式
        /// </summary>
        public virtual AudioTrackMode AudioTrackMode { get; set; } = AudioTrackMode.Stream;
        private AudioTrack? createTrack()
        {
            //AudioTrack = new AudioTrack(Stream.Music, audioFormat.SampleRate, audioFormat.GetChannelOut(), audioFormat.GetEncoding(), audioFormat.DesiredLatency, AudioTrackMode.Stream); ;
            var audio_attr = new AudioAttributes.Builder();
            AudioAttributes? attr = null;
            if (audio_attr != null) audio_attr.SetUsage(AudioUsageKind);
            if (audio_attr != null) audio_attr.SetFlags(AudioFlags);
            if (audio_attr != null) audio_attr.SetLegacyStreamType(AudioStream);
            if (audio_attr != null) audio_attr.SetContentType(AudioContentType);
            if (audio_attr != null) attr = audio_attr.Build();
            var audio_format = new Android.Media.AudioFormat.Builder();
            Android.Media.AudioFormat? format = null;
            if (audio_format != null) audio_format.SetChannelMask(AudioFormat.GetChannelOut());
            if (audio_format != null) audio_format.SetEncoding(AudioFormat.GetEncoding());
            if (audio_format != null) audio_format.SetSampleRate(AudioFormat.SampleRate);
            if (audio_format != null) format = audio_format.Build();

            var track = new AudioTrack.Builder();
            if (audio_attr != null) { track.SetAudioAttributes(attr); }
            if (format != null) { track.SetAudioFormat(format); }
            if (format != null) { track.SetPerformanceMode(AudioTrackPerformanceMode.LowLatency); }
            if (track != null) track.SetBufferSizeInBytes(AudioFormat.GetDesiredLatencySizeInBytes());
            if (track != null) track.SetTransferMode(AudioTrackMode);
            return track?.Build();
        }
    }
}
#endif