using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipX.Enums
{
    public class ClipPeriod
    {
        public string Value { get; private set; }
        private ClipPeriod(string value)
        {
            Value = value;
        }

        public static ClipPeriod Day { get { return new ClipPeriod("Day"); } }
        public static ClipPeriod Week { get { return new ClipPeriod("Week"); } }
        public static ClipPeriod Month { get { return new ClipPeriod("Month"); } }
        public static ClipPeriod All { get { return new ClipPeriod("All"); } }

        public override string ToString() => Value;
    }
}
