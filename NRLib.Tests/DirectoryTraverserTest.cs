using NRLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace NRLib.Tests
{
    [TestClass()]
    public class DirectoryTraverserTest
    {
        [TestMethod()]
        public void GetAllFilesFlatTest()
        {
            string newDirName = Guid.NewGuid().ToString();

            CreateDirAndFiles(newDirName, 10);

            DirectoryTraverser traverser = new DirectoryTraverser();
            
            var files = traverser.GetAllFiles(newDirName);

            Assert.AreEqual(10, files.Count());
        }

        [TestMethod()]
        public void GetAllFilesSubDirsTest()
        {
            string dirName = Guid.NewGuid().ToString();
            string subdirName1 = Guid.NewGuid().ToString();
            string subdirName2 = Guid.NewGuid().ToString();

            CreateDirAndFiles(dirName, 5);

            CreateDirAndFiles(Path.Combine(dirName, subdirName1), 6);
            CreateDirAndFiles(Path.Combine(dirName, subdirName2), 7);

            DirectoryTraverser traverser = new DirectoryTraverser();

            var files = traverser.GetAllFiles(dirName);

            Assert.AreEqual(18, files.Count());
        }

        #region Helpers
        /// <summary>
        /// Creates N empty files
        /// </summary>
        private void CreateDirAndFiles(string dirPath, int fileCount)
        {
            Directory.CreateDirectory(dirPath);

            for (int i = 0; i < fileCount; i++)
            {
                string filePath = Path.Combine(dirPath, Guid.NewGuid().ToString());

                FileStream fStream = File.Create(filePath);
                fStream.Dispose();
            }
        }
        #endregion
    }
}
