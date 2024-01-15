/*
* FILE				: Driver.cs
* PROJECT			: PROG 2001 - Assignment 05 (myOwnWebServer)
* PROGRAMMERS		: Cody Glanville ID: 8864645
* FIRST VERSION		: November 19, 2023
* DESCRIPTION		:
*	This file is used in order to hold the main program for launching my personally created web server. As such, 
*	the logic for running the web server will be found in this file. The server will act in order to listen for incoming
*	requests from clients and then manually parse their requests and respond to them accordingly with the proper 
*	HTTP response protocol that is required. This server will only serve one client request at a time as it was 
*	created to be single-threaded.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;

namespace myOwnWebServer
{
    /*
    * INTERNAL CLASS        : Driver
    * DESCRIPTION	        :
    *	This class is used in order to hold the Main() that will help run our server program. As such, validation is carried out on what was entered into
    *	the command-line and then a TcpListener is utilized in order to create a single-threaded server that will handle one client request at a time.
    */
    internal class Driver
    {
        static void Main(string[] args)
        {
            FileOperations.InitialLogCreation();                                   // Determines if the .log file needs to be created or overwritten
          
            CommandValidation userInput = new CommandValidation();

            userInput.ArgumentCountCheck(args.Count());                            // Check the count of arguments before proceeding

            bool canContinue = userInput.CommandLineParse(args);                   // Parses the command-line arguments to get their actual values

            if (!canContinue)
            {
                CmdLineError.BasicErrorClosing();
            }

            if (!userInput.ValueChecks(userInput._ParsedCommandLine))              // Access each specific command-line value (without prefix ie. -webRoot)
            {
                CmdLineError.BasicErrorClosing();
            }

            TcpListener listener = new TcpListener(userInput._SelectedAddress, userInput._SelectedPort);    // Create a new TcpListener for the server

            try
            {
                listener.Start();                   // Start the server
                FileOperations.ServerStarted();     // Make a new server log entry to indicate the server starting successfully

                while (true)                        // Start and run the server in a while loop
                {
                    RequestHandler clientHandler = new RequestHandler(userInput._AddressString, userInput._WebsiteData, userInput._SelectedPort.ToString());

                    TcpClient client = listener.AcceptTcpClient();

                    clientHandler.RequestParser(client);            // Take in a request and validate it

                    ResponseHandler responseHandler = new ResponseHandler(client, clientHandler.usableFile, clientHandler.StatusCode.ToString(), clientHandler.contentType, userInput._WebsiteData);

                    responseHandler.ResponseSender();               // Send out a response to the client

                    client.Close();                                 // Close the connection to the client
                }
            }
            catch (Exception)                                       // Utilized if the server crashes unexpectedly 
            {
                Console.WriteLine("CRITICAL SERVER ERROR: The server has crashed...");
                FileOperations.CriticalServerError();
                CmdLineError.BasicErrorClosing();
            }
        }
    }
}
