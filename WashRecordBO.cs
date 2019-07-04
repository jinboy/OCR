using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YunZhi
{
    public class WashRecordBO
    {
        public string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public string plate;

        public string Plate
        {
            get { return plate; }
            set { plate = value; }
        }
        public string washTime;

        public string WashTime
        {
            get { return washTime; }
            set { washTime = value; }
        }
    }
}
