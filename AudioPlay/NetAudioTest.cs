using Hi.Audio.Ref;
using Hi.Audio;
using NetPs.Udp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlay
{
    internal class NetAudioTest
    {
        public static void Run()
        {

            var format = new AudioFormat(48000, 1, 16);
            format.BufferMilliseconds = 10;
            format.DesiredLatency = 100;
            new FlacCodec(format);
            var reader = new AudioRecorder(format);
            var player = new AudioPlayer(reader.AudioFormat);
            var codec = new FlacCodec(reader.AudioFormat);
            //codec.Bitrate = 510;
            //codec.Complexity = 10;
            //codec.FrameSize = 10;
            //codec.VBR = true;
            var host = new UdpHost("0.0.0.0:15926");
            var dtag = new byte[] { 1, 2, 3 };
            var txs = new List<UdpTx>();
            host.ReceicedObservable.Subscribe(data =>
            {
                if (data.Data.Length == 3 && data.Data[0] == 1 && data.Data[1] == 2 && data.Data[2] == 3)
                {
                    var tx = host.GetTx(data.IP);
                    host.Disposables.Add(tx);
                    txs.Add(tx);
                    return;
                }
                var chunk = codec.Decode(data.Data);
                player.Add(chunk);
                player.Play();
            });
            host.Rx.StartReveice();
            using (var tx = host.GetTx("192.168.2.12:15926"))
            //using (var tx = host.GetTx("192.168.68.213:15926"))
            {
                tx.Transport(dtag);
            }

            reader.Start();
            while (reader.Recording)
            {
                while (reader.CanReadChunk)
                {
                    var chunk = reader.GetNextChunk();
                    var buffer = codec.Encode(chunk);
                    foreach (var tx in txs)
                    {
                        tx.Transport(buffer);
                    }
                }
            }
            Console.ReadLine();
        }
    }
}
