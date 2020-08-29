using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Model;

namespace ConsoleApp1.Service
{
    public class EncodeVideoService
    {
        public delegate void VideoEncodeDelegate(object source, EventArgs args);
        public event VideoEncodeDelegate VideoEncoded;
        public void EncodeVideo(Video video)
        {
            //
            Console.WriteLine("Video Encoded");
            OnVideoEncoed();
        }
        protected virtual void OnVideoEncoed()
        {
            if (VideoEncoded!=null)
            {
                VideoEncoded(this, EventArgs.Empty);
            }
        }
    }
}
