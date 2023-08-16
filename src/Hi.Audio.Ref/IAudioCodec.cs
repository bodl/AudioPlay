namespace Hi.Audio.Ref
{
    using System;

    /// <summary>
    /// 音频 加解码器
    /// </summary>
    public interface IAudioCodec
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="chunk">块</param>
        /// <returns></returns>
        byte[] Encode(AudioChunk chunk);
        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="inputPacket">数据包</param>
        /// <returns></returns>
        AudioChunk Decode(byte[] inputPacket);
    }
}
