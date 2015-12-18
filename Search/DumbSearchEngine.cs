﻿using NRLib.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRLib.Search
{
    public class DumbSearchEngine : SearchEngine
    {
        public override event ErrorHandler Error;

        public override IEnumerable<MatchInfo> Search(string searchQuery)
        {
            if (RootDirectory == null)
            {
                RaiseError(new SearchErrorArgs()
                {
                    Exception = new Exception("RootDirectory was null."),
                });

                yield break;
            }

            BlockingCollection<Book> allBooks = IndexingManager.Default.ListBooks(RootDirectory);

            foreach (Book book in allBooks.GetConsumingEnumerable())
            {
                string[] terms = searchQuery.Split(' ');

                if (terms.All(s => book.FileInfo.FullName.ToLower().IndexOf(s.ToLower()) != -1))
                {
                    yield return new MatchInfo()
                    {
                        Book = book,
                    };
                }
            }
        }

        protected override void IndexBook(Book bookInfo)
        {
            // no-op, no index - all in runtime
        }

        protected void RaiseError(SearchErrorArgs errorInfo)
        {
            Error?.Invoke(this, errorInfo);
        }
    }
}