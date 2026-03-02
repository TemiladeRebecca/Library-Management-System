using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal class Librarian : Person
    {
        public string EmployeeCode { get; }
        public Librarian(string id, string name, string employeeCode) : base(id, name)
        {
            EmployeeCode = employeeCode;
        }
        public override string GetDetails()
        {
            return $"[Librarian] {Name} | Employee: {EmployeeCode} | ID: {Id}";
        }
    }
}
