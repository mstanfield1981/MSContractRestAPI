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

            return new ContractResponse { contracts = contracts };
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

            return new ContractResponse { contracts = contracts };
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
        public ContractUpdateResponse updateContract(ContractRequest _request)
        {
            ContractUpdateResponse response = new ContractUpdateResponse();

            using (var conn = createConnection())
            {
                conn.Open();

                try
                {
                    using (var cmd = new NpgsqlCommand(new SQLStatements().updateContract(_request), conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        response = validateUpdate(reader, _request.Id);

                        response.originalRequest = _request;
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
        public ContractDeleteResponse deleteContract(int id)
        {
            ContractDeleteResponse response = new ContractDeleteResponse();

            using (var conn = createConnection())
            {
                try
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(new SQLStatements().deleteContract(id), conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        response = validateDelete(reader, id);
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
        private ContractUpdateResponse validateUpdate(NpgsqlDataReader reader, int contractId)
        {
            ContractUpdateResponse response = new ContractUpdateResponse();

            if (reader.HasRows)
            {
                response.updateSuccessful = true;
            }
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                resp.Content = new StringContent(String.Format("Error updating contract. {0}", contractId));

                throw new HttpResponseException(resp);
            }
            return response;
        }
        private ContractDeleteResponse validateDelete(NpgsqlDataReader reader, int contractId)
        {
            ContractDeleteResponse response = new ContractDeleteResponse();

            if (reader.HasRows)
            {
                response.deleteSuccessful = true;
                response.deleteMessage = String.Format("Contract Id {0} deleted.", contractId);
            }
            else
            {
                var resp = new HttpResponseMessage(HttpStatusCode.BadRequest);
                resp.Content = new StringContent(String.Format("Error updating contract. {0}", contractId));

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