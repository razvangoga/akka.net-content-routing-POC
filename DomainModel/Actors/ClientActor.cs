using Akka.Actor;
using Akka.Routing;
using DomainModel.Messages;
using DomainModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Actors
{
    public class ClientActor : ReceiveActor
    {
        private Client _client;
        private IActorRef _pushActor;

        public ClientActor(Client client)
        {
            this._client = client;

            this._pushActor = Context.ActorOf(Props.Create(() => new PushActor())
                .WithRouter(new RoundRobinPool(10)));

            this.Receive<NoodlPublished>(n =>
            {
                this._pushActor.Tell(new ClientNoodlPublished() { Client = this._client, Noodl = n.Noodl });
            });
        }
    }
}
