using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JabbR.Models;
using JabbR.Services;

namespace JabbR.Commands
{
 
    public interface ICommand 
    {
        void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent);
    }
}
