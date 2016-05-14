using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util;
using DomainModel.Model;
using Akka.Configuration;
using DomainModel.Actors;

namespace DomainModel.Routers
{
    /// <summary>
    /// This class represents a <see cref="Pool"/> router that sends messages it receives to all of its <see cref="Routee">routees</see>.
    /// </summary>
    public class ContentRouterPool : Pool
    {
        private List<Client> _clients;

        /// <summary>
        /// This class represents a surrogate of a <see cref="BroadcastPool"/> router.
        /// Its main use is to help during the serialization process.
        /// </summary>
        public class ContentRouterPoolSurrogate : ISurrogate
        {
            /// <summary>
            /// Creates a <see cref="ContentRouterPool"/> encapsulated by this surrogate.
            /// </summary>
            /// <param name="system">The actor system that owns this router.</param>
            /// <returns>The <see cref="ContentRouterPool"/> encapsulated by this surrogate.</returns>
            public ISurrogated FromSurrogate(ActorSystem system)
            {
                return new ContentRouterPool(NrOfInstances, Resizer, SupervisorStrategy, RouterDispatcher, UsePoolDispatcher);
            }

            /// <summary>
            /// The number of routees associated with this pool.
            /// </summary>
            public int NrOfInstances { get; set; }
            /// <summary>
            /// Determine whether or not to use the pool dispatcher. The dispatcher is defined in the
            /// 'pool-dispatcher' configuration property in the deployment section of the router.
            /// </summary>
            public bool UsePoolDispatcher { get; set; }
            /// <summary>
            /// The resizer to use when dynamically allocating routees to the pool.
            /// </summary>
            public Resizer Resizer { get; set; }
            /// <summary>
            /// The strategy to use when supervising the pool.
            /// </summary>
            public SupervisorStrategy SupervisorStrategy { get; set; }
            /// <summary>
            /// The dispatcher to use when passing messages to the routees.
            /// </summary>
            public string RouterDispatcher { get; set; }
        }

        /// <summary>
        /// Creates a surrogate representation of the current <see cref="ContentRouterPool"/>.
        /// </summary>
        /// <param name="system">The actor system that owns this router.</param>
        /// <returns>The surrogate representation of the current <see cref="ContentRouterPool"/>.</returns>
        public override ISurrogate ToSurrogate(ActorSystem system)
        {
            return new ContentRouterPoolSurrogate
            {
                NrOfInstances = NrOfInstances,
                UsePoolDispatcher = UsePoolDispatcher,
                Resizer = Resizer,
                SupervisorStrategy = SupervisorStrategy,
                RouterDispatcher = RouterDispatcher
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentRouterPool"/> class.
        /// </summary>
        /// <param name="config">The configuration used to configure the pool.</param>
        public ContentRouterPool(Config config) : base(config)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastPool"/> class.
        /// </summary>
        /// <param name="nrOfInstances">The initial number of routees in the pool.</param>
        /// <param name="resizer">The resizer to use when dynamically allocating routees to the pool.</param>
        /// <param name="supervisorStrategy">The strategy to use when supervising the pool.</param>
        /// <param name="routerDispatcher">The dispatcher to use when passing messages to the routees.</param>
        /// <param name="usePoolDispatcher"><c>true</c> to use the pool dispatcher; otherwise <c>false</c>.</param>
        public ContentRouterPool(int nrOfInstances, Resizer resizer, SupervisorStrategy supervisorStrategy, string routerDispatcher, bool usePoolDispatcher = false)
            : base(nrOfInstances, resizer, supervisorStrategy, routerDispatcher, usePoolDispatcher)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentRouterPool"/> class.
        /// 
        /// <note>
        /// A <see cref="ContentRouterPool"/> configured in this way uses the <see cref="Pool.DefaultStrategy"/> supervisor strategy.
        /// </note>
        /// </summary>
        /// <param name="nrOfInstances">The initial number of routees in the pool.</param>
        public ContentRouterPool(List<Client> clients) : base(clients.Count, null, DefaultStrategy, null)
        {
            this._clients = clients;
        }

        /// <summary>
        /// Creates a router that is responsible for routing messages to routees within the provided <paramref name="system" />.
        /// </summary>
        /// <param name="system">The actor system that owns this router.</param>
        /// <returns>The newly created router tied to the given system.</returns>
        public override Router CreateRouter(ActorSystem system)
        {
            Dictionary<Client, Routee> routees = new Dictionary<Client, Routee>();

            foreach (Client client in this._clients)
            {
                IActorRef clientActor = system.ActorOf(Akka.Actor.Props.Create(() => new ClientActor(client)), client.Name);
                routees.Add(client, Routee.FromActorRef(clientActor));
            }

            return new Router(new ContentRoutingLogic(routees));
        }

        /// <summary>
        /// Creates a new <see cref="ContentRouterPool"/> router with a given <see cref="SupervisorStrategy"/>.
        /// 
        /// <note>
        /// This method is immutable and returns a new instance of the router.
        /// </note>
        /// </summary>
        /// <param name="strategy">The <see cref="SupervisorStrategy"/> used to configure the new router.</param>
        /// <returns>A new router with the provided <paramref name="strategy" />.</returns>
        public override Pool WithSupervisorStrategy(SupervisorStrategy strategy)
        {
            return new ContentRouterPool(NrOfInstances, Resizer, strategy, RouterDispatcher, UsePoolDispatcher);
        }

        /// <summary>
        /// Creates a new <see cref="ContentRouterPool"/> router with a given <see cref="Resizer"/>.
        /// 
        /// <note>
        /// This method is immutable and returns a new instance of the router.
        /// </note>
        /// </summary>
        /// <param name="resizer">The <see cref="Resizer"/> used to configure the new router.</param>
        /// <returns>A new router with the provided <paramref name="resizer" />.</returns>
        public override Pool WithResizer(Resizer resizer)
        {
            return new ContentRouterPool(NrOfInstances, resizer, SupervisorStrategy, RouterDispatcher, UsePoolDispatcher);
        }

        /// <summary>
        /// Creates a new <see cref="ContentRouterPool"/> router with a given dispatcher id.
        /// 
        /// <note>
        /// This method is immutable and returns a new instance of the router.
        /// </note>
        /// </summary>
        /// <param name="dispatcher">The dispatcher id used to configure the new router.</param>
        /// <returns>A new router with the provided dispatcher id.</returns>
        public override Pool WithDispatcher(string dispatcher)
        {
            return new ContentRouterPool(NrOfInstances, Resizer, SupervisorStrategy, dispatcher, UsePoolDispatcher);
        }

        /// <summary>
        /// Configure the current router with an auxiliary router for routes that it does not know how to handle.
        /// </summary>
        /// <param name="routerConfig">The router to use as an auxiliary source.</param>
        /// <returns>The router configured with the auxiliary information.</returns>
        public override RouterConfig WithFallback(RouterConfig routerConfig)
        {
            return OverrideUnsetConfig(routerConfig);
        }
    }
}
