using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NRLib
{
    public class DirectoryTraverser
    {
        /// <summary>
        /// Returns all files within root dir and all its subdirs.
        /// </summary>
        public IEnumerable<FileInfo> GetAllFiles(string rootDir)
        {
            foreach (var filePath in Directory.GetFiles(rootDir))
            {
                yield return new FileInfo(filePath);
            }

            foreach (var subDirPath in Directory.GetDirectories(rootDir))
            {
                foreach (var subDirFileInfo in GetAllFiles(subDirPath))
                {
                    yield return subDirFileInfo;
                }
            }
        }
    }
}
