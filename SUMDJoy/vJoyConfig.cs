using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SUMDJoy
{
    [DataContract(Name = "SUMDvJoyConfig")]
    public class VJoyConfig
    {
        [DataMember]
        public string COMPort { get; set; }
        [DataMember]
        public uint vJoyDevice { get; set; }
        [DataMember]
        public Dictionary<Assignment, int> Assignments { get; set; }

        public VJoyConfig()
        {
            Assignments = new Dictionary<Assignment, int>();
        }
    }
}
