using DomainModel.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Model
{
    public class Client
    {
        public int Id { get; set; }

        public string Name
        {
            get
            {
                return string.Format("Client{0}", this.Id);
            }
        }

        public List<string> FeedChannels { get; set; }

        public bool IsInteresedIn(Noodl noodl)
        {
            return noodl.FeedChannels.Any(fc => this.FeedChannels.Contains(fc));
        }

        public static List<Client> GetClients(int count)
        {
            List<Client> result = new List<Client>();

            for (int i = 1; i <= count; i++)
            {
                result.Add(new Client()
                {
                    Id = i,
                    FeedChannels = FeedChannel.Channels.Sample(3)
                });
            }

            return result;
        }
    }
}
