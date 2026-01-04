using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2GnowTakehomeExercise.DataAccess.DbManager
{
    public sealed class SingletonSqlServerDbManager
    {
        private static readonly ISqlServerDbManager _instance = new SqlServerDbManager();
        private static ISqlServerDbManager _injectedInstance;

        static SingletonSqlServerDbManager()
        {
        }
        private SingletonSqlServerDbManager()
        {
        }
        public static ISqlServerDbManager Instance
        {
            get
            {
                if (_injectedInstance != null)
                {
                    return _injectedInstance;
                }
                return _instance;
            }
        }

        public static void SetInstance(ISqlServerDbManager toSet)
        {
            _injectedInstance = toSet;
        }
    }
}
