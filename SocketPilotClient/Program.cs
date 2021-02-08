///
/// CTS Pack, Monitor Pro Developed by Dept. RnD
/// Copyright 2020 by PNESOLUTION
/// All rights reserved.
/// This software is the confidential and proprietary information
/// of PNESOLUTION. (“Confidential Information”). You shall not
/// disclose such Confidential Information and shall use it only in
/// accordance with the terms of the license agreement you entered into
/// with PNESOLUTION.
///
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace SocketPilotClient
{
    /// <summary>
    /// TCPIPClient Class는 SBC와의 통신에서 Client의 역할로 Server에 통신을 요청하며 데이터를 주고 받는다.
    /// @class TCPIPClient
    /// @author Younguk Lee
    /// @date 2020.11.18
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            /// 클라이언트 소켓이 동작하는 스레드
            Thread clientThread = new Thread(clientFunc);
            clientThread.IsBackground = true;
            clientThread.Start();

            Console.WriteLine("종료하려면 아무 키나 누르세요...");
            Console.ReadLine();
        }

        private static void clientFunc(object obj)
        {
            try
            {
                IPAddress localAddress = IPAddress.Parse("192.168.59.1");
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                EndPoint serverEP = new IPEndPoint(localAddress, 11200);
                Console.WriteLine("Try Connect to IP : {0}", localAddress.ToString());
                socket.Connect(serverEP);
                byte[] buf = Encoding.UTF8.GetBytes(DateTime.Now.ToString());
                socket.Send(buf);

                byte[] recvBytes = new byte[1024];
                int nRecv = socket.Receive(recvBytes);
                string txt = Encoding.UTF8.GetString(recvBytes, 0, nRecv);

                Console.WriteLine(txt);

                socket.Close();
                Console.WriteLine("TCP Client socket: Closed");
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
