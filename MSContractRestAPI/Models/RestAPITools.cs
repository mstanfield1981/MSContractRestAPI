using System;
using System.Collections.Generic;
using System.Web.Http;
using Npgsql;
using System.Configuration;
using System.Net.Http;
using System.Net;

namespace MSContractRestAPI.Models
{
    public class RestAPITools
    {
        public ContractResponse findContracts()
        {
            List<Contract> contracts;

            using (var conn = createConnection())
            {
                try
                {
                    contracts = parseResults(executeSQLStatement(new SQLStatements().selectApprovedContracts(), conn));
                }
                catch (Exception e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound);
                    resp.Content = new StringContent(e.Message);

                    throw new HttpResponseException(resp);
                }
            }

            return new ContractResponse { contracts = contracts, isSuccessful = true };
        }
        public ContractResponse findContracts(int _contractId)
        {
            List<Contract> contracts;

            using (var conn = createConnection())
            {
                try
                {
                    contracts = parseResults(executeSQLStatement(new SQLStatements().selectSingleContract(_contractId), conn));
                }
                catch (Exception e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound);
                    resp.Content = new StringContent(e.Message);

                    throw new HttpResponseException(resp);
                }
            }

            return new ContractResponse { contracts = contracts, isSuccessful = true };
        }
        public ContractInsertResponse createContract(ContractRequest _request)
        {
            ContractInsertResponse response = new ContractInsertResponse();

            using (var conn = createConnection())
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    try
                    {
                        cmd.Connection = conn;
                        SQLStatements sqlStatements = new SQLStatements();
                        cmd.CommandText = sqlStatements.insertSqlStatement(_request);
                        cmd.ExecuteNonQuery();

                        response.insertSuccessful = true;
                        response.originalRequest = _request;
                    }
                    catch (Exception e)
                    {
                        var resp = new HttpResponseMessage(HttpStatusCode.Conflict);
                        resp.Content = new StringContent(String.Format("Error inserting contract. {0}", e.Message));

                        throw new HttpResponseException(resp);
                    }
                }
            }
            return response;
        }
        public ContractResponse updateContract(ContractRequest _request)
        {
            ContractResponse response = new ContractResponse();

            using (var conn = createConnection())
            {
                conn.Open();

                try
                {
                    using (var cmd = new NpgsqlCommand(new SQLStatements().updateContract(_request), conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        response = validateDBTransaction(reader, _request.Id);
                    }
                }
                catch (Exception e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    resp.Content = new StringContent(e.Message);

                    throw new HttpResponseException(resp);
                }
            }

            return response;
        }
        public ContractResponse deleteContract(int id)
        {
            ContractResponse response = new ContractResponse();

            using (var conn = createConnection())
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(new SQLStatements().deleteContract(id), conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        response = validateDBTransaction(reader, id);
                    }
                }
                catch (Exception e)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    resp.Content = new StringContent(e.Message);
                    throw new HttpResponseException(resp);
                }
            }

            return response;
        }
        private NpgsqlConnection createConnection()
        {
            return new NpgsqlConnection(ConfigurationManager.AppSettings["PostGresNextGear"]);
        }
        private List<Contract> parseResults(NpgsqlDataReader reader)
        {
            List<Contract> contracts = new List<Contract>();

            while (reader.Read())
            {
                contracts.Add(new Contract
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    BusinessNumber = reader.GetInt32(2),
                    AmountRequested = reader.GetDouble(3),
                    ActivationDate = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                    ApprovalStatus = reader.GetString(5)
                });
            }

            return contracts;
        }
        private ContractResponse validateDBTransaction(NpgsqlDataReader reader, int contractId)
        {
            ContractResponse response = new ContractResponse();

            if (reader.HasRows)
            {
                response.isSuccessful = true;
            }
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);

                throw new HttpResponseException(resp);
            }
            return response;
        }

        private NpgsqlDataReader executeSQLStatement(string sqlStatement, NpgsqlConnection conn)
        {
            conn.Open();

            using (var cmd = new NpgsqlCommand(sqlStatement, conn))
            {
                return cmd.ExecuteReader();
            }
        }
    }
}