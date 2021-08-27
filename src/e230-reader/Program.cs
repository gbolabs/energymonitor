using System;
using System.IO.Ports;

namespace e230_reader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("List SerialPorts");
            foreach (var spn in SerialPort.GetPortNames())
            {
                Console.WriteLine("\t" + spn);
            }
            Console.WriteLine("Trying to create SerialPort");
            SerialPort sp = new SerialPort();

        }
    }
}
