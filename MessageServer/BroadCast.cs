using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MessageServer
{
    public class BroadCast
    {
        //flag是用来判断传进来的msg前面是否需要加上uName:，也就是判断是不是系统信息，是系统信息的话就设置flag为false
        public static void PushMessage(String msg, String uName, Boolean flag, Dictionary<String, Socket> clientList)
        {
            foreach (var item in clientList)
            {
                Socket brdcastSocket = (Socket)item.Value;
                String msgTemp = null;
                Byte[] castBytes = new Byte[4096];
                if (flag == true)
                {
                    msgTemp = uName + ":" + msg + "\t\t" + DateTime.Now.ToString();
                    castBytes = Encoding.UTF8.GetBytes(msgTemp);
                }
                else
                {
                    msgTemp = msg + "\t\t" + DateTime.Now.ToString();
                    castBytes = Encoding.UTF8.GetBytes(msgTemp);
                }
                try
                {
                    brdcastSocket.Send(castBytes);
                }
                catch (Exception e)
                {
                    brdcastSocket.Close();
                    brdcastSocket = null;
                    File.AppendAllText("E:\\Exception.txt", e.ToString() + "\r\nPushMessage\r\n" + DateTime.Now.ToString() + "\r\n");
                    continue;
                }
            }
        }


    }
}
