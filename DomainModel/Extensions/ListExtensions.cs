using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        ///  random number generator for the enumerable sampler.
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        ///  returns a random sample of the elements in an IEnumerable
        /// </summary>
        public static List<T> Sample<T>(this List<T> population, int sampleSize)
        {
            if (population == null)
            {
                return null;
            }

            List<T> localPopulation = population.ToList();
            if (localPopulation.Count < sampleSize)
                return localPopulation;

            List<T> sample = new List<T>(sampleSize);

            while (sample.Count < sampleSize)
            {
                int i = random.Next(0, localPopulation.Count);
                sample.Add(localPopulation[i]);
                localPopulation.RemoveAt(i);
            }

            return sample.OrderBy(a => a).ToList();
        }
    }
}
