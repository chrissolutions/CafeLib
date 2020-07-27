using System;
using System.Collections.Generic;
using System.Text;
using CafeLib.Core.Dynamic;

namespace CafeLib.Core.UnitTests.DynamicModels
{
    public class User : Expando
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime? ExpiresOn { get; set; }

        public User() : base()
        {
        }

        // only required if you want to mix in seperate instance
        public User(object instance)
            : base(instance)
        {
        }
    }
}
