using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http;
using MSContractRestAPI.Models;


namespace MSContractRestAPI.Controllers
{
    public class ContractsController : ApiController
    {
        public ContractResponse Get()
        {
            RestAPITools apiTools = new RestAPITools();

            return apiTools.findContracts();
        }
        public ContractResponse Get(int id)
        {

            RestAPITools apiTools = new RestAPITools();

            return apiTools.findContracts(id);
        }
        public ContractInsertResponse Post(ContractRequest _request)
        {
            RestAPITools apiTools = new RestAPITools();

            return apiTools.createContract(_request);
        }
        public ContractUpdateResponse Put(ContractRequest _request)
        {
            RestAPITools apiTools = new RestAPITools();

            return apiTools.updateContract(_request);
        }
        public ContractDeleteResponse Delete(int id)
        {
            RestAPITools apiTools = new RestAPITools();

            return apiTools.deleteContract(id);
        }
    }
}
