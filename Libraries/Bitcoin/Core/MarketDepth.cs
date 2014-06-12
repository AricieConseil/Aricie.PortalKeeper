using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
    [Serializable]
	public class MarketDepth : ResponseObject, ICloneable
	{

        public MarketDepth()
        {
            Init();
        }

        public MarketDepth(ResponseObject objresponseObject)
            : base(objresponseObject)
        {
            Init();
        }

        public MarketDepth(MarketDepth cloneDepth) : this(cloneDepth, false)
        {
            
        }

        public MarketDepth(MarketDepth cloneDepth, bool deepCopy)
        {
            if (cloneDepth != null)
            {
                if (deepCopy)
                {
                    BidOrders = new List<Order>(cloneDepth.BidOrders);
                    AskOrders = new List<Order>(cloneDepth.AskOrders);
                }
                else
                {
                    BidOrders = cloneDepth.BidOrders;
                    AskOrders = cloneDepth.AskOrders;
                }
            }
            else
            {
                Init();
            }
        }

        private void Init()
        {
            BidOrders = new List<Order>();
            AskOrders = new List<Order>();
        }

        public List<Order> AskOrders { get; set; }

        public List<Order> BidOrders { get; set; }

        [XmlIgnore()]
	    public decimal[][] Asks
	    {
	        get
	        {
	            return AskOrders.Select(objOrder => new decimal[3] {objOrder.Price, objOrder.Amount, objOrder.Date}).ToArray();
	        }
	        set
	        {
	            foreach (var triplet in value)
	            {
                    if (triplet.Length <= 2 || triplet[2]<=0)
	                {
                        AskOrders.Add(new Order(OrderType.Sell, triplet[0], triplet[1]));    
	                }
                    else
                    {
                        AskOrders.Add(new Order(OrderType.Sell, triplet[0], triplet[1],
                            Common.ConvertFromUnixTimestamp(Convert.ToInt64( triplet[2]))));
                    }
	                
	            }
	            
	        }
	    }

        [XmlIgnore()]
	    public decimal[][] Bids
	    {
            get
            {
                return BidOrders.Select(objOrder => new decimal[3] { objOrder.Price, objOrder.Amount, objOrder.Date }).ToArray();
            }
            set
            {
                foreach (var triplet in value)
                {
                    if (triplet.Length <= 2 || triplet[2] <= 0)
                    {
                        BidOrders.Add(new Order(OrderType.Sell, triplet[0], triplet[1]));
                    }
                    else
                    {
                        BidOrders.Add(new Order(OrderType.Sell, triplet[0], triplet[1],
                            Common.ConvertFromUnixTimestamp(Convert.ToInt64(triplet[2]))));
                    }

                }

            }
	    }

        public object Clone()
        {
            return new MarketDepth(this, true);
        }
	}
}