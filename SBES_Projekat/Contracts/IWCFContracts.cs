﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IWCFContracts
    {
		[OperationContract]
		void TestCommunication();
		[OperationContract]
		void SendMessage(string message);

        [OperationContract]
        string Read();
        [OperationContract]
        bool Write(string text);
    }
}

