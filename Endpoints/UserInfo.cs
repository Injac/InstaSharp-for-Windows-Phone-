using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace InstaSharp {
    [DataContract]
    public class UserInfo {
        [DataMember]
        public int Id { get; set; }
         [DataMember]
        public string Username { get; set; }
         [DataMember]
        public string Full_Name { get; set; }
         [DataMember]
        public string Profile_Picture { get; set; }
    }
}
