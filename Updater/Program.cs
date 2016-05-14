using Akka.Actor;
using Akka.Configuration;
using DomainModel;
using DomainModel.Actors;
using DomainModel.Messages;
using DomainModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("Deployer", ConfigurationFactory.ParseString(@"
                akka {  
                    actor{
                        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                        deployment {
                            /directpublisher {
                                remote = ""akka.tcp://DirectPublisher@localhost:8090""
                            }
                        }
                    }
                    remote {
                        helios.tcp {
		                    port = 0
		                    hostname = localhost
                        }
                    }
                }")))
            {
                IActorRef directPublicationActor = system.ActorOf(Props.Create(() => new PublicationActor()), "directpublisher");

                do
                {

                    while (!Console.KeyAvailable)
                    {
                        Console.WriteLine("Sending new noodl");

                        Noodl noodl = Noodl.GetNextNoodl();

                        directPublicationActor.Tell(new NoodlPublished() { Noodl = noodl });

                        Thread.Sleep(4000); // 20,000 a day equates to about one every 4 seconds
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            }
        }
    }
}
