using System;
using System.Windows.Media.Imaging;

namespace WpfExplorer.Models
{
    public class DirectoryItem
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public string Size { get; set; }
        public string Type { get; set; }
        public string LastModify { get; set; }
        public BitmapSource Icon { get; set; }

        public DirectoryItem(string name, string fullName, string size, string type, string lastmodify)
        {
            Name = name;
            DisplayName = name.Length < 17 ? Name : Name.Substring(0, 17) + "...";
            FullName = fullName;
            Size = size;
            Type = type;
            LastModify = lastmodify;
            Icon = IconLoader.GetInstance().GetIcon(FullName);
        }
    }
}
