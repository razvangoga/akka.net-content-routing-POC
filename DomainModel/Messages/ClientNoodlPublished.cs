using DomainModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Messages
{
    public class ClientNoodlPublished : NoodlPublished
    {
        public Client Client { get; set; }
    }
}
