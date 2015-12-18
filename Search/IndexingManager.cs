using NRLib.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NRLib.Search
{
    public class IndexingManager
    {
        public static readonly IndexingManager Default = new IndexingManager();

        private IEnumerable<string> m_allowedExtensions;

        public IndexingManager()
        {
            m_allowedExtensions = Properties.Settings.Default.Extensions.Split('|').Select(e => e.ToLower());
        }

        public BlockingCollection<Book> ListBooks(string dir)
        {
            var result = new BlockingCollection<Book>();

            Task.Factory.StartNew(() =>
            {
                Walk(new DirectoryInfo(dir), result);

                result.CompleteAdding();

            });

            // return control instantly - will fill collection in non blocking fashion
            return result;
        }

        private void Walk(DirectoryInfo dir, BlockingCollection<Book> target)
        {
            foreach (var file in dir.GetFiles())
            {
                // if match yield - return 
                Book book = ConstructBook(file.FullName);

                if (book != null)
                {
                    target.Add(book);
                }
            }

            foreach (var subdirectory in dir.GetDirectories())
            {
                Walk(subdirectory, target);
            }
        }

        private Book ConstructBook(string path)
        {
            if (m_allowedExtensions.Contains(Path.GetExtension(path).ToLower()))
            {
                Book book = BookProvisioner.Current.TryConstructBookDefinitionFor(path);

                return book;
            }

            return null;
        }
    }
}
