using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KramerUwp.App.Api
{
    public class Program
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Playlist
    {
        public int duration { get; set; }
        public DateTime publishdateutc { get; set; }
        public int id { get; set; }
        public string url { get; set; }
        public string statkey { get; set; }
    }

    public class Broadcastfile
    {
        public int duration { get; set; }
        public DateTime publishdateutc { get; set; }
        public int id { get; set; }
        public string url { get; set; }
        public string statkey { get; set; }
    }

    public class Broadcast
    {
        public Playlist playlist { get; set; }
        public List<Broadcastfile> broadcastfiles { get; set; }
    }

    public class Program2
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Downloadpodfile
    {
        public string title { get; set; }
        public string description { get; set; }
        public int filesizeinbytes { get; set; }
        public Program2 program { get; set; }
        public int duration { get; set; }
        public DateTime publishdateutc { get; set; }
        public int id { get; set; }
        public string url { get; set; }
        public string statkey { get; set; }
    }

    public class Episode
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string imageurl { get; set; }
        public Program program { get; set; }
        public DateTime publishdateutc { get; set; }
        public Broadcast broadcast { get; set; }
        public Downloadpodfile downloadpodfile { get; set; }
    }

    public class RootObject
    {
        public string copyright { get; set; }
        public List<Episode> episodes { get; set; }
    }

}
