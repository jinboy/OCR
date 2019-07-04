using System;
using System.Collections.Generic;
using System.Text;

namespace YunZhi
{
    public class PlateBO
    {
        public string id;
        public string plate;
        public string department;
        public string name;
        public string phone;
        public int carWash;
        public string createTime;
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Plate
        {
            get { return plate; }
            set { plate = value; }
        }
        public string Department
        {
            get { return department; }
            set { department = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }
        public int CarWash
        {
            get { return carWash; }
            set { carWash = value; }
        }
        public string CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }
    }
}
