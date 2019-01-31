﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FileBrowser.Properties;

namespace FileBrowser.Model {
    /// <summary>
    /// This class represents a directory
    /// </summary>
    public class Folder : IEquatable<Folder> {

      
        public int FolderId { get; set; }

        /// <summary>
        /// Path of the directory
        /// </summary>
        private string path;

        /// <summary>
        /// Gets or sets the path of the directory
        /// </summary>

        public string Path {
            get => path;
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new ArgumentException(Resources.ArgumentExceptionPathFolder);
                }
                path = value;
            }
        }

       
        /// for entity framework
      
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public Folder() {

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="path">The path of the directory</param>
        public Folder(string path) {
            Path = path;
        }

        /// <summary>
        /// Gets all the files that match the extensions by performaing a deep search trough the whole file hierarchy, starting from the Path of this Folder object.
        /// </summary>
        /// <param name="extensions">A collection of extensions</param>
        /// <returns>A sorted collection of files that match the extensions</returns>
        public FileBrowserTreeItem GetFiles(ICollection<FileExtension> extensions) {
            DirectoryInfo directory = new DirectoryInfo(Path);
            FileBrowserTreeItem rootItem = new FileBrowserTreeItem(Path);
            foreach (FileExtension ext in extensions) {
                string regex = "*" + ext.Extension;
                rootItem.Children.AddRange(directory.GetFiles(regex, SearchOption.AllDirectories).Select(fi => new FileBrowserTreeItem(fi.Name)));
            }
            rootItem.Children.Sort(new FileBrowserTreeItemComparer());
            return rootItem;
        }


        /// <summary>
        /// Overriden from IEquatable. Determines if an other Directory instance is equal to this one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>true if equal; else false</returns>
        public bool Equals(Folder other) {
            if (other == null) {
                return false;
            }
            return Path.Equals(other.Path);
        }



        /// <summary>
        /// Special File Comparer that treats digits as numerical rather than text
        /// </summary>
        private class FileBrowserTreeItemComparer : IComparer<FileBrowserTreeItem> {

            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern int StrCmpLogicalW(string x, string y);


            /// <summary>
            /// Compares two Unicode strings. Digits in the strings are considered as numerical content rather than text. this test is not case-sensitive.
            /// </summary>
            /// <param name="x">First FileInfo instance</param>
            /// <param name="y">Second FileInfo instance</param>
            /// <returns>0 if identical
            /// 1 if x > y
            /// -1 if x < y
            /// </returns>
            public int Compare(FileBrowserTreeItem x, FileBrowserTreeItem y) {
                return StrCmpLogicalW(x.Name, y.Name);
            }
        }
    }
}
