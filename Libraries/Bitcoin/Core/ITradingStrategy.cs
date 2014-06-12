namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	public interface ITradingStrategy
	{
		Wallet ComputeNewOrders(Wallet currentOrders, MarketInfo objMarket, ExchangeInfo objExchange, TradingHistory history);
	}
}