using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace SUMDJoy
{
    public class SUMDToVJoy
    {
        private const int MAX_POS = 0x1c20;
        private const int MIN_POS = 0x41a0;


        private SUMD _sumd;
        private vJoy _joystick;
        private vJoy.JoystickState _joystickState;
        private int _vJoyCurrentDevice;
        private long _maxValue = 0;

        public event EventHandler NewFrameRecieved;

        public bool IsvJoyEnabled { get { return _joystick.vJoyEnabled(); } }
        public string vJoyInfo { get { return string.Format("{0} V: {1}", _joystick.GetvJoyProductString(), _joystick.GetvJoySerialNumberString()); } }

        //public bool IsAssignmentCorrect
        //{
        //    get
        //    {
        //        return !(AxeAssignment.Values.FindDuplicates().Any() || ButtonAssignment.Values.FindDuplicates().Any());
        //    }
        //}

        public string[] SerialPorts { get { return GetComPorts(); } }

        public uint[] FreevJoyDevices
        {
            get
            {
                return vJoyDevices
                    .Where(d => d.Value == VjdStat.VJD_STAT_FREE)
                    .ToDictionary(i => i.Key, i => i.Value)
                    .Keys
                    .ToArray();
            }
        }

        public bool SUMDStatus { get { return _sumd.IsFrameValid; } }
        public bool DriverMatch { get; private set; }
        public uint vJoyDevice { get; set; }
        public Dictionary<uint, VjdStat> vJoyDevices { get; private set; }
        public Dictionary<Assignment, int> Assignments { get; set; }

        public int vJoyCurrentDevice
        {
            get { return _vJoyCurrentDevice; }
            set
            {
                if (value <= 0 || value > 16)
                    throw new ArgumentOutOfRangeException();

                _vJoyCurrentDevice = value;
            }
        }

        public SUMDToVJoy()
        {
            _sumd = new SUMD();
            _sumd.NewFrameRecieved += Sumd_NewFrameRecieved;
            _joystick = new vJoy();
            _joystickState = new vJoy.JoystickState();
            vJoyDevices = new Dictionary<uint, VjdStat>();
            Assignments = new Dictionary<Assignment, int>();

            for (uint i = 1; i <= 16; i++)
                vJoyDevices.Add(i, _joystick.GetVJDStatus(i));

            uint DllVer = 0, DrvVer = 0;
            DriverMatch = _joystick.DriverMatch(ref DllVer, ref DrvVer);
        }


        protected virtual void OnNewFrameRecieved()
        {
            if (NewFrameRecieved != null)
                NewFrameRecieved(this, new EventArgs());
        }

        public void SetComPort(string port)
        {
            _sumd.SetSerialPort(port);
        }

        public void Start()
        {
            _sumd.StartListening();
        }

        public void Stop()
        {
            _sumd.StopListening();
        }

        public void GetvJoyInfos()
        {
            Assignments.Clear();

            Assignments.Add(new NoneAssingment(), 0);

            if (_joystick.GetVJDAxisExist(vJoyDevice, HID_USAGES.HID_USAGE_X))
                Assignments.Add(new AxeAssignment(HID_USAGES.HID_USAGE_X), 1);

            if (_joystick.GetVJDAxisExist(vJoyDevice, HID_USAGES.HID_USAGE_Y))
                Assignments.Add(new AxeAssignment(HID_USAGES.HID_USAGE_Y), 2);

            if (_joystick.GetVJDAxisExist(vJoyDevice, HID_USAGES.HID_USAGE_Z))
                Assignments.Add(new AxeAssignment(HID_USAGES.HID_USAGE_Z), 3);

            if (_joystick.GetVJDAxisExist(vJoyDevice, HID_USAGES.HID_USAGE_RX))
                Assignments.Add(new AxeAssignment(HID_USAGES.HID_USAGE_RX), 4);

            if (_joystick.GetVJDAxisExist(vJoyDevice, HID_USAGES.HID_USAGE_RY))
                Assignments.Add(new AxeAssignment(HID_USAGES.HID_USAGE_RY), 0);

            if (_joystick.GetVJDAxisExist(vJoyDevice, HID_USAGES.HID_USAGE_RZ))
                Assignments.Add(new AxeAssignment(HID_USAGES.HID_USAGE_RZ), 0);

            if (_joystick.GetVJDAxisExist(vJoyDevice, HID_USAGES.HID_USAGE_SL0))
                Assignments.Add(new AxeAssignment(HID_USAGES.HID_USAGE_SL0), 0);

            if (_joystick.GetVJDAxisExist(vJoyDevice, HID_USAGES.HID_USAGE_SL1))
                Assignments.Add(new AxeAssignment(HID_USAGES.HID_USAGE_SL1), 0);

            //if (_joystick.GetVJDAxisExist(vJoyDevice, HID_USAGES.HID_USAGE_WHL))
            //    Assignments.Add(new AxeAssignment(HID_USAGES.HID_USAGE_WHL));

            //if (_joystick.GetVJDAxisExist(vJoyDevice, HID_USAGES.HID_USAGE_POV))
            //    Assignments.Add(new AxeAssignment(HID_USAGES.HID_USAGE_POV));

            for (int i = 1; i <= _joystick.GetVJDButtonNumber(vJoyDevice); i++)
                Assignments.Add(new ButtonAssignment(i), i + 4);

            _joystick.GetVJDAxisMax(vJoyDevice, HID_USAGES.HID_USAGE_X, ref _maxValue);
        }

        private void Sumd_NewFrameRecieved(object sender, EventArgs e)
        {
            Task.Factory.StartNew(
                            OnNewFrameRecieved,
                            CancellationToken.None,
                            TaskCreationOptions.None,
                            TaskScheduler.FromCurrentSynchronizationContext())
                        .Wait();


            _joystickState.bDevice = (byte)vJoyDevice;

            foreach (var assignment in Assignments)
            {
                if (assignment.Key is AxeAssignment)
                {
                    int axis = assignment.Value - 1;
                    if (axis < 0)
                        continue;

                    if (assignment.Key.Name == AxeAssignment.GetAxeName(HID_USAGES.HID_USAGE_X))
                        _joystickState.AxisX = CalcStickPos(_sumd.Channels[axis]);

                    if (assignment.Key.Name == AxeAssignment.GetAxeName(HID_USAGES.HID_USAGE_Y))
                        _joystickState.AxisY = CalcStickPos(_sumd.Channels[axis]);

                    if (assignment.Key.Name == AxeAssignment.GetAxeName(HID_USAGES.HID_USAGE_Z))
                        _joystickState.AxisZ = CalcStickPos(_sumd.Channels[axis]);

                    if (assignment.Key.Name == AxeAssignment.GetAxeName(HID_USAGES.HID_USAGE_RX))
                        _joystickState.AxisXRot = CalcStickPos(_sumd.Channels[axis]);

                    if (assignment.Key.Name == AxeAssignment.GetAxeName(HID_USAGES.HID_USAGE_RY))
                        _joystickState.AxisYRot = CalcStickPos(_sumd.Channels[axis]);

                    if (assignment.Key.Name == AxeAssignment.GetAxeName(HID_USAGES.HID_USAGE_RZ))
                        _joystickState.AxisZRot = CalcStickPos(_sumd.Channels[axis]);

                    if (assignment.Key.Name == AxeAssignment.GetAxeName(HID_USAGES.HID_USAGE_SL0))
                        _joystickState.Slider = CalcStickPos(_sumd.Channels[axis]);

                    if (assignment.Key.Name == AxeAssignment.GetAxeName(HID_USAGES.HID_USAGE_SL1))
                        _joystickState.Dial = CalcStickPos(_sumd.Channels[axis]);

                }
                else if (assignment.Key is ButtonAssignment)
                {
                    ButtonAssignment button = assignment.Key as ButtonAssignment;

                    int buttonChannel = assignment.Value - 1;
                    if (buttonChannel < 0)
                        continue;

                    if (_sumd.Channels[buttonChannel] > 0x2ee0)
                        _joystickState.Buttons |= (uint)(1 << button.ButtonNo - 1);
                    else
                        _joystickState.Buttons &= ~((uint)(1 << button.ButtonNo - 1));

                }
            }

            _joystick.UpdateVJD(vJoyDevice, ref _joystickState);
            _joystick.AcquireVJD(vJoyDevice);


        }

        private int CalcStickPos(int pos)
        {
            double a = _maxValue / (MAX_POS - MIN_POS);
            double bb = -a * MIN_POS;

            return (int)(pos * a + bb);
        }


        public static string[] GetComPorts()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }
    }


}
