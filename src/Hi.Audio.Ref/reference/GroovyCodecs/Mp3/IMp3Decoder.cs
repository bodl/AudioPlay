using System.IO;

namespace Hi.Audio.Ref.GroovyCodecs.Mp3
{
    public interface IMp3Decoder
    {
        void close();

        void decode(MemoryStream sampleBuffer, bool playOriginal);
    }
}