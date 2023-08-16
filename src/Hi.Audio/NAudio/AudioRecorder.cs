#if NET6_0 || NET6_0_WINDOWS7_0 || NETCOREAPP3_1
namespace Hi.Audio
{
    using System;
    using System.Collections.Generic;
    using NAudio.Wave;
    using NetPs.Socket;

    public class AudioRecorder : IAudioRecorder
    {
        private long time_readed = 0;
        public QueueStream stream;
        public IWaveIn waveIn;

        public event EventHandler? DataAvailable;
        public event EventHandler? RecordingStopped;

        public AudioFormat AudioFormat { get; }
        public int DeviceNumber { get; }
        public AudioRecorder(AudioFormat audioFormat, int deviceNumber = 0)
        {
            AudioFormat = audioFormat;
            DeviceNumber = deviceNumber;
            stream = new QueueStream();
            waveIn = new WaveInEvent() { DeviceNumber = deviceNumber, BufferMilliseconds = audioFormat.BufferMilliseconds };
            waveIn.WaveFormat = audioFormat.ToWaveFormat();
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.RecordingStopped += WaveIn_RecordingStopped;
        }
        public long TotalTime => (stream.Length + time_readed) / AudioFormat.ChunkLength;
        public bool CanReadChunk => stream.Length >= AudioFormat.ChunkLength;

        public bool Recording { get; set; }

        public void Start()
        {
            if (Recording) return;
            Recording = true;
            stream.Clear();
            waveIn.StartRecording();
        }

        public void Stop()
        {
            if (!Recording) return;
            Recording = false;
            waveIn?.StopRecording();
        }

        private void WaveIn_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            Recording = false;
            RecordingStopped?.Invoke(this, e);
        }

        private void WaveIn_DataAvailable(object? sender, WaveInEventArgs e)
        {
            stream.Enqueue(e.Buffer, 0, e.BytesRecorded);
            DataAvailable?.Invoke(this, e);
        }
        public AudioChunk GetNextChunk()
        {
            var buffer = stream.Dequeue(this.AudioFormat.ChunkLength);
            time_readed += this.AudioFormat.ChunkLength;
            var chunk = new AudioChunk(buffer, AudioFormat.SampleRate);
            return chunk;
        }

        public void Dispose()
        {
            waveIn.DataAvailable -= WaveIn_DataAvailable;
            waveIn.RecordingStopped -= WaveIn_RecordingStopped;
        }

        /// <summary>
        /// 录音设备信息
        /// </summary>
        /// <returns></returns>
        public IList<WaveInCapabilities> DevicesInfo()
        {
            var devices = new List<WaveInCapabilities>();
            for (var ix = 0; ix < WaveInEvent.DeviceCount; ix++)
            {
                var capabilities = WaveInEvent.GetCapabilities(ix);
                devices.Add(capabilities);
            }
            return devices;
        }
    }
}
#endif
