/*
* FILE				: CmdLineError.cs
* PROJECT			: PROG 2001 - Assignment 05 (myOwnWebServer)
* PROGRAMMERS		: Cody Glanville ID: 8864645
* FIRST VERSION		: November 19, 2023
* DESCRIPTION		:
*	While a server should not show direct output to the command-line in applications such as these, in our scenario
*	we will still show output to the console depending on if the user has entered unacceptable values. As such, 
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
    /*
    * INTERNAL CLASS        : CmdLineError()
    * DESCRIPTION	        :
    *	This class is used in order to display output to the console if an error is encountered in the command-line arguments used for the application.
    *	As such, if an error is encountered in any of the 3 needed arguments (or more arguments were provided), these methods will display the proper 
    *	error message to the user. While some of these methods only contain one line of output, we still keep the majority of the output to the console
    *	relating to command-line validation in this one file.
    */
    internal class CmdLineError
    {
        /*
        *	METHOD          : ServerLogError()
        *	DESCRIPTION		:
        *		This method is used when an error occurs in accessing the server log file. As such, an error is output to the console describing
        *		that the log file could not be accessed or written to.
        *   PARAMETERS      :
        *		void                 :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		void                 :   Void is used as there are no return values for this method
        */
        public static void ServerLogError()
        {
            Console.WriteLine("ERROR: Could not access / write to the server log file...");
            BasicErrorClosing();
        }

        /*
        *	METHOD          : ArgumentCountError()
        *	DESCRIPTION		:
        *		This method is used when a user inputs an invalid amount of command-line arguments into the application. As such, they will be notified
        *		of there error and they will be prompted to exit the program through a button press.
        *   PARAMETERS      :
        *		void                 :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		void                 :   Void is used as there are no return values for this method
        */
        public static void ArgumentCountError()
        {
            Console.WriteLine("ERROR: Invalid amount of command-line arguments. Please try again...");
            BasicErrorClosing();
        }

        /*
        *	METHOD          : RootDefineError()
        *	DESCRIPTION		:
        *		This method is used to output an error to the console window when a user does not preceed the folder path the server will draw
        *		its resources from with '-webRoot='. As such, they are notified of their error and prompted to try again.
        *   PARAMETERS      :
        *		void                 :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		void                 :   Void is used as there are no return values for this method
        */
        public static void RootDefineError()
        {
            Console.WriteLine("ERROR: Please preceed the folder path's argument with '-webRoot=' and try again...");
        }

        /*
        *	METHOD          : IPDefineError()
        *	DESCRIPTION		:
        *		This method is used to output an error statement to the console window when a user does not preceed the IP address on their
        *		argument input with '-webIP='. As such, they are notified of their error and prompted to exit the program and try again.
        *   PARAMETERS      :
        *		void                 :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		void                 :   Void is used as there are no return values for this method
        */
        public static void IPDefineError()
        {
            Console.WriteLine("ERROR: Please preceed the IP address's argument with '-webIP=' and try again...");
        }

        /*
        *	METHOD          : PortDefineError()
        *	DESCRIPTION		:
        *		This method is used to output an error statement to the console window when a user does not preceed the server port of their
        *		argument input with '-webPort='. As such, they are notified of their error and prompted to exit the program and try again.
        *   PARAMETERS      :
        *		void                 :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		void                 :   Void is used as there are no return values for this method
        */
        public static void PortDefineError()
        {
            Console.WriteLine("ERROR: Please preceed the port's argument with '-webPort=' and try again...");
        }

        /*
        *	METHOD          : InvalidPort()
        *	DESCRIPTION		:
        *		This method is used to output an error statement to the console window when an invalid port value was used as an argument. As such, the
        *		user is prompted of their error to notify then of the mistake.
        *   PARAMETERS      :
        *		void                 :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		void                 :   Void is used as there are no return values for this method
        */
        public static void InvalidPort()
        {
            Console.WriteLine("ERROR: An invalid port value was used on the command-line...");
        }

        /*
        *	METHOD          : InvalidFolderPath()
        *	DESCRIPTION		:
        *		This method is used when a user has input an invalid folder path that does not exist into the command-line as an arugment for the
        *		server application. As such, they are notified of their mistake upon which they will also have to exit the program and try again with
        *		a proper folder path.
        *   PARAMETERS      :
        *		void                 :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		void                 :   Void is used as there are no return values for this method
        */
        public static void InvalidFolderPath()
        {
            Console.WriteLine("ERROR: An invalid folder path was used on the command-line...");
        }

        /*
        *	METHOD          : InvalidIP()
        *	DESCRIPTION		:
        *		This method is used when a user has input an invalid server IP address as an argument on the command-line. As such, they will be notified of 
        *		their error and will need to try again with a valid IP address in order to start the server application.
        *   PARAMETERS      :
        *		void                 :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		void                 :   Void is used as there are no return values for this method
        */
        public static void InvalidIP()
        {
            Console.WriteLine("ERROR: An invalid IP address was used on the command-line... ");
        }

        /*
       *	METHOD          : BasicErrorClosing()
       *	DESCRIPTION		:
       *		This method is used when an invalid value has been used on the command-line and the program needs to be closed. As such, all that is used in 
       *		this method is a prompt to press any button to exit, upon which a ReadKey() is used to then exit the program.
       *   PARAMETERS      :
       *		void                 :   Void is used as there are no parameters for this method
       *	RETURNS			:
       *		void                 :   Void is used as there are no return values for this method
       */
        public static void BasicErrorClosing()
        {
            Console.WriteLine("Press any button to exit...");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
