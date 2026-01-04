using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2GnowTakehomeExercise.DataAccess.CommandBuilders
{
    public interface ICommandBuilder
    {
        SqlCommand Build<T>(IDbConnection conn, string commandText, T parameterSource);
    }
}
