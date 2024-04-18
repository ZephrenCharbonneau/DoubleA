using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoubleA.Models
{
    public class UserSettings
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string DefaultListSource { get; set; }
    }
}
