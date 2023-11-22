/*
* FILE				: CommandValidation.cs
* PROJECT			: PROG 2001 - Assignment 05 (myOwnWebServer)
* PROGRAMMERS		: Cody Glanville ID: 8864645
* FIRST VERSION		: November 19, 2023
* DESCRIPTION		:
*	This file is used in order to contain the validation methods used for the verifying the command-line arguments
*	the user will enter to properly run the application. Methods in this file will determine if the user has properly
*	implemented the adequate prefix for their arguments (Ex. -webRoot=) and will also determine if the values of these 
*	arguments can be used in order to run the server. During the validation process, properties will be updated that will
*	be accessed upon the server starting in order to run the server on the desired IP address and port.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Reflection;

namespace myOwnWebServer
{
    /*
    * PUBLIC INTERNAL CLASS : CommandValidation
    * DESCRIPTION	        :
    *	This class is used in order to properly validate the command-line arguments that are used for the application
    *	pertaining to their formatting and also the values each argument will hold and be required for throughout the
    *	program. Properties will be accessed in order to obtain the values of the IP address and port for the server 
    *	after validation has been completed.
    */
    public class CommandValidation
    {
        private string[] parsedCommandLine;

        public string[] _ParsedCommandLine
        {
            get { return parsedCommandLine; }
            set { parsedCommandLine = value; }
        }

        public IPAddress _SelectedAddress { get; set; }

        public int _SelectedPort { get; set; }

        public string _WebsiteData { get; set; }

        public string _AddressString { get; set; }


        public CommandValidation() 
        {
            parsedCommandLine = new string[3];
        }

        /*
        *	Method          : CommandLineParse(string[] commandLineArgs)
        *	DESCRIPTION		:
        *		This method acts in order to parse the command-line arguments the user will have entered into the program in order to recieve the
        *		actual value of each arument (instead of it being combined with its prefix (Ex. -webPort=5300). Each argument is individually 
        *		checked to see if it can be parsed correctly. If not, the proper error is displayed to the user in order to let them know
        *		which area is causing the application to not continue.
        *   PARAMETERS      :
        *		string commandLineArgs     :   These are the command-line arguments that were entered into the program
        *	RETURNS			:
        *		bool true	               :   True is returned if the arguments contained their proper prefix
        *		bool false                 :   False is returned if ANY of the arguments did not have their proper prefix
        */
        public bool CommandLineParse(string[] commandLineArgs)
        {
            string checkedRoot;
            string checkedIP;
            string checkedPort;

            int i = 0;

            if (commandLineArgs[0].StartsWith(Constants.kRootPrefix))
            {
                checkedRoot = commandLineArgs[0].Substring(Constants.kRootPrefix.Length);
                parsedCommandLine[0] = checkedRoot;
            }
            else
            {
                CmdLineError.RootDefineError();
                i++;
            }

            if (commandLineArgs[1].StartsWith(Constants.kIPPrefix))
            {
                checkedIP = commandLineArgs[1].Substring(Constants.kIPPrefix.Length);
                parsedCommandLine[1] = checkedIP;
            }
            else
            {
                CmdLineError.IPDefineError();
                i++;
            }

            if (commandLineArgs[2].StartsWith(Constants.kPortPrefix))
            {
                checkedPort = commandLineArgs[2].Substring(Constants.kPortPrefix.Length);
                parsedCommandLine[2] = checkedPort;
            }
            else
            {
                CmdLineError.PortDefineError();
                i++;
            }

            if (i != 0)
            {
                return false;
            }

            return true;        // Only true if we have 3 values that contain the proper prefixes
        }

        /*
        *	Method          : ValueChecks(string[] parsedCommandArgs)
        *	DESCRIPTION		:
        *		This method acts to validate the parsed data that was recieved from the command-line arguments. The parsing was used in order to remove the 
        *		prefixes used to indicate what each argument was used for. Now, each of the separate arguments will undergo a validation to ensure it is a 
        *		proper value that can be used in the program. If the value can be used, a property will be updated to reflect its valid status. Otherwise, the
        *		method will ultimately return false, indicating that the program cannot continue with the invalid values.
        *   PARAMETERS      :
        *		string[] parsedCommandArgs           :   These are the parsed command-line arguments without their prefixes
        *	RETURNS			:
        *		bool true	                         :   True is returned if the values are ALL valid
        *		bool false                           :   False is returned if ANY of the values are invalid
        */
        public bool ValueChecks(string[] parsedCommandArgs)
        {
            int h = 0;

            if (!StorageFileCheck(parsedCommandArgs[0]))
            {
                CmdLineError.InvalidFolderPath();
                h++;
            }
            else
            {
                _WebsiteData = parsedCommandArgs[0];
            }
            
            if (!IPAddressCheck(parsedCommandArgs[1]))
            {
                CmdLineError.InvalidIP();
                h++;
            }
            else
            {
                _AddressString = parsedCommandArgs[1];
                IPAddress addressBridge;
                IPAddress.TryParse(parsedCommandArgs[1], out addressBridge);
                _SelectedAddress = addressBridge;
            }

            if (!PortCheck(parsedCommandArgs[2]))
            {
                CmdLineError.InvalidPort();
                h++;
            }
            else
            {
                int portBridge;
                int.TryParse(parsedCommandArgs[2], out portBridge);
                _SelectedPort = portBridge;
            }

            if (h != 0)
            {
                return false;
            }

            return true;
        }

        /*
        *	Method          : IPAddressCheck(string inputIP)
        *	DESCRIPTION		:
        *		This method acts in order to validate the server IP address the user will enter into the program. There are a variety of checks this 
        *		value will need to pass through, each of which need to be passed in order to use it in the program. As such, this method will go through this 
        *		logic and will return a bool indicating the status of the validity of the IP.
        *   PARAMETERS      :
        *		string inputIP             :   This is the IP address that will be validated
        *	RETURNS			:
        *		bool true	               :   True is returned if the IP address is valid
        *		bool false                 :   False is returned if the IP address is invalid
        */
        private bool IPAddressCheck(string inputIP)
        {
            // First check if we have a null field or it is only whitespace

            if (string.IsNullOrWhiteSpace(inputIP))
            {
                return false;
            }

            // Split the user-entered string into pieces using a '.' as a delimiter to determine if there is exactly 4 separate numbers separated by a '.'

            string[] pieces = inputIP.Split('.');

            if (pieces.Length != 4)
            {
                return false;
            }

            // Now determine if each small piece is a valid integer value and also between 0-255

            foreach (string piece in pieces)
            {
                int pieceValue;

                if (!int.TryParse(piece, out pieceValue))
                {
                    return false;
                }

                if (pieceValue < 0 || pieceValue > 255)
                {
                    return false;
                }
            }

            // Tests have completed, if all passed, we have a valud value to use for our user's IP
            return true;
        }

        /*
        *	Method          : PortCheck(string inputPort)
        *	DESCRIPTION		:
        *		This method acts in order to validate the port input the user will enter into the application. As such, it must be an integer number
        *		and within a valid range. If the number passes the validation, it can be properly used for the application to continue. 
        *   PARAMETERS      :
        *		string inputPort           :   This is the number that will be validated
        *	RETURNS			:
        *		bool true	               :   True is returned if the port is valid
        *		bool false                 :   False is returned if the port is invalid
        */
        private bool PortCheck(string inputPort)
        {
            int result;

            if (!int.TryParse(inputPort, out result))
            {
                return false;
            }

            if (result >= 0 && result <= 65535)
            {
                return true;
            }

            return false;
        }

        /*
        *	Method          : StorageFileCheck(string fileInput)
        *	DESCRIPTION		:
        *		This method acts in order to determine if a folder directory exists. It will take the argument from its parameter and run a quick check
        *		to see whether it can be found or not and return the proper status indicating the result.
        *   PARAMETERS      :
        *		string folderPath          :   This is the path to the directory to be searched for
        *	RETURNS			:
        *		bool true	               :   True is returned if the directory is found
        *		bool false                 :   False is returned if the directory is not found
        */
        private bool StorageFileCheck(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return false;
            }

            return true;
        }

        /*
        *	Method          : ArgumentCountCheck(int argCount)
        *	DESCRIPTION		:
        *		This method is used in order to determine if a user has entered a valid amount of command-line arguments into the 
        *		application. If the user did not enter the exact amount of arguments needed, the user will be informed of their 
        *		error and they will be prompted to exit the application and try again.
        *   PARAMETERS      :
        *		int argCount          :   This is the path to the directory to be searched for
        *	RETURNS			:
        *		void                  :   Void is used as there is no return value for this method            
        */
        public void ArgumentCountCheck(int argCount)
        {
            if (argCount == Constants.kArgsMin || argCount > Constants.kArgsMax)          // Check the count of command-line arguments
            {
                CmdLineError.ArgumentCountError();
            }
        }
    }
}
