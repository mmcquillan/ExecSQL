using System;
using System.Xml;
using System.IO;
using System.Threading;

namespace ExecSQL
{

    class Program
    {

        // public to keep track of errors
        public static bool Success = true;
        public static int Threads = 0;

        // main program
        [MTAThread]
        static void Main(string[] args)
        {

            // collect stats
            DateTime startTime = DateTime.Now;


            // open settings file
            XmlDocument settings = new XmlDocument();
            if (args.Length == 1)
            {
                settings.Load(args[0]);
            }
            // else no settings file and quit
            else
            {
                Console.WriteLine("EXITING WITH ERROR! No settings file supplied");
                Console.WriteLine(" execsql.exe <file.xml>");
                Environment.Exit(1);
            }


            // loop over the commands
            foreach (XmlNode command in settings["execsql"])
            {

                // make sure we are in a success state
                if (Program.Success)
                {

                    // collect stats
                    DateTime cmdStartTime = DateTime.Now;

                    // feedback to console
                    Console.WriteLine("");
                    Console.WriteLine(command.Attributes["name"].Value);

                    // create a threadpool
                    ThreadPoolWait threads = new ThreadPoolWait();
                    
                    // check the command has the right values
                    if (command.Name == "exec" && command.Attributes.GetNamedItem("source") != null)
                    {

                        // loop over the command
                        foreach (XmlNode item in command)
                        {
                            Exec exec = new Exec(command.Attributes["source"].Value, item.InnerText);
                            threads.QueueUserWorkItem(new WaitCallback(exec.Run));
                        }

                        // wait for all threads to complete
                        threads.WaitOne();

                        // feedback on duration
                        DateTime cmdStopTime = DateTime.Now;
                        TimeSpan cmdDuration = cmdStopTime - cmdStartTime;
                        Console.WriteLine("=" + String.Format("{0,10:0.000}", cmdDuration.TotalSeconds) + "s ");

                    }
                    else
                    {
                        Console.WriteLine("! ERROR: Incorrect Config XML format.");
                        Program.Success = false;
                    }
                }

            }

            // feedback on duration
            DateTime stopTime = DateTime.Now;
            TimeSpan duration = stopTime - startTime;
            Console.WriteLine("");
            if (Success)
            {
                Console.WriteLine("All Completed in " + duration.TotalSeconds + "s");
            }
            else
            {
                Console.WriteLine("EXITING WITH ERROR! (" + duration.TotalSeconds + " sec)");
                Environment.Exit(1);
            }

        }

    }
}
