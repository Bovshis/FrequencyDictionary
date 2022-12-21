using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POSTagger.Corpora;
using POSTagger.Taggers;

namespace FrequencyDictionary.Extensions
{
    public static class TaggerExtension
    {
        public static string Tags(this ITagger tagger, Corpus corpus, string word)
        {
            throw new NotImplementedException();
        }
    }
}
