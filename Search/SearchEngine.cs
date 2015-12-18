using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NRLib.Common;

namespace NRLib.Search
{
    public abstract class SearchEngine
    {
        public delegate void ErrorHandler(object sender, SearchErrorArgs arg);
        public abstract event ErrorHandler Error;

        public string RootDirectory { get; set; }

        public abstract IEnumerable<MatchInfo> Search(string searchQuery);

        protected abstract void IndexBook(Book bookInfo);
    }
}