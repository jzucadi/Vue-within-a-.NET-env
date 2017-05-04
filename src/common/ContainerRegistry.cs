using StructureMap;
using Toucan.Contract;

namespace Toucan.Common
{
    public class ContainerRegistry : Registry
    {
        public ContainerRegistry()
        {
            For<ICryptoService>().Use<CryptoHelper>();
        }
    }
}