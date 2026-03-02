using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal class Member : Person
    {
        public string MembershipType { get; }
        public Member(string id, string name, string membershipType) : base(id, name)
        {
            MembershipType = membershipType;
        }
    }
}
