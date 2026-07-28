using System.Collections.Generic;

namespace streamflix.extractors
{
    public class Video
    {
        public string Source { get; set; } = string.Empty;
        public List<Server> Servers { get; set; } = new();

        public class Server
        {
            public string Name { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
        }
    }
}
