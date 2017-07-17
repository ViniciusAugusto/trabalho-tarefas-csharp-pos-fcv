using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Tarefas.Models;

namespace Tarefas.Hubs
{
    public class NotificationHub : Hub
    {
        public void Entrou(string nome)
        {
            Clients.Others.entrou(nome);
        }
    }
}