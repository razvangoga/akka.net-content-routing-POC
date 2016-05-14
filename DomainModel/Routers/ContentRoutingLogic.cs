using Akka.Routing;
using DomainModel.Actors;
using DomainModel.Messages;
using DomainModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Routers
{
    public class ContentRoutingLogic : RoutingLogic
    {
        private Dictionary<Client, Routee> _clientRoutees;

        public ContentRoutingLogic(Dictionary<Client, Routee> countRoutees)
        {
            this._clientRoutees = countRoutees;
        }

        /// <summary>
        /// Picks all the <see cref="Routee">routees</see> in <paramref name="routees"/> to receive the <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message that is being routed.</param>
        /// <param name="routees">A collection of routees that receives the <paramref name="message"/>.</param>
        /// <returns>A <see cref="Routee"/> that contains all the given <paramref name="routees"/> that receives the <paramref name="message"/>.</returns>
        public override Routee Select(object message, Routee[] routees)
        {
            if (routees == null || !routees.Any())
                return Routee.NoRoutee;

            List<Routee> eligibleRoutees = new List<Routee>();

            foreach (Client client in this._clientRoutees.Keys)
            {
                if (client.IsInteresedIn((message as NoodlPublished).Noodl))
                    eligibleRoutees.Add(this._clientRoutees[client]);
            }

            if (eligibleRoutees.Count == 0)
                return Routee.NoRoutee;

            return new SeveralRoutees(eligibleRoutees.ToArray());
        }
    }
}
