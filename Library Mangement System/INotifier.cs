using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Management_System
{
    internal interface INotifier
    {
        void Notify(string message);
    }

    // Prints to the console
    internal class ConsoleNotifier : INotifier
    {
        public void Notify(string message)
            => Console.WriteLine($"\n  >> {message}");
    }

    // Writes to a text file
    internal class FileNotifier : INotifier
    {
        private readonly string _filePath;

        public FileNotifier(string filePath)
        {
            _filePath = filePath;
        }

        public void Notify(string message)
            => File.AppendAllText(_filePath, $"[{DateTime.Now}] {message}\n");
    }


}
