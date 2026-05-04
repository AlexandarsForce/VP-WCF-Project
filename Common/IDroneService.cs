using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IDroneService
    {
        [OperationContract]
        void StartSession(SessionData meta);

        [OperationContract]
        void PushSample(DroneSample sample);

        [OperationContract]
        void EndSession();
    }
}
