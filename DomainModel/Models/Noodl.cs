using DomainModel.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Model
{
    public class Noodl
    {
        public int NoodlId { get; set; }
        public List<string> FeedChannels { get; set; }

        public static int NoodlCounter = 0;

        public static Noodl GetNextNoodl()
        {
            NoodlCounter++;

            return new Noodl()
            {
                NoodlId = NoodlCounter,
                FeedChannels = FeedChannel.Channels.Sample(3)
            };
        }
    }
}
