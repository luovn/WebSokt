using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建一个新的Socket,基于TCP的Stream Socket（流式套接字）
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //将该socket绑定到主机上面的某个端口
            socket.Bind(new IPEndPoint(IPAddress.Any, 4530));

            //启动监听，并且设置一个最大的队列长度
            socket.Listen(4);

            //开始接受客户端连接请求
            socket.BeginAccept(new AsyncCallback((ar) =>
            {
                //客户端的Socket实例，后续可以将其保存起来
                var client = socket.EndAccept(ar);

                //给客户端发送一个消息
                client.Send(Encoding.Unicode.GetBytes("Hi there, I accept you request at " + DateTime.Now.ToString()));

                //实现每隔一段时间发送
                //定时器
                var timer = new System.Timers.Timer();
                timer.Interval = 3000D;
                timer.Enabled = true;
                timer.Elapsed += (o, a) =>
                {
                    if (client.Connected)
                    {
                        try
                        {
                            client.Send(Encoding.Unicode.GetBytes("Message from server at" + DateTime.Now.ToString()));
                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine(ex.Message);
                        }
                    }
                    else {
                        timer.Stop();
                        timer.Enabled = false;
                    }
                };
                timer.Start();

                //接受客户端发过来的消息(这个和在客户端实现的方式是一样的） 双向
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceieveMessage), client);

            }), null);


            Console.WriteLine("Server is ready!");
            Console.Read();

        }

        static byte[] buffer = new byte[1024];
        public static void ReceieveMessage(IAsyncResult ar) {
            try
            {
                var socket = ar.AsyncState as Socket;

                var length = socket.EndReceive(ar);
                var message = Encoding.Unicode.GetString(buffer);
                Console.WriteLine(message);

                socket.BeginReceive(buffer, 0, length, SocketFlags.None, new AsyncCallback(ReceieveMessage), socket);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }
    }
}
