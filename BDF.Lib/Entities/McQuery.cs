using System;
using System.Collections.Generic;
using System.Text;

namespace BDF.Lib.Entities
{
    public class McQuery
    {
        public string ip { get; set; }
        public int port { get; set; }
        public Debug debug { get; set; }
        public Motd motd { get; set; }
        public Players players { get; set; }
        public string version { get; set; }
        public bool online { get; set; }
        public int protocol { get; set; }
        public string hostname { get; set; }
        public string icon { get; set; }
        public string software { get; set; }
        public Plugins plugins { get; set; }
    }

    public class Debug
    {
        public bool ping { get; set; }
        public bool query { get; set; }
        public bool srv { get; set; }
        public bool querymismatch { get; set; }
        public bool ipinsrv { get; set; }
        public bool cnameinsrv { get; set; }
        public bool animatedmotd { get; set; }
        public int cachetime { get; set; }
        public int apiversion { get; set; }
    }

    public class Motd
    {
        public IList<string> raw { get; set; }
        public IList<string> clean { get; set; }
        public IList<string> html { get; set; }
    }

    public class Players
    {
        public int online { get; set; }
        public int max { get; set; }
    }

    public class Plugins
    {
        public IList<string> names { get; set; }
        public IList<string> raw { get; set; }
    }

}
