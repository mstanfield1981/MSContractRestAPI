using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MSContractRestAPI.Models
{
    [DataContract]
    public class ContractResponse
    {
        [DataMember(Name = "Contracts")]
        public List<Contract> contracts { get; set; }
    }
}