/*
* FILE				: Constants.cs
* PROJECT			: PROG 2001 - Assignment 05 (myOwnWebServer)
* PROGRAMMERS		: Cody Glanville ID: 8864645
* FIRST VERSION		: November 19, 2023
* DESCRIPTION		:
*	This file is used in order to hold the constants used throughout the program. As such, it will hold
*	values relating to magic numbers that may need to be changed in multiple places in the source code of 
*	the assignment. The path for the server log file is also contained here because it will always be the same.
*	Other constants relate to the HTTP response and request messaging protocols. 
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
    * STATIC INTERNAL CLASS: Constants
    * DESCRIPTION	       :
    *	This class is used for the storage of constants that will be used throughout the application. These constants relate 
    *	to status codes, the number of command-line arguments required, and the format of the command-line arguments as well. 
    *	A read-only dictionary is also utilized in order to access the descriptions of the error codes that are utilized for 
    *	the server and content type string arrays are used for certain file families.
    */
    static internal class Constants
    {

        // Used for accessing the log file

        public const string kServerLogPath = "myOwnWebServer.log";

        // Command-line argument count max / min

        public const int kArgsMin = 0;
        public const int kArgsMax = 3;

        // Determines success / failure of accessing a resource

        public const string kOutputDecider = "200";

        // Command-line formatting for specific server areas

        public const string kRootPrefix = "-webRoot=";
        public const string kIPPrefix = "-webIP=";
        public const string kPortPrefix = "-webPort=";

        // String arrays holding the extension types of related file types

        public static readonly string[] kHTMLFamily = { ".htm", ".html", ".htmls", ".htx", "shtml", ".acgi" };
        public static readonly string[] KJPGFamily = { ".jfif", ".jfif-tbnl", ".jpeg", ".jpg" };

        // File families with only one type of extension that will be used (txt and gif files)

        public const string kGifExt = ".gif";
        public const string kTextExt = ".txt";

        // Content Types

        public const string kContentTypeHTML = "text/html";
        public const string kContentTypeText = "text/plain";
        public const string kContentTypeJPG = "image/jpeg";
        public const string kContentTypeGIF = "image/gif";

        // HTTP Version needed and proper method 

        public const string kVersion = "HTTP/1.1";
        public const string kProperVerb = "GET";

        // Individual error codes

        public const int kOK = 200;                 // Used for a good request that can get a proper response
        public const int kBadRequest = 400;         // Used for bad syntax
        public const int kNotFound = 404;           // Used for files that are not found
        public const int kMethodNotAloud = 405;     // Used for methods outside of GET
        public const int kForbidden = 403;          // Used for files that are not used with the proper extension
        public const int kInvalidHTTPVersion = 505; // Used for invalid HTTP versions
        public const int kBadGateway = 502;         // Used for invalid server IP in requests
        public const int kUnsupportedMedia = 415;   // Used for if an extension is used that is not supported

        // Dictionary made read-only to still act as a type of constant that can be read elsewhere in the application

        public static readonly IReadOnlyDictionary<string, string> HTTPDictionary = new Dictionary<string, string>
        {
            { "200", "OK" },
            { "400", "Bad Request" },
            { "404", "Not Found" },
            { "405", "Method Not Aloud" },
            { "403", "Forbidden" },
            { "415", "Unsupported Media Type" },
            { "505", "HTTP Version Not Supported" },
            { "502", "Bad Gateway" }
        };
    }
}
