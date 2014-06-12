using System;

namespace Aricie.DNN.Modules.PortalKeeper.BitCoin
{
	[Serializable]
	public class ResponseObject
	{
		public string Error;

		public string ReturnCodes;

		public ResponseObject()
		{
			this.Error = "";
			this.ReturnCodes = "";
		}

		public ResponseObject(ResponseObject objResponseObject)
		{
			this.Error = objResponseObject.Error;
			this.ReturnCodes = objResponseObject.ReturnCodes;
		}
	}
}