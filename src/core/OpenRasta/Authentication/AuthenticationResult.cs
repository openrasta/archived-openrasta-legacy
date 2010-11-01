using System;
using System.Collections.Generic;
using System.Security.Principal;
using OpenRasta.Pipeline.Contributors;

namespace OpenRasta.Authentication
{
    public class AuthenticationResult
    {
        public class MalformedCredentials : AuthenticationResult { }

        public class Failed : AuthenticationResult { }

        public class Success : AuthenticationResult
        {
            public string Username { get; private set; }
            public string[] Roles { get; private set; }

            public Success(string username, params string[] roles)
            {
                Username = username;
                Roles = roles;
            }
        }
    }
}