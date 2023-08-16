namespace Hi.Audio.Ref
{
    using System;
    using Hi.Audio.Ref.Concentus.Structs;


    /// <summary>
    /// Opus加解码器
    /// </summary>
    /// <see cref="https://opus-codec.org/"/>
    /// <remarks>
    /// Bitrate: 6kb/s~510kb/s (Default:.120 KBit/s)<br/>
    /// Narrowband: 8KHz~48KHz<br/>
    /// FrameSize: 2.5ms~60ms (Default: 10ms)<br/>
    /// Mono/Stereo<br/>
    /// </remarks>
    public class OpusCodec : IAudioCodec
    {
        private byte[] scratchBuffer;
        private BasicBufferShort _incomingSamples;
        private int act_sampleRate;

        /// <summary>
        /// 采样率
        /// </summary>
        public virtual int SampleRate { get; }
        /// <summary>
        /// 通道数
        /// </summary>
        public virtual int Channels { get; }
        public virtual AudioFormat AudioFormat { get; }
        OpusApplication OpusApplication { get;} = OpusApplication.OPUS_APPLICATION_AUDIO;

        OpusDecoder OpusDecoder { get; }
        OpusEncoder OpusEncoder { get; }

        /// <summary>
        /// Opus加解码器
        /// </summary>
        /// <param name="audioFormat">音频格式</param>
        /// <param name="opusApplication">使用场景 (Default: OPUS_APPLICATION_AUDIO)</param>
        public OpusCodec(AudioFormat audioFormat, OpusApplication opusApplication = OpusApplication.OPUS_APPLICATION_AUDIO)
        {
            this.AudioFormat = audioFormat;
            this.act_sampleRate = audioFormat.SampleRate;
            this.Channels = audioFormat.Channels;
            this.OpusApplication = opusApplication;
            //must be 8 / 12 / 16 / 24 / 48 Khz
            if (act_sampleRate / 8 <= 1000)
            {
                this.SampleRate = 8000;
            }
            else if (act_sampleRate / 12 <= 1000)
            {
                this.SampleRate = 12000;
            }
            else if (act_sampleRate / 16 <= 1000)
            {
                this.SampleRate = 16000;
            }
            else if (act_sampleRate / 24 <= 1000)
            {
                this.SampleRate = 24000;
            }
            else
            {
                this.SampleRate = 48000;
            }
            this._incomingSamples = new BasicBufferShort(SampleRate);
            this.scratchBuffer = new byte[10000];

            OpusEncoder = OpusEncoder.Create(SampleRate, Channels, OpusApplication);
            OpusDecoder = OpusDecoder.Create(SampleRate, Channels);
        }

        private int _frameSize => (int)(SampleRate * FrameSize / 1000);
        /// <summary>
        /// 2.5\5\10\20\40\60 ms
        /// </summary>
        public double FrameSize = 10;
        public virtual int FrameSampleRate => _frameSize * Channels;
        /// <summary>
        /// 比特率 KBit/s
        /// </summary>
        public int Bitrate = 128;
        /// <summary>
        /// 0~10
        /// </summary>
        public int Complexity = 5;
        public bool VBR = false;
        public bool CVBR = false;

        /// <summary>
        /// 编码
        /// </summary>
        /// <remarks>
        /// 此方法会进行更新 Bitrate比特率、Complexity复杂度、VBR\CVBR\CBR。
        /// </remarks>
        /// <param name="input"></param>
        /// <returns>空列表则无数据</returns>
        public virtual byte[] Encode(AudioChunk input)
        {
            config_encoder();
            var data = input.Data;
            if (act_sampleRate != SampleRate) { data = Lanczos.Resample(data, act_sampleRate, SampleRate); }
            _incomingSamples.Write(data);

            int outCursor = 0;
            if (_incomingSamples.Available() >= FrameSampleRate)
            {
                short[] nextFrameData = _incomingSamples.Read(FrameSampleRate);
                int thisPacketSize = OpusEncoder.Encode(nextFrameData, 0, _frameSize, scratchBuffer, outCursor, scratchBuffer.Length);
                outCursor += thisPacketSize;
            }

            byte[] finalOutput = new byte[outCursor];
            Array.Copy(scratchBuffer, 0, finalOutput, 0, outCursor);
            return finalOutput;
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="inputPacket"></param>
        /// <returns></returns>
        public virtual AudioChunk Decode(byte[] inputPacket)
        {
            short[] outputBuffer = new short[FrameSampleRate];

            // Normal decoding
            OpusDecoder.Decode(inputPacket, 0, inputPacket.Length, outputBuffer, 0, _frameSize, false);

            short[] finalOutput = new short[FrameSampleRate];
            Array.Copy(outputBuffer, finalOutput, finalOutput.Length);

            if (act_sampleRate != SampleRate)
            {
                finalOutput = Lanczos.Resample(finalOutput, SampleRate, act_sampleRate);
            }
            var chunk = new AudioChunk(finalOutput, SampleRate);
            return chunk;
        }

        private void config_encoder()
        {
            OpusEncoder.Bitrate = this.Bitrate * 1024; // 1024Bit = 1 KBit
            OpusEncoder.Complexity = this.Complexity;
            OpusEncoder.UseVBR = VBR;
            OpusEncoder.UseConstrainedVBR = CVBR;
            OpusEncoder.EnableAnalysis = false;
        }
    }
}
