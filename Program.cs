/*
* FILE				: program.cs
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

namespace myOwnWebServer
{
    internal class Program
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

            TcpListener listener = new TcpListener(userInput._SelectedAddress, userInput._SelectedPort);    // Create a new Tcp Listener for the server




            try
            {
                listener.Start();                   // start the server
                FileOperations.ServerStarted();     // Make server log to indicate the server start

                while (true)                        // Start server in a while loop :)
                {
                    RequestHandler clientHandler = new RequestHandler(userInput._AddressString, userInput._WebsiteData, userInput._SelectedPort.ToString());

                    TcpClient client = listener.AcceptTcpClient();

                    clientHandler.RequestParser(client);

                    ResponseHandler responseHandler = new ResponseHandler(client, clientHandler.usableFile, clientHandler.StatusCode, clientHandler.contentType, userInput._WebsiteData);

                    responseHandler.ResponseSender();
                    client.Close();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Server blew up... :(");
            }
        }
    }
}
