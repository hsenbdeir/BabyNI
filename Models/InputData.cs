namespace BabyNI.Models
{
    public class InputData
    {
      //  public string NETWORK_SID { get; set; }
        public DateTime DATETIME_KEY { get; set; }
        public string NODENAME { get; set; }
        public float NEID { get; set; }
        public string OBJECT { get; set; }
        public DateTime TIME { get; set; }
        public int? INTERVAL { get; set; }
        public string DIRECTION { get; set; }
        public string NEALIAS { get; set; }
        public string NETYPE { get; set; }
        public string POSITION { get; set; }

        // Fields specific to certain input files
        public double? RXLEVELBELOWTS1 { get; set; }
        public double? RXLEVELBELOWTS2 { get; set; }
        public double? MINRXLEVEL { get; set; }
        public double? MAXRXLEVEL { get; set; }
        public double? TXLEVELABOVETS1 { get; set; }
        public double? MINTXLEVEL { get; set; }
        public double? MAXTXLEVEL { get; set; }
        public string IDLOGNUM { get; set; }
        public string FAILUREDESCRIPTION { get; set; }

        // Fields specific to other input files
        public double? RFINPUTPOWER { get; set; }
        public double? MEANRXLEVEL1M { get; set; }
        public string TID { get; set; }
       public string FARENDTID { get; set; }
     //  public string SLOT { get; set; }
      //  public string PORT { get; set; }
    }
}
