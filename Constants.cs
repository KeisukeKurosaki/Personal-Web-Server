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
    static internal class Constants
    {
        public const string kServerLogPath = "myOwnWebServer.log";

        public const int kArgsMin = 0;
        public const int kArgsMax = 3;

        public const string kRootPrefix = "-webRoot=";
        public const string kIPPrefix = "-webIP=";
        public const string kPortPrefix = "-webPort=";

        public static readonly string[] kHTMLFamily = { ".htm", ".html", ".htmls", ".htx", "shtml", ".acgi" };
        public static readonly string[] KJPGFamily = { ".jfif", ".jfif-tbnl", ".jpeg", ".jpg" };

        public const string kGifExt = ".gif";
        public const string kTextExt = ".txt";

        public const string kVersion = "HTTP/1.1";
        public const string kProperVerb = "GET";

        public const int kOK = 200;                 // Used for a good request that can get a proper response
        public const int kBadRequest = 400;         // Used for bad syntax
        public const int kNotFound = 404;           // Used for files that are not found
        public const int kMethodNotAloud = 405;     // Used for methods outside of GET
        public const int kForbidden = 403;          // Used for files that are not used with the proper extension
        public const int kInvalidHTTPVersion = 505; // Used for invalid HTTP versions
        public const int kBadGateway = 502;         // Used for invalid server IP in requests
    }
}
