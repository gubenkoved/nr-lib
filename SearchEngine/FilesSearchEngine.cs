using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NRLib
{
    public abstract class FilesSearchEngine
    {
        public string RootDirectory { get; set; }

        public abstract IEnumerable<FileMatchInfo> Search(string searchQuery);

        protected abstract void IndexFile(FileInfo fileInfo);
    }
}