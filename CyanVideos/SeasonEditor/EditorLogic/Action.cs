using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CyanVideos.SeasonEditor
{
    public class Action
    {
        public Action()
        {
        }

        public virtual void PerformAction(bool reverse = false)
        {
        }

        public virtual string toString()
        {
            return "";
        }
        public static void CopyWait(string from, string to, bool overwrite = false)
        {
            File.Copy(from, to, overwrite);
            for (int i = 0; i < 50;)
            {
                if (IsFileLocked(new FileInfo(to))) { 
                    Thread.Sleep(100); continue; 
                }
                return;
            }
            throw new Exception("Copy is taking too long! Check your file extensions.");
        }

        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        public static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            Console.WriteLine("CopyRecurs " + targetPath);
            Directory.CreateDirectory(targetPath);

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Console.WriteLine("CopyRecurs -> " + dirPath);
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                Console.WriteLine("CopyRecurs -> " + newPath);
                CopyWait(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
        public static void DeleteFilesRecursively(string targetPath)
        {
            //Delete all the files
            foreach (string target in Directory.GetFiles(targetPath, "*.*", SearchOption.AllDirectories))
            {
                File.Delete(target);
            }

            //Now Delete all of the directories
            foreach (string target in Directory.GetDirectories(targetPath, "*", SearchOption.AllDirectories))
            {
                Directory.Delete(target);
            }
            Directory.Delete(targetPath);
        }
    }
    public class Movement : Action
    {
        public string from;
        public string to;
        public bool is_dir;
        public bool rev_op;
        public Movement(string from, string to, bool rev_op = false)
        {
            this.from = from;
            this.to = to;
            is_dir = Path.GetExtension(from) == "";
            this.rev_op = rev_op;
        }

        public override void PerformAction(bool reverse = false)
        {
            reverse = reverse ? !rev_op : rev_op;
            if (!reverse)
            {
                Console.WriteLine(toString());
                Directory.Move(from, to);
            }
            else
            {
                Console.WriteLine("reverse " + toString());
                Directory.Move(to, from);
            }
        }

        public override string toString()
        {
            string string_ = "move: " + from + " -> " + to;
            return string_;
        }
    }
    public class Copy : Action
    {
        public string from;
        public string to;
        public bool is_dir;
        public bool rev_op;
        public Copy(string from, string to, bool rev_op = false)
        {
            this.from = from;
            this.to = to;
            is_dir = Path.GetExtension(from) == "";
            this.rev_op = rev_op;
        }

        public override void PerformAction(bool reverse = false)
        {
            reverse = reverse ? !rev_op : rev_op;
            if (!reverse)
            {
                Console.WriteLine(toString());
                if (is_dir)
                {
                    CopyFilesRecursively(from, to);
                }
                else
                {
                    if (!File.Exists(to)) CopyWait(from, to);
                }
            }
            else
            {
                Console.WriteLine("reverse " + toString());
                if (is_dir)
                {
                    DeleteFilesRecursively(to);
                }
                else
                {
                    if (File.Exists(to)) File.Delete(to);
                }
            }
        }

        public override string toString()
        {
            string string_ = "copy: " + from + " -> " + to;
            return string_;
        }
    }
    public class Create : Action
    {
        public string directory;
        public bool rev_op;
        public Create(string directory, bool rev_op = false)
        {
            this.rev_op = rev_op;
            this.directory = directory;
        }

        public override void PerformAction(bool reverse = false)
        {
            reverse = reverse ? !rev_op : rev_op;
            if (!reverse)
            {
                Console.WriteLine(toString());
                Directory.CreateDirectory(directory);
            }
            else
            {
                Console.WriteLine("reverse " + toString());
                Directory.Delete(directory, true);
            }
        }

        public override string toString()
        {
            string string_ = "create: " + directory;
            return string_;
        }
    }
}