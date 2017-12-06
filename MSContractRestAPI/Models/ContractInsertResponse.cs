using System;
using System.Runtime.Serialization;

namespace MSContractRestAPI.Models
{
    [DataContract]
    public class ContractInsertResponse
    {
        [DataMember(Name = "Insert successful")]
        public Boolean insertSuccessful { get; set; }
        [DataMember(Name = "Original request")]
        public ContractRequest originalRequest { get; set; }
    }
}