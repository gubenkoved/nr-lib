using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRLib.Common
{
    [Serializable]
    public class Book
    {
        private FileInfo m_fileInfo;
        public FileInfo FileInfo
        {
            get { return m_fileInfo; }
            set { m_fileInfo = value; }
        }

        [NonSerialized]
        private IEnumerable<string> m_authors;
        public IEnumerable<string> Authors
        {
            get { return m_authors; }
            set { m_authors = value; }
        }

        [NonSerialized]
        private string m_name;
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        [NonSerialized]
        private string m_extension;
        public string Extension
        {
            get { return m_extension; }
            set { m_extension = value; }
        }

        public Book()
        {
            Authors = new List<string>();
        }

        public override bool Equals(object obj)
        {
            if (obj is Book)
            {
                return this.FileInfo.FullName == ((Book)obj).FileInfo.FullName;
            }

            return false;
        }
    }
}
