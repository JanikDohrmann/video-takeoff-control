using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace video_takeoff_control.video_source
{
    internal class IpVideoSource : IVideoSource
    {
        private MainWindow mainWindow;

        private Socket socket_receive;
        private Socket socket_send;

        private string camId;

        private bool recording;

        public IpVideoSource(MainWindow mainWindow, string camId)
        {
            this.mainWindow = mainWindow;
            this.camId = camId;
            this.recording = false;

            IPEndPoint endPoint_receive = new IPEndPoint(IPAddress.Parse(Settings.videoSourceReceiveIps[camId]), Settings.videoSourceReceivePorts[camId]);
            socket_receive = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket_receive.Bind(endPoint_receive);
            socket_receive.Listen(10); // Wartet auf eingehende Verbindungen

            IPEndPoint endPoint_send = new IPEndPoint(IPAddress.Parse(Settings.videoSourceSendIps[camId]), Settings.videoSourceSendPorts[camId]);
            socket_send = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket_send.Connect(endPoint_send); // Verbindet zu einem Server
        }
        
        public void close()
        {
            socket_receive.Close();
        }

        public void preview()
        {
            while (recording)
            {
                video_NewFrame();
            }
        }

        public void startRecording()
        {
            throw new NotImplementedException();
        }

        public void stopRecording()
        {
            recording = false;
        }

        // Wartet auf eingehende Verbindung und empfängt Daten
        public async Task<Bitmap> ReceiveImageAsync()
        {
            Socket clientSocket = await socket_receive.AcceptAsync();
            int imageSize = Settings.videoSourceImageSize[camId];

            // Lese die Bilddaten
            byte[] imageBuffer = new byte[imageSize];
            int totalBytesReceived = 0;
            while (totalBytesReceived < imageSize)
            {
                int bytesToRead = Math.Min(1024, imageSize - totalBytesReceived);
                int bytesReceived = await clientSocket.ReceiveAsync(imageBuffer, SocketFlags.None);
                totalBytesReceived += bytesReceived;
            }

            // Konvertiere die Bilddaten in ein Bitmap
            using (MemoryStream ms = new MemoryStream(imageBuffer))
            {
                return new Bitmap(ms);
            }
        }

        private async void video_NewFrame()
        {
            Bitmap bitmap = await ReceiveImageAsync();
            mainWindow.newFrame(bitmap);
        }
    }
}
