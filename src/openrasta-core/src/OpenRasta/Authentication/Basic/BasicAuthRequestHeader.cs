using System;
using System.Text;
using OpenRasta;

namespace OpenRasta.Authentication.Basic
{
    public class BasicAuthRequestHeader
    {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public BasicAuthRequestHeader(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}