using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CyanVideos.MultiMonitorTool
{
    public class MonitorCollection
    {
        //    API CALLS    ----------------------------------------------------------------------------------------------------------------------
<<<<<<< HEAD
        public static MonitorCollection getMonitorConfiguration()
        {
            string multimonitorDirectory = Path.Combine(Environment.CurrentDirectory, "MultiMonitorTool");
            string multimonitorExe = Path.Combine(multimonitorDirectory, "MultiMonitorTool.exe");
            string conf_path = Path.Combine(multimonitorDirectory, "multimonitor.cfg");
            cmdAsync(multimonitorExe, "/SaveConfig \"" + conf_path + "\"");

            string[] lines = null;
            for (int i = 0; i < 20; i++)
            {
                if (!SeasonEditor.Action.IsFileLocked(new FileInfo(conf_path)))
                {
                    lines = File.ReadAllLines(conf_path);
                    for (int j = 0; j < 20; j++)
                    {
                        if (!SeasonEditor.Action.IsFileLocked(new FileInfo(conf_path)))
                        {
                            File.Delete(conf_path);
                            Console.WriteLine("Deleting conf file");
                            break;
                        }
                        Thread.Sleep(100);
                    }
                    break;
                }
                Thread.Sleep(100);
            }

            MonitorCollection monitors = new MonitorCollection();
            if (lines != null)
            {
                List<string> monitor_gen = new List<string>();
                foreach (var line in lines)
                {
                    if (line.StartsWith("["))
                    {
                        Monitor.Add(monitor_gen, monitors);
                        monitor_gen = new List<string>();
                    }
                    else
                    {
                        monitor_gen.Add(line.Substring(line.IndexOf("=") + 1));
                    }
                }
                Monitor.Add(monitor_gen, monitors);
            }
            monitors.Order();
            monitors.ValidateIds();
            return monitors;
=======
        public static MonitorCollection getMonitorConfiguration(string multimonitorDirectory)
        {
            try
            {
                string multimonitorExe = Path.Combine(multimonitorDirectory, "MultiMonitorTool.exe");
                string conf_path = Path.Combine(multimonitorDirectory, "multimonitor.cfg");
                cmdAsync(multimonitorExe, "/SaveConfig \"" + conf_path + "\"");
                string[] lines = null;
                for (int i = 0; i < 20; i++)
                {
                    if (File.Exists(conf_path) && !SeasonEditor.Action.IsFileLocked(new FileInfo(conf_path)))
                    {
                        lines = File.ReadAllLines(conf_path);
                        for (int j = 0; j < 20; j++)
                        {
                            if (!SeasonEditor.Action.IsFileLocked(new FileInfo(conf_path)))
                            {
                                File.Delete(conf_path);
                                Console.WriteLine("Deleting conf file");
                                break;
                            }
                            Thread.Sleep(100);
                        }
                        break;
                    }
                    Thread.Sleep(100);
                }

                MonitorCollection monitors = new MonitorCollection();
                if (lines != null)
                {
                    List<string> monitor_gen = new List<string>();
                    foreach (var line in lines)
                    {
                        if (line.StartsWith("["))
                        {
                            Monitor.Add(monitor_gen, monitors);
                            monitor_gen = new List<string>();
                        }
                        else
                        {
                            monitor_gen.Add(line.Substring(line.IndexOf("=") + 1));
                        }
                    }
                    Monitor.Add(monitor_gen, monitors);
                }
                monitors.Order();
                monitors.ValidateIds();
                foreach (Monitor m in monitors.GetAll()) m.print();
                return monitors;
            }
            catch 
            {
                MessageBox.Show("Error while fetching Monitor configuration");
                return new MonitorCollection();
            }
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
        }
        public List<Monitor> GetAll()
        {
            return collection;
        }
        public string[] getIds()
        {
            List<string> ids = new List<string>();
            foreach (Monitor monitor in collection)
            {
                ids.Add(monitor.id);
            }
            return ids.ToArray();
        }
        public Screen getScreen(string id)
        {
            foreach (Monitor monitor in collection)
            {
                if (monitor.id == id) return monitor.screen;
            }
            return null;
        }
        public class Monitor
        {
            public Screen screen;
            public string name;
            public string id;
            public int bpp;
            public int width;
            public int height;
            public int display_flags;
            public int display_freq;
            public int display_orient;
            public int x;
            public int y;
            private Monitor(Screen screen, string name, string id, int bpp, int width, int height, int display_flags, int display_freq, int display_orient, int x, int y)
            {
                this.screen = screen;
                this.name = name;
                this.id = id;
                this.bpp = bpp;
                this.width = width;
                this.height = height;
                this.display_flags = display_flags;
                this.display_freq = display_freq;
                this.display_orient = display_orient;
                this.x = x;
                this.y = y;
            }

<<<<<<< HEAD
            public void print()
            {
                Console.WriteLine(name);
                Console.WriteLine("\tScreen: " + screen);
                Console.WriteLine("\tID: " + id);
                Console.WriteLine("\tBits per Pixel: " + bpp);
                Console.WriteLine("\tWidth: " + width);
                Console.WriteLine("\tHeight: " + height);
                Console.WriteLine("\tDisplay flags: " + display_flags);
                Console.WriteLine("\tDisplat frequency: " + display_freq);
                Console.WriteLine("\tDisplay orientation: " + display_orient);
                Console.WriteLine("\tPosition X: " + x);
                Console.WriteLine("\tPosition Y: " + y);
=======
            public string print()
            {
                string output = "";
                output += name + "\n";
                output += "\tScreen: " + screen + "\n";
                output += "\tID: " + id + "\n";
                output += "\tBits per Pixel: " + bpp + "\n";
                output += "\tWidth: " + width + "\n";
                output += "\tHeight: " + height + "\n";
                output += "\tDisplay flags: " + display_flags + "\n";
                output += "\tDisplat frequency: " + display_freq + "\n";
                output += "\tDisplay orientation: " + display_orient + "\n";
                output += "\tPosition X: " + x + "\n";
                output += "\tPosition Y: " + y + "\n";
                Console.WriteLine(output);
                return output;
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
            }

            public static void Add(List<string> monitor_gen, MonitorCollection list_monitors)
            {
                if (monitor_gen.Count == 10)
                {
                    string name = monitor_gen[0];
                    string id = monitor_gen[1].Split('\\')[1];
                    int bpp;
                    int width;
                    int height;
                    int display_flags;
                    int display_freq;
                    int display_orient;
                    int x;
                    int y;
                    int.TryParse(monitor_gen[2], out bpp);
                    int.TryParse(monitor_gen[3], out width);
                    int.TryParse(monitor_gen[4], out height);
                    int.TryParse(monitor_gen[5], out display_flags);
                    int.TryParse(monitor_gen[6], out display_freq);
                    int.TryParse(monitor_gen[7], out display_orient);
                    int.TryParse(monitor_gen[8], out x);
                    int.TryParse(monitor_gen[9], out y);
                    Screen screen = null;
                    foreach (var sc in Screen.AllScreens)
                    {
                        if (sc.DeviceName == monitor_gen[0]) screen = sc;
                    }
                    list_monitors.Add(new Monitor(screen, name, id, bpp, width, height, display_flags, display_freq, display_orient, x, y));
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------

        private List<Monitor> collection;
        private MonitorCollection() { collection = new List<Monitor>(); }
        private void Add(Monitor monitor)
        {
            collection.Add(monitor);
        }
        private void Order()
        {
            collection = collection.OrderBy(p => p.x).ToList();
        }
        private void ValidateIds()
        {
            List<string> mult_ids = new List<string>();
            foreach (Monitor monitor in collection)
            {
                if (getIds().Where(s => s == monitor.id).Count() > 1 && !mult_ids.Contains(monitor.id)) mult_ids.Add(monitor.id);
            }
            foreach (string id in mult_ids)
            {
                int i = 0;
                foreach (Monitor monitor in collection)
                {
                    if (monitor.id == id)
                    {
                        i += 1;
                        monitor.id += " (" + i + ")";
                    }
                }
            }
        }
        private static void cmdAsync(string cmd, string args, bool isPath = false)
        {
            void run()
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = cmd;
                start.WindowStyle = ProcessWindowStyle.Hidden;
                start.Arguments = args;
                if (isPath) start.Arguments = "\"" + args + "\"";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
<<<<<<< HEAD
                        Console.Write(result);
=======
                        Console.WriteLine(result);
>>>>>>> fe9cbb00a4508453f9405ee283b2bff1a3681d22
                    }
                }
            }
            new System.Threading.Thread(run).Start();
        }

    }
    
}
