using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Model
{
    public static class GeneralStatusCodes
    {
        public static readonly (string code, string message) SUCCESS = ("00", "Success");

        public static readonly (string code, string message) SYSTEM_EXCEPTION = ("SYSTEM_EXCEPTION", "System Exception occured");

        public static readonly (string code, string message) INTERNAL_SERVER_ERROR = ("INTERNAL_SERVER_ERROR", "An error has occured");

        public static readonly (string code, string message) UNEXPECTED_ERROR = ("UNEXPECTED_ERROR", "An error has occured");

        public static readonly (string code, string message) VALIDATION_ERROR = ("VALIDATION_ERROR", "Some validation error happened");
        public static readonly (string code, string message) Status_NotFound = ("NOT_FOUND", "RESOURCE NOT FOUND");

    }

    public static class UserStatusCodes
    {
        public static readonly (string code, string message) INVALID_CREDENTIALS = ("INVALID_CREDENTIALS", "Invalid user credentials provided");

        public static readonly (string code, string message) INVALID_USER = ("INVALID_USER", "User is invalid");

        
    }

    public static class AuthStatusCode
    {
        public static readonly (string code, string message) INVALID_AUTHTOKEN = ("INVALID_AUTHTOKEN", "Invalid auth token");

        public static readonly (string code, string message) LOGIN_REQUIRED = ("LOGIN_REQUIRED", "Login required. please login with username and password");

        public static readonly (string code, string message) INVALID_AUTHCLIENT = ("INVALID_AUTHCLIENT", "Client microservice is invalid. Please contact admin");


    }
}
