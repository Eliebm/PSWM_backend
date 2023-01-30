using System.Data.SqlClient;
using System.Data;

namespace PSWM_backend.Controllers
{
    public class GetSetSPI:IGetSetSPI
    {

        private readonly IConfiguration _configuration;



        public GetSetSPI(IConfiguration configuration)
        {
            _configuration = configuration;
        }




        public List<T> GetSpAllItem<T>(string sp, Func<System.Data.IDataReader, T> itemReader, params object[] sendparameters)
        {
            List<T> list = new List<T>();


            string dbConnection = _configuration.GetValue<string>("ConnectionStrings:dbconnection");

            SqlConnection sqlcon = new(dbConnection);
            sqlcon.Open();

            SqlCommand cmd = new SqlCommand(sp, sqlcon);

            cmd.CommandType = CommandType.StoredProcedure;
            SqlCommandBuilder.DeriveParameters(cmd);
            var counter = 0;
            for (var i = 0; i < cmd.Parameters.Count; i++)
            {
                var paramerter = cmd.Parameters[i];
                if (paramerter.Direction == ParameterDirection.Input)
                {
                    var paramterValue = sendparameters[counter];
                    cmd.Parameters[i].Value = paramterValue != null ? paramterValue : DBNull.Value;
                    counter++;
                }
            }

            SqlDataReader dr = cmd.ExecuteReader();

            try
            {
                while (dr.Read())
                {
                    list.Add(itemReader(dr)

           );
                }

                dr.Close();
                sqlcon.Close();


            }
            catch (SqlException ex) { }



            return list;
        }

        public string PostSpAllItem<T>(string sp, params object[] sendparameters)
        {
            string msg = "";
            try
            {
                string dbConnection = _configuration.GetValue<string>("ConnectionStrings:dbconnection");

                SqlConnection sqlcon = new(dbConnection);
                sqlcon.Open();

                SqlCommand cmd = new SqlCommand(sp, sqlcon);

                cmd.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(cmd);
                var counter = 0;
                for (var i = 0; i < cmd.Parameters.Count; i++)
                {
                    var paramerter = cmd.Parameters[i];
                    if (paramerter.Direction == ParameterDirection.Input)
                    {
                        var paramterValue = sendparameters[counter];
                        cmd.Parameters[i].Value = paramterValue != null ? paramterValue : DBNull.Value;
                        counter++;
                    }
                }

                SqlDataReader dr = cmd.ExecuteReader();


                msg = "1";



                dr.Close();
                sqlcon.Close();


            }
            catch (SqlException ex) {}



            return msg;
        }

    }
}
