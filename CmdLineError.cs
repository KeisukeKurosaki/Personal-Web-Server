/*
* FILE				: CmdLineError.cs
* PROJECT			: PROG 2001 - Assignment 05 (myOwnWebServer)
* PROGRAMMERS		: Cody Glanville ID: 8864645
* FIRST VERSION		: November 19, 2023
* DESCRIPTION		:
*	While a server should not show direct output to the command-line in applications such as these, in our scenario
*	we will still show output to the command-line depending on if the user has entered acceptable values. As such, 
*	this file will hold the methods that will output error messages to the command-line for the user to see and 
*	understand where they may have input incorrect values during their usage of the application.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace myOwnWebServer
{
    internal class CmdLineError
    {
        public static void ServerLogError()
        {
            Console.WriteLine("ERROR: Could not access / write to the server log file...");
            Console.WriteLine("Press any button to exit");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static void ArgumentCountError()
        {
            Console.WriteLine("ERROR: Invalid count of command-line arguments. Please try again...");
            Console.WriteLine("Press any button to exit");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static void RootDefineError()
        {
            Console.WriteLine("ERROR: Please preceed the folder path's argument with '-webRoot=' and try again...");
        }

        public static void IPDefineError()
        {
            Console.WriteLine("ERROR: Please preceed the IP address's argument with '-webIP=' and try again...");
        }

        public static void PortDefineError()
        {
            Console.WriteLine("ERROR: Please preceed the port's argument with '-webPort=' and try again...");
        }

        public static void InvalidPort()
        {
            Console.WriteLine("ERROR: An invalid port value was used on the command-line...");
        }
        public static void InvalidFolderPath()
        {
            Console.WriteLine("ERROR: An invalid folder path was used on the command-line...");
        }

        public static void InvalidIP()
        {
            Console.WriteLine("ERROR: An invalid IP address was used on the command-line... ");
        }

        public static void BasicErrorClosing()
        {
            Console.WriteLine("Press any button to exit");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
