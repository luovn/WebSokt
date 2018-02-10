using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建一个Socket
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //连接到指定服务器的指定端口
            socket.Connect("localhost", 4530);

            ////实现接受消息的方法

            //var buffer = new byte[1024];//设置一个缓冲区，用来保存数据
            //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback((ar) =>
            //{
            //    var length = socket.EndReceive(ar);
            //    //读取出来消息内容
            //    var message = Encoding.Unicode.GetString(buffer, 0, length);
            //    //显示消息
            //    Console.WriteLine(message);

            //}), null);


            

            Console.WriteLine("connect to the server");

            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceieveMessage), socket);

            //向服务器发送数据
            while (true) {
                var message = "Message from Client:" + Console.ReadLine();
                var outputbuffer = Encoding.Unicode.GetBytes(message);
                socket.BeginSend(outputbuffer, 0, outputbuffer.Length, SocketFlags.None, null, null);
            }
            
        }
        static byte[] buffer = new byte[1024];
        //一直接受消息
        public static void ReceieveMessage(IAsyncResult  ar) {
            try
            {
                var socket = ar.AsyncState as Socket;
                var length = socket.EndReceive(ar);
                //读取消息
                var message = Encoding.Unicode.GetString(buffer, 0, length);
                //显示消息
                Console.WriteLine(message);

                //读取下一条信息
                socket.BeginReceive(buffer, 0,buffer.Length,SocketFlags.None, new AsyncCallback(ReceieveMessage), socket);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
        
    }
}
