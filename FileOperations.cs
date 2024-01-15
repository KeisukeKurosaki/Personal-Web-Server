/*
* FILE				: FileOperations.cs
* PROJECT			: PROG 2001 - Assignment 05 (myOwnWebServer)
* PROGRAMMERS		: Cody Glanville ID: 8864645
* FIRST VERSION		: November 19, 2023
* DESCRIPTION		:
*	This file is used in order carry out the file IO writing operations to the applications log file. As
*	such, each of the methods contained in this class are static and meant to be called without needing an
*	object of type FileOperations to be instantiated in order to use these methods.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace myOwnWebServer
{
    /*
    * INTERNAL CLASS        : FileOperations
    * DESCRIPTION	        :
    *	This class is used in order to write to the server log file. As such, each method is declared as public static in order 
    *	to allow these methods to be called without a class of this type needing to be instantiated. This allows these methods to be 
    *	called in multiple places due to the response and request methods being in separate classes (where the log file needs accessed).
    */
    internal class FileOperations
    {
        /*
        *	METHOD          : InitialLogCreation()
        *	DESCRIPTION		:
        *		This method is used in order to either overwrite or create a log file for the server to be able to log its operations 
        *		during its runtime. A try-catch block is used in the case of an error occuring during the access of this file.
        *		Ultimately, this log will indicate that the application itself has started, not technically the operation of the server.
        *   PARAMETERS      :
        *		void               :   Void is used as there are no return values for this method
        *	RETURNS			:
        *		void               :   Void is used as there are no return values for this method
        */
        public static void InitialLogCreation()
        {
            bool doesLogFileExist = File.Exists(Constants.kServerLogPath);

            try
            {
                using (StreamWriter writer = new StreamWriter(Constants.kServerLogPath, !doesLogFileExist))
                {
                    writer.WriteLine($"{DateTime.Now} [APPLICATION STARTED] - < Application started successfully >");
                }
            }
            catch (Exception)
            {
                CmdLineError.ServerLogError();
            }
        }

        /*
        *	METHOD          : ServerStarted()
        *	DESCRIPTION		:
        *		This method is used in order to indicate that the server has started running. As such, the server log file will be 
        *		accessed and the log will indicate that the server has started running. A try catch is used in case an error occurs with 
        *		accessing the log file.
        *   PARAMETERS      :
        *		void               :   Void is used as there are no return values for this method
        *	RETURNS			:
        *		void               :   Void is used as there are no return values for this method
        */
        public static void ServerStarted()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Constants.kServerLogPath, true))
                {
                    writer.WriteLine($"{DateTime.Now} [SERVER STARTED] - < Server started successfully >");
                }
            }
            catch (Exception)
            {
                CmdLineError.ServerLogError();
            }
        }

        /*
        *	METHOD          : RequestLog(string verb, string fileUsed)
        *	DESCRIPTION		:
        *		This method is used in order to indicate that the server has started running. As such, the server log file will be 
        *		accessed and the log will indicate that the server has started running. A try catch is used in case an error occurs with 
        *		accessing the log file.
        *   PARAMETERS      :
        *	    string verb               :   This is the method the client used to try and access a resource from the server
        *	    string fileUsed           :   This is the specific resource the client was wanting to access from the server
        *	RETURNS			:
        *		void                      :   Void is used as there are no return values for this method
        */
        public static void RequestLog(string verb, string fileUsed)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Constants.kServerLogPath, true))
                {
                    writer.WriteLine($"{DateTime.Now} [REQUEST] - < Verb: {verb} Resouce: {fileUsed} >");
                }
            }
            catch (Exception)
            {
                CmdLineError.ServerLogError();
            }
        }

        /*
        *	METHOD          : ResponseLogError(string statusCode)
        *	DESCRIPTION		:
        *		This method is used in order to write to the server log file when an error was encountered when determining the request
        *		from the client. As such, the specific error will be written into the server log along with a small description of the 
        *		error. A try-catch is used in case an error is encountered while accessing the log file.
        *   PARAMETERS      :
        *	    string statusCode         :   This is the particular status code that was encountered when determining a response for the client
        *	RETURNS			:
        *		void                      :   Void is used as there are no return values for this method
        */
        public static void ResponseLogError(string statusCode)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Constants.kServerLogPath, true))
                {
                    writer.WriteLine($"{DateTime.Now} [RESPONSE] - < {statusCode} - {Constants.HTTPDictionary[statusCode]}>");
                }
            }
            catch (Exception)
            {
                CmdLineError.ServerLogError();
            }
        }

        /*
        *	METHOD          : ResponseLogOK(string contentType, long contentLength)
        *	DESCRIPTION		:
        *		This method is used in order to write to the server log file when a response was able to be sucessfully determined for the client. As such, the
        *		log will indicate that is has logged a response, along with the content type of the resource and also the length of the resources content. A log of
        *		this type will only occur if a status code of 200 was determined through a strict validation process. A try-catch block is also used in case any 
        *		type of error occurs while accessing the server log file.
        *   PARAMETERS      :
        *	    string contentType        :   This is the content type of the specific file being output in the response from the server
        *	    long contentLength        :   This is the length of the resource that will be displayed as determined by the server
        *	RETURNS			:
        *		void                      :   Void is used as there are no return values for this method
        */
        public static void ResponseLogOK(string contentType, long contentLength)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Constants.kServerLogPath, true))
                {
                    writer.WriteLine($"{DateTime.Now} [RESPONSE] - < Content-Type: {contentType} Content-Length: {contentLength} Server: myOwnWebServer Date: {DateTime.Now} >");
                }
            }
            catch (Exception)
            {
                CmdLineError.ServerLogError();
            }
        }

        /*
        *	METHOD          : CriticalServerError()
        *	DESCRIPTION		:
        *		This method is used in order to write to the server log file when a critical error has caused the server to crash. This ultimately will
        *		allow for the log file to have a final log explaining to those that have access to the file that the server encountered a critical 
        *		error and has shut down. This method will also be used alongside output to the console window that the server is no longer running when the 
        *		exception is encountered.
        *   PARAMETERS      :
        *	    void                :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		void                :   Void is used as there are no return values for this method
        */
        public static void CriticalServerError()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Constants.kServerLogPath, true))
                {
                    writer.WriteLine($"{DateTime.Now} [CRITICAL SERVER ERROR] - < The server has crashed after encountering an unexpected error >");
                }
            }
            catch (Exception)
            {
                CmdLineError.ServerLogError();
            }
        }
    }
}
