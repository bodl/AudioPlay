namespace Hi.Audio.Ref.Codec
{
    using Hi.Audio.Ref.GroovyCodecs.Mp3;
    using System;
    using System.IO;
    using System.Text;
    using MP3Sharp;

    /// <summary>
    /// mp3编解码器
    /// </summary>
    public class Mp3Codec : IAudioCodec, IDisposable
    {
        public AudioFormat AudioFormat { get; }
        Mp3Encoder Mp3Encoder { get; }
        private byte[] buffer = new byte[4096];
        private byte[] _cache;
        private int _cache_length = 0;
        private const string mp3_header = "ID3\x03\x00\x00\x00\x00\x00\x00";
        private Stream _cache_stream = new MemoryStream();
        private int _frame_size => AudioFormat.ChunkLength * AudioFormat.BlockAlign;
        private int _frames_size => _frame_size * 0x09;
        private byte[] _last_frame;
        private bool _has_last_frame = false;
        /// <param name="audioFormat">
        /// 目前仅支持 44100/48000 KHz; Chanels:2; BitsPerSample: 16 bit
        /// </param>
        public Mp3Codec(AudioFormat audioFormat) 
        {
            AudioFormat = audioFormat;
            
            Mp3Encoder = new Mp3Encoder(audioFormat);
            _last_frame = new byte[_frame_size];
            _cache = new byte[_frames_size];
        }

        public byte[] Encode(AudioChunk chunk)
        {
            var bytes = chunk.GetDataAsBytes();

            var length = Mp3Encoder.EncodeBuffer(bytes, 0, bytes.Length, buffer);
            var total = length + _cache_length;
            // 保证输出为 FrameSize的倍数位
            if (total >= _frames_size)
            {
                var extra = total % _frames_size;
                var data = new byte[total - extra];
                if (_cache_length > 0)
                {
                    Array.Copy(_cache, 0, data, 0, _cache_length);
                }
                Array.Copy(buffer, 0, data, _cache_length, length - extra);
                _cache_length = extra;
                if (_cache_length > 0)
                {
                    Array.Copy(buffer, length - extra, _cache, 0, _cache_length);
                }

                return data;
            }
            else
            {
                Array.Copy(buffer, 0, _cache, _cache_length, length);
                _cache_length += length;
            }
            return new byte[0];
        }

        public AudioChunk Decode(byte[] inputPacket)
        {
            var _mp3 = new MemoryStream();
            _mp3.Write(Encoding.ASCII.GetBytes(mp3_header));
            if (_has_last_frame)
            {
                _mp3.Write(_last_frame);
            }
            _mp3.Write(inputPacket);
            _mp3.Position = 0;
            _mp3.Seek(0, SeekOrigin.Begin);
            _mp3.Flush();
            Array.Copy(inputPacket, inputPacket.Length - _frame_size, _last_frame, 0, _frame_size);
            _has_last_frame = true;
            using (var stream = new MP3Stream(_mp3))
            {
                _cache_stream.SetLength(0);
                _cache_stream.Seek(0, SeekOrigin.Begin);
                //stream.Position -= inputPacket.Length;
                while (!stream.IsEOF)
                {
                    var len = stream.Read(buffer, 0 , 4096);
                    _cache_stream.Write(buffer, 0, len);
                }
                var buff = new byte[_cache_stream.Length];
                _cache_stream.Position = 0;
                _cache_stream.Read(buff, 0, buff.Length);
                var chunk = new AudioChunk(buff, AudioFormat.SampleRate);
                return chunk;
            }
        }

        public void Dispose()
        {
            _cache_stream.Dispose();
        }
    }
}
