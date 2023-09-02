using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace CyanVideos.SeasonEditor
{
    internal class ReferencesInjector
    {
        private string main_folder;
        public ReferencesInjector(string main_folder)
        {
            this.main_folder = main_folder;
        }

        public List<Reference> GetReferences()
        {
            Console.WriteLine("Getting references from " + main_folder);
            List<Reference> references = new List<Reference>();
            string[] videos = Program.GetAllVideos(main_folder, 10);
            for (int i = 0; i < videos.Length; i++)
            {
                Reference reference = new Reference(i);
                reference.path = videos[i];
                reference.directory = Path.GetDirectoryName(reference.path);
                reference.name = Path.GetFileNameWithoutExtension(reference.path);
                reference.extension = Path.GetExtension(reference.path);
                reference.Initialize();
                references.Add(reference);
            }
            return references;
        }
    }
}