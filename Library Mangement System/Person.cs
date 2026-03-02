using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal class Person
    {
        private readonly string _id;
        public string Id => _id;
        public string Name { get; private set; }
        public Person(string id, string name)
        {
            _id = name;
            Name = name;
        }
        public virtual string GetDetails()
        {
            return $"[Person] {Name} (ID: {Id})";
        }

    }
}
