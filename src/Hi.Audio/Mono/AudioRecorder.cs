#if MONOANDROID
#region 录音权限
//录音
using Android.App;

[assembly: UsesPermission(Android.Manifest.Permission.ModifyAudioSettings)]
[assembly: UsesPermission(Android.Manifest.Permission.RecordAudio)]
#endregion
namespace Hi.Audio
{
    using System;
    using Android.Media;
    using NetPs.Socket;

    public class AudioRecorder : IAudioRecorder
    {
        public AudioFormat AudioFormat { get; }
        public long TotalTime => (queueStream.Length + time_readed) / AudioFormat.ChunkLength;
        public bool CanReadChunk => queueStream.Length >= AudioFormat.ChunkLength;
        public AudioRecord? AudioRecord;
        public AudioSource AudioSource { get; set; } = AudioSource.Default;
        public bool Recording => AudioRecord?.RecordingState == RecordState.Recording;

        private byte[] _buffer;
        private QueueStream queueStream;
        private long time_readed = 0;

        public event EventHandler? DataAvailable;
        public event EventHandler? RecordingStopped;

        public AudioRecorder(AudioFormat audioFormat)
        {
            AudioFormat = audioFormat;
            _buffer = new byte[AudioFormat.GetBufferSizeInBytes()];
            queueStream = new QueueStream();
        }
        public void Dispose()
        {
            if (AudioRecord != null)
            {
                AudioRecord.PeriodicNotification += AudioRecord_PeriodicNotification;
                AudioRecord.Release();
            }
        }

        public AudioChunk GetNextChunk()
        {
            var buffer = queueStream.Dequeue(this.AudioFormat.ChunkLength);
            time_readed += this.AudioFormat.ChunkLength;
            var chunk = new AudioChunk(buffer, AudioFormat.SampleRate);
            return chunk;
        }

        public async void Start()
        {
            if (AudioRecord == null)
            {
                AudioRecord = new AudioRecord(AudioSource, AudioFormat.SampleRate, AudioFormat.GetChannelIn(), AudioFormat.GetEncoding(), AudioFormat.GetBufferSizeInBytes());
                AudioRecord.PeriodicNotification += AudioRecord_PeriodicNotification;
            }
            if (AudioRecord.RecordingState == RecordState.Recording) return;
            queueStream.Clear();
            AudioRecord?.StartRecording();
            try
            {
                while (AudioRecord?.RecordingState == RecordState.Recording)
                {
                    var length = await AudioRecord.ReadAsync(_buffer, 0, AudioFormat.GetBufferSizeInBytes());
                    queueStream.Enqueue(_buffer, 0, length);
                    this.DataAvailable?.Invoke(this, null);
                }
            }
            finally
            {
                Stop();
            }
        }

        private void AudioRecord_PeriodicNotification(object sender, AudioRecord.PeriodicNotificationEventArgs e)
        {
            if (AudioRecord == null) return;
            var length = AudioRecord.Read(_buffer, 0, AudioFormat.GetBufferSizeInBytes());
            if (length  == 0) return;
            queueStream.Enqueue(_buffer, 0, length);
            this.DataAvailable?.Invoke(this, null);
        }

        public void Stop()
        {
            if (AudioRecord?.RecordingState != RecordState.Recording) return;
            AudioRecord?.Stop();
            this.RecordingStopped?.Invoke(this, null);
        }
    }
}
#endif