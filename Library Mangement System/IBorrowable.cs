using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement_System
{
    public interface IBorrowable
    {
         void Borrow(string memberId);
        void Return();
    }
}
