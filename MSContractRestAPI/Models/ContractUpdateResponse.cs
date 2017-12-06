using System;
using System.Runtime.Serialization;
namespace MSContractRestAPI.Models
{
    [DataContract]
    public class ContractUpdateResponse
    {
        [DataMember(Name = "Update successful")]
        public Boolean updateSuccessful { get; set; }
        [DataMember(Name = "Update message")]
        public String updateMessage { get; set; }
        [DataMember(Name = "Original request")]
        public ContractRequest originalRequest { get; set; }
    }
}