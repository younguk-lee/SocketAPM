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
using System.Net.NetworkInformation;
using System.Windows.Forms;


namespace SocketPilotServer
{
    /// <summary>
    /// TCPIPServer Class는 SBC와의 통신에서 Server의 역할을 위하여 구현된 비동기 동작을 위한 버퍼
    /// @class TCPIPServer
    /// @author Younguk Lee
    /// @date 2020.11.18
    /// </summary>
    public class AsyncStateData
    {
        public byte[] Buffer;
        public Socket Socket;
    }
    /// <summary>
    /// TCPIPServer Class는 SBC와의 통신에서 Server의 역할을 위하여 구현된 비동기동작(APM(Asynchronous Programming Model) 방식)
    /// @class TCPIPServer
    /// @author Younguk Lee
    /// @date 2020.11.18
    /// </summary>
    class Program ///TCPIPServer
    {
        static void Main(string[] args)
        {
            ///서버 소켓이 동작하는 스레드
            Thread serverThread = new Thread(serverFunc);
            serverThread.IsBackground = true;
            serverThread.Start();
            Thread.Sleep(500); ///소켓 서버용 스레드가 실행될 시간을 주기 위해

            Console.WriteLine("종료하려면 아무 키나 누르세요...");
            Console.ReadLine();
        }

        private static void serverFunc(object obj)
        {
            Socket svrSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ipAddress = GetCurrentIPAddress();
            if (ipAddress == null)
            {
                Console.WriteLine("No Lan card for IPv4");
                return;
            }
            else
            {
                Console.WriteLine("Get IP : {0}", ipAddress.ToString());
            }
            IPEndPoint endPoint = new IPEndPoint(ipAddress, 11200);

            svrSocket.Bind(endPoint);
            svrSocket.Listen(10);

            while (true)
            {
                Socket clntSocket = svrSocket.Accept();

                AsyncStateData data = new AsyncStateData();
                data.Buffer = new byte[1024];
                data.Socket = clntSocket;

                clntSocket.BeginReceive(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, asyncReceiveCallback, data);
            }
        }

        private static void asyncReceiveCallback(IAsyncResult asyncReulst)
        {
            try
            {
                AsyncStateData rcvData = asyncReulst.AsyncState as AsyncStateData;
                int nRecv = rcvData.Socket.EndReceive(asyncReulst);
                string txt = Encoding.UTF8.GetString(rcvData.Buffer, 0, nRecv);

                byte[] sendBytes = Encoding.UTF8.GetBytes("Hello: " + txt);
                rcvData.Socket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, asyncSendCallback, rcvData.Socket);
            }
            catch (SocketException)
            {

            }
        }

        private static void asyncSendCallback(IAsyncResult asyncResult)
        {
            Socket socket = asyncResult.AsyncState as Socket;
            socket.EndSend(asyncResult);
        }

        private static IPAddress GetCurrentIPAddress()
        {
            IPAddress[] addrs = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

            foreach (IPAddress ipAddr in addrs)
            {
                if (ipAddr.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipAddr;
                }
            }
            return null;
        }
    }
}

