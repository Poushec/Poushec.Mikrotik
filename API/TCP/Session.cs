using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using Poushec.Mikrotik.Configurations;
using Poushec.Mikrotik.Exceptions;

namespace Poushec.Mikrotik.API.TCP
{
    internal class Session : IDisposable
    {
        private TcpClient _tcpClient;
        private Stream _connectStream;
        private SslStream _sslConnectStream;
        private bool _useSSL; 

        private Session(string ipAddress, int apiPort, bool useSsl = true, bool ignoreTikCert = true)
        {
            _useSSL = useSsl;
            _tcpClient = new TcpClient();

            try
            {
                _tcpClient.Connect(ipAddress, apiPort);
            }
            catch (SocketException ex)
            {
                throw new TikAPIException($"Could not establish connection with Mikrotik by port {apiPort}. Inner Exception: {ex.GetType()} : {ex.Message}");
            }

            if (_useSSL)
            {
                _sslConnectStream = new SslStream(
                    _tcpClient.GetStream(),
                    false,
                    ignoreTikCert ? new RemoteCertificateValidationCallback((a, b, c, d) => true) : null,
                    null
                );

                try
                {
                    _sslConnectStream.AuthenticateAsClient(ipAddress);
                }
                catch (System.Security.Authentication.AuthenticationException ex)
                {
                    throw new MikrotikCertificateInvalidException(ex.Message);
                }
            }
            else
            {
                _connectStream = _tcpClient.GetStream() as Stream;
            }
        }
        
        public static Session Open(MikrotikAPIConfig apiConfig) => new Session(apiConfig.IPAddress, apiConfig.APIPort, apiConfig.UseSSL, apiConfig.IgnoreTikCert);
        
        /// <summary>
        /// Closes TCP connection and Connect Stream
        /// </summary>
        public void Dispose()
        {
            if (_useSSL)
            {
                _sslConnectStream.Close();
            }
            else
            {
                _connectStream.Close();
            }

            _tcpClient.Close();
        }

        public void Send(string command, bool endsentence = false)
        {
            var connectStream = _useSSL ? _sslConnectStream : _connectStream;

            byte[] DataToSendasByte = Encoding.ASCII.GetBytes(command.ToCharArray());
            byte[] SendSize = EncodeLength(DataToSendasByte.Length);
            connectStream.Write(SendSize, 0, SendSize.Length);
            connectStream.Write(DataToSendasByte, 0, DataToSendasByte.Length);

            if (endsentence)
            {
                connectStream.WriteByte(0);
            }
        }

        public void Login(string username, string password)
        {
            Send("/login");
            Send("=name=" + username);
            Send("=password=" + password, true);

            if (Read()[0] != "!done")
            {
                throw new TikInvalidCredentialsException("Failed to log in. Check your credentials");   
            }
        }

        private byte[] EncodeLength(int delka)
        {
            if (delka < 0x80)
            {
                byte[] tmp = BitConverter.GetBytes(delka);
                return new byte[1] { tmp[0] };
            }
            if (delka < 0x4000)
            {
                byte[] tmp = BitConverter.GetBytes(delka | 0x8000);
                return new byte[2] { tmp[1], tmp[0] };
            }
            if (delka < 0x200000)
            {
                byte[] tmp = BitConverter.GetBytes(delka | 0xC00000);
                return new byte[3] { tmp[2], tmp[1], tmp[0] };
            }
            if (delka < 0x10000000)
            {
                byte[] tmp = BitConverter.GetBytes(delka | 0xE0000000);
                return new byte[4] { tmp[3], tmp[2], tmp[1], tmp[0] };
            }
            else
            {
                byte[] tmp = BitConverter.GetBytes(delka);
                return new byte[5] { 0xF0, tmp[3], tmp[2], tmp[1], tmp[0] };
            }
        }

        public List<string> Read()
        {
            var connectStream = _useSSL ? _sslConnectStream : _connectStream;

            List<string> output = new List<string>();
            string o = "";
            byte[] tmp = new byte[4];
            long count;
            while (true)
            {
                tmp[3] = (byte)connectStream.ReadByte();
                //if(tmp[3] == 220) tmp[3] = (byte)connectStream.ReadByte(); it sometimes happend to me that 
                //mikrotik send 220 as some kind of "bonus" between words, this fixed things, not sure about it though
                if (tmp[3] == 0)
                {
                    output.Add(o);
                    if (o.Substring(0, 5) == "!done")
                    {
                        break;
                    }
                    else
                    {
                        o = "";
                        continue;
                    }
                }
                else
                {
                    if (tmp[3] < 0x80)
                    {
                        count = tmp[3];
                    }
                    else
                    {
                        if (tmp[3] < 0xC0)
                        {
                            int tmpi = BitConverter.ToInt32(new byte[] { (byte)connectStream.ReadByte(), tmp[3], 0, 0 }, 0);
                            count = tmpi ^ 0x8000;
                        }
                        else
                        {
                            if (tmp[3] < 0xE0)
                            {
                                tmp[2] = (byte)connectStream.ReadByte();
                                int tmpi = BitConverter.ToInt32(new byte[] { (byte)connectStream.ReadByte(), tmp[2], tmp[3], 0 }, 0);
                                count = tmpi ^ 0xC00000;
                            }
                            else
                            {
                                if (tmp[3] < 0xF0)
                                {
                                    tmp[2] = (byte)connectStream.ReadByte();
                                    tmp[1] = (byte)connectStream.ReadByte();
                                    int tmpi = BitConverter.ToInt32(new byte[] { (byte)connectStream.ReadByte(), tmp[1], tmp[2], tmp[3] }, 0);
                                    count = tmpi ^ 0xE0000000;
                                }
                                else
                                {
                                    if (tmp[3] == 0xF0)
                                    {
                                        tmp[3] = (byte)connectStream.ReadByte();
                                        tmp[2] = (byte)connectStream.ReadByte();
                                        tmp[1] = (byte)connectStream.ReadByte();
                                        tmp[0] = (byte)connectStream.ReadByte();
                                        count = BitConverter.ToInt32(tmp, 0);
                                    }
                                    else
                                    {
                                        //Error in packet reception, unknown length
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < count; i++)
                {
                    o += (Char)connectStream.ReadByte();
                }
            }
            return output;
        }
    }
}