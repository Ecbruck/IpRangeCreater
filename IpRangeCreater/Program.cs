using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IpRangeCreater
{
    class Program
    {
        static void Main(string[] args)
        {
            string contents;
            using (var wc = new System.Net.WebClient())
                contents = wc.DownloadString(@"http://ftp.apnic.net/apnic/stats/apnic/delegated-apnic-latest");


            var records1 = contents.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);


            var records2 = records1.Select(r => IpRegionRecord.Parse(r)).Where(r => r != null);


            var records3 = records2.Where(r => r.Protocol == "ipv4" && r.Region == "CN").Select(r => r.ToString());


            System.IO.File.WriteAllLines("IpRange.txt", records3);
        }
    }

    class IpRegionRecord
    {
        public string Region { get; private set; }
        public string Protocol { get; private set; }
        public string Address { get; private set; }
        public int Range { get; private set; }

        static public IpRegionRecord Parse(string record)
        {
            string[] contents = record.Split('|');
            if (contents.Length != 7)
                return null;

            IpRegionRecord result = new IpRegionRecord();

            if (string.IsNullOrWhiteSpace(contents[1]))
                return null;
            else
                result.Region = contents[1];

            if (string.IsNullOrWhiteSpace(contents[2]))
                return null;
            else
                result.Protocol = contents[2];

            if (string.IsNullOrWhiteSpace(contents[3]))
                return null;
            else
                result.Address = contents[3];

            int range;
            if (string.IsNullOrWhiteSpace(contents[4]) || !int.TryParse(contents[4], out range))
                return null;
            else
                result.Range = range;

            return result;
        }

        public override string ToString()
        {
            int rangeExponent = (int)Math.Floor(Math.Log(Range) / Math.Log(2));
            if (rangeExponent < 1)
                return null;

            return string.Format("{0}/{1}", Address, 32 - rangeExponent);
        }
    }
}
