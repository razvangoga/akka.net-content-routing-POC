using Akka.Actor;
using DomainModel.Messages;
using DomainModel.Model;
using DomainModel.Routers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Actors
{
    public class PublicationActor : ReceiveActor
    {
        private IActorRef _clientActor;

        public PublicationActor()
        {
            this._clientActor = Context.ActorOf(Props.Create(() => new ClientRouterActor())
                .WithRouter(new ContentRouterPool(Client.GetClients(3))), "contentRouter");

            this.Receive<NoodlPublished>((n) =>
            {
                this._clientActor.Tell(n);
            });
        }
    }
}
