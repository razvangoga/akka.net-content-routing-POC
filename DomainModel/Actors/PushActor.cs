using Akka.Actor;
using DomainModel.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Actors
{
    public class PushActor : ReceiveActor
    {
        private Random _random = new Random();

        public PushActor()
        {
            this.Receive<ClientNoodlPublished>(cn =>
            {
                Thread.Sleep(1000 * _random.Next(1, 9));
                Console.WriteLine("Client {0} : Noodl {0} was published", cn.Client.Id, cn.Noodl.NoodlId);
            });

            //this.Receive<NoodlPublished>(n =>
            //{
            //    Thread.Sleep(1000 * _random.Next(1, 9));
            //    Console.WriteLine("Noodl {0} was published", n.Noodl.NoodlId);
            //});
        }
    }
}
