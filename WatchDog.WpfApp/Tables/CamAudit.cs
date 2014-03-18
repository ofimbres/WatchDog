using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace WatchDog.WpfApp.Tables
{
    public class CamAudit : TableEntity
    {
        private const string PARTITION_KEY = "AnyPartition";
        public CamAudit(DateTime shortTime, string location, byte[] picture)
        {
            this.ShortTime = shortTime;
            this.Location = location;
            this.Picture = picture;

            this.RowKey = shortTime.ToString("yyyy-MM-dd--hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);
            this.PartitionKey = PARTITION_KEY;
        }

        //public DateTime ShotTime
        //{
        //    get { return DateTime.Now;/*DateTime.Parse(this.RowKey);*/ }
        //    set { this.RowKey = "32"; /*value.ToString();*/ }
        //}
        //public string Location
        //{
        //    get { return this.PartitionKey; }
        //    set { this.PartitionKey = /*value*/ "d"; }
        //}

        public DateTime ShortTime { get; set; }
        public string Location { get; set; }
        public byte[] Picture { get; set; }
    }
}
