using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aricie.Collections;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin.Exchanges
{

    public class CryptsyResponseBase
    {
        public int Success { get; set; }
        public string Error { get; set; }
    }

    public class CryptsyDepthResponse : CryptsyResponseBase
    {
        
        public CryptsyMarketsDictionary @Return { get; set; }
    }

    public class CryptsyWalletResponse : CryptsyResponseBase
    {
        public CryptsyWallet @Return { get; set; }
    }

    public class CryptsyTickerResponse : CryptsyResponseBase
    {
        public CryptsyTickerReturn @Return { get; set; }
    }

    public class CryptsyMarketsResponse : CryptsyResponseBase
    {
        public List<CryptsyActiveMarket> @Return { get; set; }
    }

    public class CryptsyTickerReturn
    {
        public CryptsyMarketsDictionary Markets { get; set; }
    }

    public class CryptsyMarketsDictionary :
        SerializableDictionary<String, CryptsyMarket>
    {

    }

    public class CryptsyWallet
    {
        public CryptsyBalances Balances_Available { get; set; }
        public CryptsyBalances Balances_Hold { get; set; }
        public int ServerTimeStamp { get; set; }
        public string ServerTimeZone { get; set; }
        public DateTime ServerDateTime { get; set; }
        public int OpenOrderCount { get; set; }

        public SerializableDictionary<string, Wallet> ToWalletDictionary()
        {
            var toReturn = new SerializableDictionary<string, Wallet>();
            foreach (var objBalance in Balances_Available)
            {
                var objWallet = new Wallet();
                objWallet.PrimaryBalance = objBalance.Value;
                toReturn.Add(objBalance.Key, objWallet);
            }
            return toReturn;
        }
    }

    public class CryptsyBalances :
     SerializableDictionary<String, decimal>
    {

    }

    public class CryptsyActiveMarket
    {
        public string marketid { get; set; }
        public string label { get; set; }
        public string primary_currency_code { get; set; }
        public string primary_currency_name { get; set; }
        public string secondary_currency_code { get; set; }
        public string secondary_currency_name { get; set; }
        public decimal current_volume { get; set; }
        public decimal last_trade { get; set; }
        public decimal high_trade { get; set; }
        public decimal low_trade { get; set; }
        public DateTime created { get; set; }
    }


    public class CryptsyMarket
    {

        public CryptsyMarket()
        {
            RecentTrades = new List<CryptsyTrade>();
            SellOrders = new List<CryptsyOrder>();
            BuyOrders = new List<CryptsyOrder>();
        }

        public string Marketid { get; set; }
        public string Label { get; set; }
        public decimal LastTradePrice { get; set; }
        public decimal Volume { get; set; }
        public DateTime LastTradeTime { get; set; }
        public string PrimaryName { get; set; }
        public string PrimaryCode { get; set; }
        public string SecondaryName { get; set; }
        public string SecondaryCode { get; set; }
        public List<CryptsyTrade> RecentTrades { get; set; }
        public List<CryptsyOrder> SellOrders { get; set; }
        public List<CryptsyOrder> BuyOrders { get; set; }


        public MarketInfo ToMarketInfo()
        {
            var toReturn = new MarketInfo();
            toReturn.Id = Marketid;
            toReturn.Label = Label;
            toReturn.Ticker = new Ticker(LastTradePrice, Volume);
            toReturn.PrimaryName = this.PrimaryName;
            toReturn.PrimaryCode = this.PrimaryCode;
            toReturn.SecondaryName = this.SecondaryName;
            toReturn.SecondaryCode = this.SecondaryCode;
            toReturn.Time = LastTradeTime;
            foreach (var objCryptsyOrder in SellOrders)
                toReturn.MarketDepth.AskOrders.Add(objCryptsyOrder.ToOrder(OrderType.Sell));
            foreach (var objCryptsyOrder in BuyOrders)
                toReturn.MarketDepth.BidOrders.Add(objCryptsyOrder.ToOrder(OrderType.Buy));
            foreach (var objCryptsyTrade in this.RecentTrades)
                toReturn.RecentTrades.Add(objCryptsyTrade.ToTrade());
            return toReturn;
        }

    }


    public class CryptsyTrade
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }

        public Trade ToTrade()
        {
            return new Trade() { Id = this.Id, Price = this.Price, Amount = this.Quantity, Time = this.Time };
        }
    }

    public class CryptsyOrder
    {
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Total { get; set; }

        public Order ToOrder(OrderType objType)
        {
            return new Order(objType, Price, Quantity);
        }
    }




}


