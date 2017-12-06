using System.Runtime.Serialization;

namespace MSContractRestAPI.Models
{
    [DataContract]
    public class ContractRequest
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int BusinessNumber { get; set; }
        [DataMember]
        public double AmountRequested { get; set; }
    }
}


