using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
    [DefaultProperty("DisplayName")]
    [Serializable]
    public class Transaction
    {

        public string Id { get; set; }

        public string Symbol { get; set; }

        public DateTime Time { get; set; }

        public string TimeStamp { get; set; }

        public string TimeZone { get; set; }

        public TransactionType TransactionType { get; set; }

        public string Address { get; set; }

        public decimal Amount { get; set; }

        public decimal Fee { get; set; }

        [Browsable(false)]
        [XmlIgnore]
        public virtual string DisplayName
        {
            get
            {
                return string.Format("{0} {1} - {2} : {3} {4} {5},  Address: {6}, Fee: {7}",
                    Time.ToShortDateString(), Time.ToShortTimeString(), TransactionType, Amount.ToString(CultureInfo.InvariantCulture), Symbol,
                    Address, Fee.ToString(CultureInfo.InvariantCulture));
            }
        }

    }
}