using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessageServer
{
    //该类专门负责接收客户端发来的消息，并转发给所有的客户端
    public class HandleClient
    {
        Socket clientSocket;
        String clNo;
        Dictionary<String, Socket> clientList = new Dictionary<string, Socket>();
        TextBox txtMsg;
        public HandleClient() { }
        public HandleClient(TextBox tb)
        {
            txtMsg = tb;
        }


        public void StartClient(Socket inClientSocket, String clientNo, Dictionary<String, Socket> cList)
        {
            clientSocket = inClientSocket;
            clNo = clientNo;
            clientList = cList;

            Thread th = new Thread(Chat);
            th.IsBackground = true;
            th.Start();
        }

        private void Chat()
        {
            Byte[] bytesFromClient = new Byte[4096];
            String dataFromClient;
            String msgTemp = null;
            Byte[] bytesSend = new Byte[4096];
            Boolean isListen = true;

            while (isListen)
            {
                try
                {
                    if (clientSocket == null || !clientSocket.Connected)
                    {
                        return;
                    }
                    if (clientSocket.Available > 0)
                    {
                        Int32 len = clientSocket.Receive(bytesFromClient);
                        if (len > -1)
                        {
                            dataFromClient = Encoding.UTF8.GetString(bytesFromClient, 0, len);
                            if (!String.IsNullOrWhiteSpace(dataFromClient))
                            {
                                dataFromClient = dataFromClient.Substring(0, dataFromClient.LastIndexOf("$"));   //这里的dataFromClient是消息内容，上面的是用户名
                                if (!String.IsNullOrWhiteSpace(dataFromClient))
                                {
                                    BroadCast.PushMessage(dataFromClient, clNo, true, clientList);
                                    msgTemp = clNo + ":" + dataFromClient + "\t\t" + DateTime.Now.ToString();
                                    String newMsg = msgTemp;
                                    File.AppendAllText("E:\\MessageRecords.txt", newMsg + "\r\n", Encoding.UTF8);
                                }
                                else
                                {
                                    isListen = false;
                                    clientList.Remove(clNo);
                                    txtMsg.BeginInvoke(new Action(() =>
                                    {
                                        txtMsg.Text += clNo + "已断开与服务器连接\r" + DateTime.Now + "\r\n";
                                    }));
                                    BroadCast.PushMessage(clNo + "已下线\r", "", false, clientList);
                                    clientSocket.Close();
                                    clientSocket = null;
                                }
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    isListen = false;
                    clientList.Remove(clNo);


                    clientSocket.Close();
                    clientSocket = null;
                    File.AppendAllText("E:\\Exception.txt", e.ToString() + "\r\nChat\r\n" + DateTime.Now.ToString() + "\r\n");
                }
            }

        }

    }
}
