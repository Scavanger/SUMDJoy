using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace SUMDJoy
{

    public class SUMD : IDisposable
    {
        private const int BAUD_RATE = 115200;
        private const int MAX_CHANNELS = 16;
        private const int VENDOR_ID = 0xA8;
        private const int STATUS_VALID = 0x1;
        private const int STATUS_FAILSAFE = 0x81;
        private const ushort CRC_POLYNOME = 0x1021;
        private const int SUMD_HEADER_SIZE = 3;
        private const int BUFFER_SIZE = SUMD_HEADER_SIZE + MAX_CHANNELS * 2 + 2;

        private SerialPort _port;
        private TaskScheduler _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private CancellationTokenSource _cts;

        private byte[] _buffer;
        private int _sumdIndex = 0;
        private bool _frameDone = false;
        private ushort _crc = 0;

        public event EventHandler NewFrameRecieved;

        public int[] Channels { get; private set; }
        public IEnumerable<int> ChannelsUs
        {
            get
            {
                foreach (int c in Channels)
                    yield return c / 8;
            }
        }

        public bool IsFailsafe { get; private set; }
        public bool IsFrameValid { get; private set; }
        public int ChannelCount { get; private set; }
        public bool IsListening { get; private set; }

        public SUMD()
        {
            _cts = new CancellationTokenSource();
            _buffer = new byte[BUFFER_SIZE];

            IsFailsafe = false;
            IsFrameValid = false;
            ChannelCount = MAX_CHANNELS;
            Channels = new int[MAX_CHANNELS];
            IsListening = false;
        }

        public SUMD(string portName) : this()
        {
            if (string.IsNullOrWhiteSpace(portName))
                throw new ArgumentNullException("port");   

        }

        protected virtual void OnNewFrameRecieved()
        {
            if (NewFrameRecieved != null)
                NewFrameRecieved(this, new EventArgs());
        }

        public void SetSerialPort(string portName)
        {
            if (string.IsNullOrWhiteSpace(portName))
                throw new ArgumentNullException("port");

            if (_port == null)
                _port = new SerialPort(portName, BAUD_RATE);

            if (IsListening)
                StopListening();

            if (_port.IsOpen)
                _port.Close();

            _port = new SerialPort(portName, BAUD_RATE);
        }

        public void StartListening()
        {
            if (IsListening)
                return;

            if (_port == null)
                throw new InvalidOperationException("ComPort unavailable");

            try
            {
                if (!_port.IsOpen)
                    _port.Open();
            }
            catch (System.IO.IOException)
            {

                IsListening = false;
                return;
            }

            IsListening = true;
            _cts = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    try
                    {
                        int b = _port.BaseStream.ReadByte();
                        if (b == -1)
                            break;

                        SumdRecieve((byte)b);
                    }
                    catch (System.IO.IOException)
                    {
                        IsFrameValid = false;
                        _frameDone = false;
                    }

                    if (_frameDone)
                    {
                        SumdStatus();

                        Task.Factory.StartNew(
                            OnNewFrameRecieved,
                            CancellationToken.None,
                            TaskCreationOptions.None,
                            _scheduler)
                        .Wait();
                    }
                }

                IsListening = false;
            }, _cts.Token);
        }

        public void StopListening()
        {
            _cts.Cancel();
        }

        private void SumdRecieve(byte b)
        {
            if (_sumdIndex == 0)
            {
                if (b != VENDOR_ID)
                    return;
                else
                {
                    _frameDone = false;
                    _crc = 0;
                }
            }

            if (_sumdIndex == 2)
                ChannelCount = b;

            if (ChannelCount > MAX_CHANNELS)
                ChannelCount = MAX_CHANNELS;

            if (_sumdIndex < BUFFER_SIZE)
                _buffer[_sumdIndex] = b;

            _sumdIndex++;
            if (_sumdIndex <= CRCHighByteOffset(ChannelCount))
                CRC16(b);
            else if (_sumdIndex > CRCLowByteOffset(ChannelCount))
            {
                _sumdIndex = 0;
                _frameDone = true;
            }

        }

        private void SumdStatus()
        {
            switch (_buffer[1])
            {
                case STATUS_FAILSAFE:
                    IsFailsafe = true;
                    IsFrameValid = true;
                    break;
                case STATUS_VALID:
                    IsFrameValid = true;
                    IsFailsafe = false;
                    break;
                default:
                    IsFailsafe = false;
                    IsFrameValid = false;
                    break;
            }

            if (_crc != (_buffer[CRCHighByteOffset(ChannelCount)] << 8 | _buffer[CRCLowByteOffset(ChannelCount)]))
                IsFrameValid = false;

            for (int i = 1; i <= ChannelCount; i++)
                Channels[i - 1] = _buffer[ChannelHighByteOffet(i)] << 8 | _buffer[ChannelLowByteOffet(i)];
        }

        private void CRC16(byte value)
        {
            _crc = (ushort)(_crc ^ value << 8);
            for (int i = 0; i < 8; i++)
            {
                if ((_crc & 0x8000) > 0)
                    _crc = (ushort)((_crc << 1) ^ CRC_POLYNOME);
                else
                    _crc = (ushort)(_crc << 1);
            }
        }

        private static int ChannelHighByteOffet(int n)
        {
            return n * 2 + 1;
        }

        private static int ChannelLowByteOffet(int n)
        {
            return n * 2 + 2;
        }

        private static int CRCHighByteOffset(int n)
        {
            return (n + 1) * 2 + 1;
        }

        private static int CRCLowByteOffset(int n)
        {
            return (n + 1) * 2 + 2;
        }

        public void Dispose()
        {
            if (_port.IsOpen)
                _port.Close();
        }
    }
}
