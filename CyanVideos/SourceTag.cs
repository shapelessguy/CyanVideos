using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public class SourceTag : Label
    {
        public static int height = 40;
        public void Disposer()
        {
            if(this != null) Dispose();
        }
        public string name;
        public string sourcedirectory;
        public SourceTag(string sourcedirectory, string name)
        {
            if (Source.verbose) Console.WriteLine("       Tag: " + name);
            this.name = name;
            this.sourcedirectory = sourcedirectory;
        }
    }
}
