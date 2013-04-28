using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiveSenseMonitoring
{
    public class Graph
    {


        public Graph(string Title, string URL, string id)
        {
            // TODO: Complete member initialization
            this.Title = Title;
            this.URL = URL;
            this.id = id;
        }
        public String Title { get; set; }
        public String URL { get; set; }

        public string id { get; set; }
    }
}