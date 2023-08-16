#if NET6_0 || NET6_0_WINDOWS7_0 || NETCOREAPP3_1
namespace Hi.Audio
{
    using System;
    using NAudio.Wave;
    using NAudio.Wave.SampleProviders;
    public class AudioPlayer : IAudioPlayer
    {
        IWavePlayer wavePlayer;
        AudioStream audioStream;
        public AudioFormat AudioFormat { get; }
        public float Volume { get => wavePlayer.Volume;  set => wavePlayer.Volume = value; }
        public int DeviceNumber { get; }
        public AudioPlayer(AudioFormat audioFormat, int deviceNumber = 0)
        {
            this.AudioFormat = audioFormat;
            this.DeviceNumber = deviceNumber;
            wavePlayer = new WaveOutEvent() {
                DeviceNumber = DeviceNumber,
                DesiredLatency = AudioFormat.DesiredLatency //时延
            };
            audioStream = new AudioStream(audioFormat.ToWaveFormat());
            var provider = CreateAudioStream();
            wavePlayer.Init(provider);
        }

        public ISampleProvider CreateAudioStream()
        {
            //audioStream = new AudioStream(new WaveFormat(11051, 1));
            var channel = new SampleChannel(audioStream, true);
            var meter = new MeteringSampleProvider(channel);
            return meter;
        }

        public void Dispose()
        {
            wavePlayer.Dispose();
        }

        public void Play()
        {
            if (wavePlayer.PlaybackState == PlaybackState.Playing) return;
            wavePlayer.Play();
        }

        public void Pause()
        {
            if (wavePlayer.PlaybackState != PlaybackState.Playing) return;
            wavePlayer.Pause();
        }

        public void Stop()
        {
            if (wavePlayer.PlaybackState == PlaybackState.Stopped) return;
            wavePlayer.Stop();
        }

        public void Add(byte[] buffer)
        {
            this.audioStream.Add(buffer);
        }
    }
}
#endif