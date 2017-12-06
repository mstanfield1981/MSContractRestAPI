using System;

namespace MSContractRestAPI.Models
{
    public class SQLStatements
    {
        public string insertSqlStatement(ContractRequest _request)
        {
            ContractRequest request = _request;
            String ret;
            if (request.AmountRequested < 50000)
            {
                ret = string.Format("INSERT INTO Contracts(CONTRACTID, NAME, BUSINESSNUMBER, AMOUNTREQUESTED, STATUS, ACTIVATIONDATE) VALUES({0},'{1}',{2},{3},'Approved','{4}')", _request.Id,
                                                                                                                                                                                    _request.Name,
                                                                                                                                                                                    _request.BusinessNumber,
                                                                                                                                                                                    _request.AmountRequested,
                                                                                                                                                                                    DateTime.Now);
            }
            else
            {
                ret = string.Format("INSERT INTO Contracts(CONTRACTID, NAME, BUSINESSNUMBER, AMOUNTREQUESTED, STATUS) VALUES({0},'{1}',{2},{3},'TBD') RETURNING *", _request.Id,
                                                                                                                                                                    _request.Name,
                                                                                                                                                                    _request.BusinessNumber,
                                                                                                                                                                    _request.AmountRequested);
            }

            return ret;
        }
        public string selectApprovedContracts()
        {
            return "SELECT CONTRACTID, NAME, BUSINESSNUMBER, AMOUNTREQUESTED,ACTIVATIONDATE, STATUS FROM CONTRACTS WHERE status = 'Approved'";
        }
        public string selectSingleContract(int Id)
        {
            return string.Format("SELECT CONTRACTID, NAME, BUSINESSNUMBER, AMOUNTREQUESTED,ACTIVATIONDATE, STATUS FROM CONTRACTS WHERE contractId = '{0}'", Id);
        }
        public string deleteContract(int Id)
        {
            return string.Format("DELETE FROM CONTRACTS WHERE CONTRACTID = {0} RETURNING CONTRACTID", Id);
        }
        public string updateContract(ContractRequest _request)
        {
            string ret;

            if (_request.AmountRequested > 50000)
            {
                ret = string.Format("UPDATE CONTRACTS SET NAME = '{0}', BUSINESSNUMBER = {1}, AMOUNTREQUESTED = {2}, ACTIVATIONDATE = DEFAULT, STATUS = 'TBD' WHERE CONTRACTID = {3}  RETURNING CONTRACTID", _request.Name,
                                                                                                                                                                                                                _request.BusinessNumber,
                                                                                                                                                                                                                _request.AmountRequested,
                                                                                                                                                                                                                _request.Id);
            }
            else
            {
                ret = string.Format("UPDATE CONTRACTS SET NAME = '{0}', BUSINESSNUMBER = {1}, AMOUNTREQUESTED = {2}, ACTIVATIONDATE = '{4}', STATUS = 'Approved' WHERE CONTRACTID = {3}  RETURNING CONTRACTID", _request.Name,
                                                                                                                                                                                                                _request.BusinessNumber,
                                                                                                                                                                                                                _request.AmountRequested,
                                                                                                                                                                                                                _request.Id,
                                                                                                                                                                                                                DateTime.Now);
            }

            return ret;
        }
    }
}