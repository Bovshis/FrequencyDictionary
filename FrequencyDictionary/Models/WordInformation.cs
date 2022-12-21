using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrequencyDictionary.Models
{
    public class WordInformation
    {
        public int Count { get; set; }
        public string Lemma { get; set; }
        public string Tags { get; set; }

        public override string ToString()
        {
            return $"{Count},{Lemma},{Tags}";
        }

        public static WordInformation Parse(string information)
        {
            var elements = information.Split(',');
            if (elements.Length != 3) throw new ArgumentException("Bad string format!");

            return new WordInformation()
            {
                Count = int.Parse(elements[0]),
                Lemma = elements[1],
                Tags = elements[2]
            };
        }
    }
}
