using System;
using System.Collections.Generic;

namespace NRLib.Search
{
    public class LuceneSearchEngine : SearchEngine
    {
        public override event ErrorHandler Error;

        public override void Init()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<MatchInfo> Search(string searchQuery)
        {
            throw new NotImplementedException();
        }
    }
}
