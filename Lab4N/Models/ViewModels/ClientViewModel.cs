using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Lab4N.Models.ViewModels
{
    public class ClientViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
        public IEnumerable<Brokerage> Brokerages { get; set; }
    }
}