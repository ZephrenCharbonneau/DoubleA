using System;
using System.Collections.Generic;
using System.Text;

namespace DoubleA.HTTP
{
    public class GraphQLQuery
    {
        public string query { get; set; }
        public Dictionary<string, object> variables { get; set; }
    }
}
