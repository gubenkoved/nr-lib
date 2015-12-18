using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NRLib.Common
{
    public class BookProvisioner
    {
        public static BookProvisioner Current = new BookProvisioner();

        private Regex m_fileNameTemplateRegex = new Regex(@"^((?<autors>[^-]+)-(?<name>.+)\.(?<extension>[^.]+))|((?<name>.+)\.(?<extension>[^.]+))$", RegexOptions.RightToLeft | RegexOptions.Compiled);

        public Book TryConstructBookDefinitionFor(string path)
        {
            var fileInfo = new System.IO.FileInfo(path);

            Match templated = m_fileNameTemplateRegex.Match(fileInfo.Name);

            if (templated.Success)
            {
                List<string> authors = new List<string>();

                if (templated.Groups["autors"].Success)
                {
                    foreach (var autor in templated.Groups["autors"].Value.Split(','))
                    {
                        authors.Add(autor.Trim());
                    }
                }

                return new Book()
                {
                    FileInfo = fileInfo,
                    Name = templated.Groups["name"].Value.Trim(),
                    Extension = templated.Groups["extension"].Value.Trim(),
                    Authors = authors
                };
            }

            return null;
        }
    }
}
