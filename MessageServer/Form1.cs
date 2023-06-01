using System.Net.Sockets;
using System.Net;
using System.Text;

namespace MessageServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ipadr = IPAddress.Loopback;
        }

        //�������ͻ��˵�ͨ���׽���
        public static Dictionary<String, Socket> clientList = null;
        //����һ�������׽��� 
        Socket serverSocket = null;
        //����һ���������
        Boolean isListen = true;
        //�����������߳�
        Thread thStartListen;
        //Ĭ��һ������������IP
        IPAddress ipadr;
        //��endpoint����Ϊ��Ա�ֶ�
        IPEndPoint endPoint;


        private void button1_Click(object sender, EventArgs e)
        {
            if (serverSocket == null)
            {
                try
                {
                    isListen = true;
                    clientList = new Dictionary<string, Socket>();



                    //ʵ�������׽���

                    //�ο���ַ��http://blog.csdn.net/sight_/article/details/8138802
                    //int socket(int domain, int type, int protocol);
                    //  domain:   Э��������Э���塣���õ�Э�����У�AF_INET��AF_INET6��AF_LOCAL�����AF_UNIX��Unix��socket����AF_ROUTE�ȵȡ�
                    //Э���������socket�ĵ�ַ���ͣ���ͨ���б�����ö�Ӧ�ĵ�ַ����AF_INET������Ҫ��ipv4��ַ��32λ�ģ���˿ںţ�16λ�ģ�����ϡ�AF_UNIX������Ҫ��һ������·������Ϊ��ַ��
                    //  type:     ָ��socket���ͣ������õ�socket�����У�SOCK_STREAM��SOCK_DGRAM��SOCK_RAW��SOCK_PACKET��SOCK_SEQPACKET�ȵ�
                    //  protocol:   ָ��Э�顣���õ�Э���У�IPPROTO_TCP��IPPTOTO_UDP��IPPROTO_SCTP��IPPROTO_TIPC��
                    //�����������type��protocol����������ϵģ���SOCK_STREAM�����Ը�IPPROTO_UDP��ϡ���protocolΪ0ʱ�����Զ�ѡ��type���Ͷ�Ӧ��Ĭ��Э��
                    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);     //AddressFamily.InterNetwork����IPV4��ַ��������IPV6   �ο���ַ��http://bbs.csdn.net/topics/390283656?page=1

                    //�˵�
                    /*  ��IPEndPoint���������������õĹ��캯����
                        public IPEndPoint(long, int); 
                        public IPEndPoint(IPAddress, int);
                        ���ǵ����þ�����ָ���ĵ�ַ�Ͷ˿ںų�ʼ��IPEndPoint�����ʵ����
                     * �ο���ַ��http://www.cnblogs.com/Medeor/p/3546359.html
                     */
                    //IPAddress ipadr = IPAddress.Parse("192.168.1.100");
                    //���txtIP������ֵ����ѡ�������IP��Ϊ������IP������Ļ���Ĭ���Ǳ�����

                    endPoint = new IPEndPoint(ipadr, 8080);     //IPAddress.loopback�Ǳ��ػ��ؽӿڣ���ʵ������ӿڣ��������ڵ�  �ο���ַ��http://baike.sogou.com/v7893363.htm?fromTitle=loopback
                    //��
                    //��һ����ַ����ض���ַ��socket
                    //int bind(int sockfd, const struct sockaddr *addr, socklen_t addrlen);
                    //sockfd:   ��socket�����֣�����ͨ��socket()���������ˣ�Ψһ��ʶһ��socket��bind()�������ǽ�����������ְ�һ�����֡�
                    //*addr:    һ��const struct sockaddr *ָ�룬ָ��Ҫ�󶨸�sockfd��Э���ַ�������ַ�ṹ���ݵ�ַ����socketʱ�ĵ�ַЭ����Ĳ�ͬ����ͬ
                    //addrlen:  ��Ӧ���ǵ�ַ�ĳ���
                    //ͨ����������������ʱ�򶼻��һ��������֪�ĵ�ַ����ip��ַ+�˿ںţ��������ṩ���񣬿ͻ��Ϳ���ͨ������������������
                    //���ͻ��˾Ͳ���ָ������ϵͳ�Զ�����һ���˿ںź������ip��ַ��ϡ�
                    //�����Ϊʲôͨ������������listen֮ǰ�����bind()�����ͻ��˾Ͳ�����ã�������connect()ʱ��ϵͳ�������һ����
                    //�ο���ַ��http://blog.csdn.net/sight_/article/details/8138802

                    //���������bind���������bind����.NET�����һ��bind��ʹ Socket ��һ�������ս��������� �����ռ�:System.Net.Sockets  ����:System���� system.dll �У�
                    //���׽��ְ�һ���˵㣬��ʵ��������������bindҲ��ʵ��
                    //�ο���վ�� https://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.bind(VS.80).aspx
                    //10.127.221.248
                    try
                    {
                        serverSocket.Bind(endPoint);

                        //�������������
                        //�����Ϊһ�����������ڵ���socket()��bind()֮��ͻ����listen()���������socket������ͻ�����ʱ����connect()�����������󣬷������˾ͻ���յ��������
                        //int listen(int sockfd, int backlog);
                        //listen�����ĵ�һ��������ΪҪ������socket�����֣��ڶ�������Ϊ��Ӧsocket�����Ŷӵ�������Ӹ�����
                        //socket()����������socketĬ����һ���������͵ģ�listen������socket��Ϊ�������͵ģ��ȴ��ͻ�����������
                        serverSocket.Listen(100);

                        thStartListen = new Thread(StartListen);
                        thStartListen.IsBackground = true;
                        thStartListen.Start();

                        //�����е㲻һ����ԭ���õ���  txtMsg.Dispatcher.BeginInvoke

                        /*Invoke���߳��еȴ�Dispatcher����ָ����������ɺ��������Ĳ�����
                         * BeginInvoke���صȴ�Dispatcher�����ƶ�������ֱ�Ӽ�������Ĳ�����
                         * �ο���ַ�� https://zhidao.baidu.com/question/1175146013330422099.html?qbl=relate_question_1&word=Dispatcher.BeginInvoke%B5%C4%CF%E0%CD%AC%BA%AF%CA%FD
                         * ���õĲο���ַ��http://www.cnblogs.com/lsgsanxiao/p/5523282.html
                        **/
                        textBox2.BeginInvoke(new Action(() =>
                        {
                            textBox2.Text += "���������ɹ�...\r\n";
                        }));
                    }
                    catch (Exception eg)
                    {
                        MessageBox.Show("�����IP��ַ��Ч������������!");
                        textBox2.BeginInvoke(new Action(() =>
                        {
                            textBox2.Text += "��������ʧ��...\r\n";
                        }));


                        if (serverSocket != null)
                        {
                            serverSocket.Close();
                            thStartListen.Abort();  //���������̹ص�

                            BroadCast.PushMessage("Server has closed", "", false, clientList);
                            foreach (var socket in clientList.Values)
                            {
                                socket.Close();
                            }
                            clientList.Clear();

                            serverSocket = null;
                            isListen = false;

                        }
                    }
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

        }

        //�̺߳�������װһ���������ӵ�ͨ���׽���
        private void StartListen()
        {
            isListen = true;
            //default()ֻ������Ϊһ����ʼֵ������Ӧ��Ϊnull  �ο���ַ��https://stackoverflow.com/questions/28720717/why-default-in-c-sharp-tcpclient-clientsocket-defaulttcpclient
            Socket clientSocket = default(Socket);

            while (isListen)
            {
                try
                {
                    //�ο���ַ�� http://bbs.csdn.net/topics/30100253
                    //  int accept(int sockfd, void *addr, int *addrlen); 
                    //ע�����serverSocket�����������������׽��֣������û������϶˿ں�᷵��һ���µ��׽���Ҳ���������clientSocket��sercerSocket�������Ƕ�����������
                    //��ϸ�ο���ַ��http://www.360doc.com/content/13/0908/17/13253385_313070996.shtml
                    //����ֵ��һ���µ��׽�������������������ǺͿͻ��˵��µ����ӣ����socket�൱��һ���ͻ��˵�socket���������ǿͻ��˵�ip��port
                    //������Ҳ�̳��ֱ��صļ����׽��֣������Ҳ�з�������ip��port��Ϣ
                    if (serverSocket == null)   //�������ֹͣ����serverSocketΪ���ˣ��Ǿ�ֱ�ӷ���
                    {
                        return;
                    }
                    clientSocket = serverSocket.Accept();   //�����������һ��ͨ���׽��֣���������׽��ֽ���ͨ�ţ�����ʱ����-1������ȫ�ִ������
                }
                catch (SocketException e)
                {
                    File.AppendAllText("E:\\Exception.txt", e.ToString() + "\r\nStartListen\r\n" + DateTime.Now.ToString() + "\r\n");
                }

                //TCP�������ֽ�����
                Byte[] bytesFrom = new Byte[4096];
                String dataFromClient = null;

                if (clientSocket != null && clientSocket.Connected)
                {
                    try
                    {
                        //Socket.Receive() �ο���ַ��http://blog.csdn.net/cpcpc/article/details/7245420
                        //public int Receive(  byte[] buffer,  int offset,   int size,  SocketFlags socketFlags )  
                        //buffer  ��byte���͵����飬�洢�յ������ݵ�λ��
                        //offset  ��buffer�д洢���������ݵ�λ��
                        //size    Ҫ���յ��ֽ���
                        //socketFlags  socketFlagesֵ�İ�λ���

                        Int32 len = clientSocket.Receive(bytesFrom);    //��ȡ�ͻ��˷�������Ϣ,���صľ����յ����ֽ���,���Ұ��յ�����Ϣ������bytesForm����

                        if (len > -1)
                        {
                            String tmp = Encoding.UTF8.GetString(bytesFrom, 0, len);  //���ֽ���ת�����ַ���
                            /*try
                            {
                                dataFromClient = EncryptionAndDecryption.TripleDESDecrypting(tmp);      //���ݼ��ܴ���
                            }
                            catch (Exception e)
                            {

                            }
                             catch (Exception e)
                            {

                            }*/
                            dataFromClient = tmp;
                            Int32 sublen = dataFromClient.LastIndexOf("$");
                            if (sublen > -1)
                            {
                                dataFromClient = dataFromClient.Substring(0, sublen);   //��ȡ�û���

                                if (!clientList.ContainsKey(dataFromClient))
                                {
                                    clientList.Add(dataFromClient, clientSocket);   //����û��������ڣ�������û�����ȥ

                                    //BroadCast�������Լ������һ���࣬����������Ϣ�������û��������͵�
                                    //PushMessage(String msg, String uName, Boolean flag, Dictionary<String, Socket> clientList)
                                    BroadCast.PushMessage(dataFromClient + "Joined", dataFromClient, false, clientList);

                                    //HandleClientҲ��һ���Լ�������࣬����������տͻ��˷�������Ϣ��ת�������еĿͻ���
                                    //StartClient(Socket inClientSocket, String clientNo, Dictionary<String, Socket> cList)
                                    HandleClient client = new HandleClient(textBox2);

                                    client.StartClient(clientSocket, dataFromClient, clientList);

                                    textBox2.BeginInvoke(new Action(() =>
                                    {
                                        textBox2.Text += dataFromClient + "�������˷�����\r" + DateTime.Now + "\r\n";
                                    }));
                                }
                                else
                                {
                                    //�û����Ѿ�����
                                    clientSocket.Send(Encoding.UTF8.GetBytes("#" + dataFromClient + "#"));
                                }
                            }
                        }
                    }
                    catch (Exception ep)
                    {
                        File.AppendAllText("E:\\Exception.txt", ep.ToString() + "\r\n\t\t" + DateTime.Now.ToString() + "\r\n");
                    }
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (serverSocket != null)
            {
                serverSocket.Close();
                thStartListen.Abort();  //���������̹ص�

                BroadCast.PushMessage("Server has closed", "", false, clientList);
                foreach (var socket in clientList.Values)
                {
                    socket.Close();
                }
                clientList.Clear();

                serverSocket = null;
                isListen = false;
                textBox2.Text += "����ֹͣ���Ͽ����пͻ�������\t" + DateTime.Now.ToString() + "\r\n";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                clientList = new Dictionary<string, Socket>();
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//ʵ���������׽���
                                                                                                           //IPAddress ipadr = IPAddress.Parse("192.168.1.100");

                endPoint = new IPEndPoint(ipadr, 8080);//�˵�
                serverSocket.Bind(endPoint);//��
                serverSocket.Listen(100);   //�������������
                thStartListen = new Thread(StartListen);
                thStartListen.IsBackground = true;
                thStartListen.Start();
                textBox2.BeginInvoke(new Action(() =>
                {
                    textBox2.Text += "���������ɹ�...\r\n";
                }
                ));
                label3.BeginInvoke(new Action(() =>
                {
                    label3.Text = endPoint.Address.ToString();
                }));
            }
            catch (SocketException ep)
            {
                MessageBox.Show(ep.ToString());
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serverSocket != null)
            {
                BroadCast.PushMessage("Server has closed", "", false, clientList);
                foreach (var socket in clientList.Values)
                {
                    socket.Close();
                }
                clientList.Clear();
                serverSocket.Close();
                serverSocket = null;
                isListen = false;
                textBox2.Text += "����ֹͣ\r\n";
            }

        }

        //���ü�����IP��ַ
        private void btnResetIp_Click(object sender, EventArgs e)
        {

            //���txtIP������ֵ����ѡ�������IP��Ϊ������IP������Ļ���Ĭ���Ǳ�����
            if (!String.IsNullOrWhiteSpace(textBox1.Text.ToString().Trim()))
            {
                try
                {
                    ipadr = IPAddress.Parse(textBox1.Text.ToString().Trim());
                    btnStop_Click(sender, e);
                    textBox2.BeginInvoke(new Action(() =>
                    {
                        textBox2.Text += "�����������У����Ժ�...\r\n";
                    }));

                    button1_Click(sender, e);


                    label3.BeginInvoke(new Action(() =>
                    {
                        label3.Text = endPoint.Address.ToString();
                    }));
                }
                catch (Exception ep)
                {
                    MessageBox.Show("��������ȷ��IP������");
                }
            }
            else
            {
                MessageBox.Show("���������ú��IP��ַ�����ԣ�");
            }


        }

        private void btnRcv_Click(object sender, EventArgs e)
        {
            if (ipadr == IPAddress.Loopback)
            {
                MessageBox.Show("��ǰ�Ѿ�����Ĭ��״̬�������޸�");
            }
            else
            {
                ipadr = IPAddress.Loopback;
                btnStop_Click(sender, e);
                textBox2.BeginInvoke(new Action(() =>
                {
                    textBox2.Text += "�����������У����Ժ�...\r\n";
                }));
                button1_Click(sender, e);
                label3.BeginInvoke(new Action(() =>
                {
                    label3.Text = endPoint.Address.ToString();
                }));
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBox2.BeginInvoke(new Action(() =>
            {
                textBox2.Text = "-----------������-----------\r\n";
            }));
        }

    }





}