using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Library_Management_System
{
    internal class Staff : Member
    {
        public Staff(string id, string name, string membershipType) : base(id, name, membershipType)
        {
           
        }

        public override int GetBorrowLimit(IDatabase db)
        {
            var limit = db.GetConfig("staff:borrowLimit");
            return limit != null ? int.Parse(limit) : 10;
        }
    }
}
