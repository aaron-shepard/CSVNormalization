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
        private TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        private TimeZoneInfo pstZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

        public string Timestamp { get; set; }
        public string Address { get; set; }
        public string ZIP { get; set; }
        public string FullName { get; set; }
        public string FooDuration { get; set; }
        public string BarDuration { get; set; }
        public string TotalDuration { get; set; }
        public string? Notes { get; set; }

        public void NormalizeData()
        {
            // TODO - change to return if failure
            this.ConvertTimestampToRFC3339();
            this.NormalizeZip();
            this.NormalizeFullName();
            this.FooDuration = this.NormalizeDuration(this.FooDuration);
            this.BarDuration = this.NormalizeDuration(this.BarDuration);
            this.NormalizeTotalDuration();
        }

        private void ConvertTimestampToRFC3339()
        {
            // assume comes in PST
            // TODO -  ISO 8601

            DateTime dt;
            if (!DateTime.TryParse(this.Timestamp, out dt))
            {
                // TODO - handle parse failure
            }

            dt = TimeZoneInfo.ConvertTime(dt, pstZone);
            dt = TimeZoneInfo.ConvertTime(dt, estZone);

            this.Timestamp = dt.ToString("yyyy-MM-dd HH:mm:ss.fffzzz");
        }

        private void NormalizeZip()
        {
            while (this.ZIP.Length < 5)
            {
                this.ZIP = "0" + this.ZIP;
            }
        }

        private void NormalizeFullName()
        {
            this.FullName = this.FullName.ToUpper();
        }

        private string NormalizeDuration(string duration)
        {
            string[] parts = duration.Split(':');
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            // seconds includes MS as decimal, need to extract
            double seconds = double.Parse(parts[2]);
            int milliseconds = (int)(((decimal)seconds % 1) * 1000);

            TimeSpan ts = new TimeSpan(0, hours, minutes, (int)seconds, milliseconds);

            return ts.TotalSeconds.ToString();
        }

        private void NormalizeTotalDuration()
        {
            double fooVal, barVal;
            this.TotalDuration = "0";
            if (Double.TryParse(this.FooDuration, out fooVal) && Double.TryParse(this.BarDuration, out barVal))
            {
                double totalVal = fooVal + barVal;
                this.TotalDuration = String.Format("{0:0.###}", totalVal);
            }
            // TODO - cannot add values
        }
    }
}
