using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVNormalization
{
    internal class ClientRecord
    {
        private readonly TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        private readonly TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        public string Timestamp { get; set; }
        public string Address { get; set; }
        public string ZIP { get; set; }
        public string FullName { get; set; }
        public string FooDuration { get; set; }
        public string BarDuration { get; set; }
        public string TotalDuration { get; set; }
        public string? Notes { get; set; }

        public bool NormalizeData()
        {
            // return false if any normalization functions fail
            bool success = true;

            success = success && this.ConvertTimestampToRFC3339();
            this.NormalizeZip();
            this.NormalizeFullName();
            this.FooDuration = NormalizeDuration(this.FooDuration);
            success = success && (this.FooDuration != null);
            this.BarDuration = NormalizeDuration(this.BarDuration);
            success = success && this.NormalizeTotalDuration();

            return success;
        }

        private bool ConvertTimestampToRFC3339()
        {
            // assume comes in PST
            // TODO -  ISO 8601 compliant as well?
            // this check should be in BL lib, prefereably pass the timezones as args

            DateTime dt;
            if (!DateTime.TryParse(this.Timestamp, out dt))
            {
                Console.Error.WriteLine("Invalid Timestamp: " + this.Timestamp);
                return false;
            }

            dt = TimeZoneInfo.ConvertTime(dt, pstZone);
            dt = TimeZoneInfo.ConvertTime(dt, estZone);

            this.Timestamp = dt.ToString("yyyy-MM-dd HH:mm:ss.fffzzz");
            return true;
        }

        private void NormalizeZip()
        {
            // TODO - confirm if expect any alpha zip codes, otherwise validate numbers only
            // this check should be in BL lib
            while (this.ZIP.Length < 5)
            {
                this.ZIP = "0" + this.ZIP;
            }
        }

        private void NormalizeFullName()
        {
            this.FullName = this.FullName.ToUpper();
        }

        private static string? NormalizeDuration(string duration)
        {
            try
            {
                // TODO this check should be in BL lib
                string[] parts = duration.Split(':');
                int hours = int.Parse(parts[0]);
                int minutes = int.Parse(parts[1]);
                // seconds includes MS as decimal, need to extract
                double seconds = double.Parse(parts[2]);
                int milliseconds = (int)(((decimal)seconds % 1) * 1000);

                TimeSpan ts = new TimeSpan(0, hours, minutes, (int)seconds, milliseconds);
                return ts.TotalSeconds.ToString();
            } catch(Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine("Invalid duration: " + duration);
                return null;
            }
            
        }

        private bool NormalizeTotalDuration()
        {
            // TODO this check should be in BL lib
            double fooVal, barVal;
            this.TotalDuration = "0";
            if (Double.TryParse(this.FooDuration, out fooVal) && Double.TryParse(this.BarDuration, out barVal))
            {
                double totalVal = fooVal + barVal;
                this.TotalDuration = String.Format("{0:0.###}", totalVal);
                return true;
            }
            return false;
        }
    }
}
