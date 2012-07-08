using System;
using System.Data;
using System.Data.SqlClient;

namespace ExecSQL
{
	class Exec
	{

        private string Source;
        private string SQL;

        public Exec(string source, string sql)
        {
            Source = source;
            SQL = sql;
        }

		public void Run(Object obj)
		{

            // feedback
            DateTime startTime = DateTime.Now;

            try
            {

                // needed vars
                SqlCommand cmd;

                // open connection to source
                SqlConnection sourceconn = new SqlConnection(Source);
                sourceconn.Open();

                // execute the statement
                cmd = new SqlCommand(SQL, sourceconn);
                cmd.CommandTimeout = 3600;
                cmd.ExecuteNonQuery();

                // clean up connections
                sourceconn.Close();

                // feedback
                DateTime stopTime = DateTime.Now;
                TimeSpan duration = stopTime - startTime;
                Console.WriteLine("+" + String.Format("{0,10:0.000}", duration.TotalSeconds) + "s " + SQL);

            }
            catch (Exception ex)
            {
                Console.WriteLine("! ERROR: " + SQL + " " + ex.Message);
                Program.Success = false;
            }
            
		}

	}
}
