using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SUMDJoy
{
    [DataContract]
    [KnownType(typeof(NoneAssingment))]
    [KnownType(typeof(AxeAssignment))]
    [KnownType(typeof(ButtonAssignment))]
    public abstract class Assignment
    {
        [DataMember]
        public abstract string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [DataContract]
    public class NoneAssingment : Assignment
    {
        [DataMember]
        public override string Name
        {
            get { return "None"; }
            set { }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ 667;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            NoneAssingment a = obj as NoneAssingment;
            if ((object)a == null)
                return false;

            return a.Name == Name;
        }

        public static bool operator ==(NoneAssingment a, NoneAssingment b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;

            return a.Name == b.Name;
        }

        public static bool operator !=(NoneAssingment a, NoneAssingment b)
        {
            return !(a == b);
        }
    }

    [DataContract]
    public class AxeAssignment : Assignment
    {
        private static readonly Dictionary<HID_USAGES, string> AXESNAMES = new Dictionary<HID_USAGES, string>()
        {
            { HID_USAGES.HID_USAGE_X, "X" },
            { HID_USAGES.HID_USAGE_Y, "Y" },
            { HID_USAGES.HID_USAGE_Z, "Z" },
            { HID_USAGES.HID_USAGE_RX, "RX" },
            { HID_USAGES.HID_USAGE_RY, "RY" },
            { HID_USAGES.HID_USAGE_RZ, "RZ" },
            { HID_USAGES.HID_USAGE_SL0, "Slider" },
            { HID_USAGES.HID_USAGE_SL1, "Dial/Slider2" },
            { HID_USAGES.HID_USAGE_WHL, "Wheel" },
            { HID_USAGES.HID_USAGE_POV, "POV Hat Switch" }

        };

        public AxeAssignment(HID_USAGES axe)
        {
            Axe = axe;
        }

    
        [DataMember]
        public HID_USAGES Axe { get; set; }

        [DataMember]
        public override string Name
        {
            get { return AXESNAMES[Axe]; }
            set { }
        }

        public static string GetAxeName(HID_USAGES axe)
        {
            return AXESNAMES[axe];
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            AxeAssignment a = obj as AxeAssignment;
            if ((object)a == null)
                return false;

            return a.Axe == Axe && a.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Axe.GetHashCode();
        }

        public static bool operator ==(AxeAssignment a, AxeAssignment b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;

            return a.Axe == b.Axe && a.Name == b.Name;
        }

        public static bool operator !=(AxeAssignment a, AxeAssignment b)
        {
            return !(a == b);
        }
    }

    [DataContract]
    public class ButtonAssignment : Assignment
    {
        [DataMember]
        public int ButtonNumber { get; set; }

        [DataMember]
        public override string Name
        {
            get { return "Button " + ButtonNumber; }
            set { }
        }

        public ButtonAssignment(int buttonNo)
        {
            ButtonNumber = buttonNo;
        }

        public static string GetButtonName(int buttonNo)
        {
            return "Button " + buttonNo;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            ButtonAssignment a = obj as ButtonAssignment;
            if ((object)a == null)
                return false;

            return a.ButtonNumber == ButtonNumber && a.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ ButtonNumber.GetHashCode();
        }

        public static bool operator ==(ButtonAssignment a, ButtonAssignment b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (((object)a == null) || ((object)b == null))
                return false;

            return a.ButtonNumber == b.ButtonNumber && a.Name == b.Name;
        }

        public static bool operator !=(ButtonAssignment a, ButtonAssignment b)
        {
            return !(a == b);
        }
    }
}
