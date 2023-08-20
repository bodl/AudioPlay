namespace AudioPlay{

    using System;
    using NetPs.Udp;
    using NAudio;
    using System.IO;
    using NAudio.Wave;
    using System.Reactive.Concurrency;
    using System.Threading.Tasks;
    using Hi.Audio;
    using Hi.Audio.Ref;
    using System.Collections.Generic;
    using MP3Sharp;

    class Program
    {
        static async Task Main()
        {
            await RealtimePlayDemo.Run();
        }
    }
}