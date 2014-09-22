using Aricie.DNN.UI.Attributes;
using Aricie.DNN.UI.WebControls.EditControls;
using DotNetNuke.UI.WebControls;
//using Jayrock.Json;
//using Jayrock.Json.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class SimulationData
	{
		private string _JsonWallet;

		private string _JsonTicker;

		private string _JsonMarketDepth;

		[Editor(typeof(CustomTextEditControl), typeof(EditControl))]
		[LineCount(8)]
		[Width(500)]
		public string JsonMarketDepth
		{
			get
			{
				return this._JsonMarketDepth;
			}
			set
			{
				this._JsonMarketDepth = value;
			}
		}

		[Editor(typeof(CustomTextEditControl), typeof(EditControl))]
		[LineCount(8)]
		[Width(500)]
		public string JsonTicker
		{
			get
			{
				return this._JsonTicker;
			}
			set
			{
				this._JsonTicker = value;
			}
		}

		[Editor(typeof(CustomTextEditControl), typeof(EditControl))]
		[LineCount(8)]
		[Width(500)]
		public string JsonWallet
		{
			get
			{
				return this._JsonWallet;
			}
			set
			{
				this._JsonWallet = value;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public MarketInfo Market
		{
			get
			{
                //todo: migrate the following to  Newtonsoft json.net
                //MarketDepth objMarketDepth;
                //TickerInfo objTickerInfo;
                //Jayrock.Json.Conversion.ImportContext impContext = new Jayrock.Json.Conversion.ImportContext();
                //impContext.Register(impContext.FindImporter(typeof(Ticker)));
                //using (StringReader reader = new StringReader(this._JsonTicker))
                //{
                //    objTickerInfo = (TickerInfo)impContext.Import(typeof(TickerInfo), JsonText.CreateReader(reader));
                //}
                //impContext = new Jayrock.Json.Conversion.ImportContext();
                //using (StringReader reader = new StringReader(this._JsonMarketDepth))
                //{
                //    objMarketDepth = (MarketDepth)impContext.Import(typeof(MarketDepth), JsonText.CreateReader(reader));
                //}
                //return new MarketInfo(objTickerInfo.ticker, objMarketDepth);
                return new MarketInfo();
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public Aricie.DNN.Modules.PortalKeeper.BitCoin.Wallet Wallet
		{
			get
			{
                //todo: migrate the following to  Newtonsoft json.net
				Aricie.DNN.Modules.PortalKeeper.BitCoin.Wallet toReturn = new Wallet();
                //Jayrock.Json.Conversion.ImportContext impContext = new Jayrock.Json.Conversion.ImportContext();
                //impContext.Register(impContext.FindImporter(typeof(List<Order>)));
                //using (StringReader reader = new StringReader(this._JsonWallet))
                //{
                //    toReturn = (Aricie.DNN.Modules.PortalKeeper.BitCoin.Wallet)impContext.Import(typeof(Aricie.DNN.Modules.PortalKeeper.BitCoin.Wallet), JsonText.CreateReader(reader));
                //}
				return toReturn;
			}
		}

		[DebuggerNonUserCode]
		public SimulationData()
		{
		}
	}
}