using System;
using System.Runtime.Serialization;

namespace MSContractRestAPI.Models
{
    [DataContract(Name = "Contract")]
    public class Contract
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int BusinessNumber { get; set; }
        [DataMember]
        public DateTime ActivationDate { get; set; }
        [DataMember]
        public double AmountRequested { get; set; }
        [DataMember]
        public string ApprovalStatus { get; set; }
    }
}