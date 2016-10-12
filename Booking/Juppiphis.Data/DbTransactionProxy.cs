using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Juppiphis.Data
{
    public class DbTransactionProxy : DbTransaction
    {
        private IDbTransaction _innerDbTransaction = null;

        internal IDbTransaction Transaction
        {
            get { return _innerDbTransaction; }
        }

        protected override DbConnection DbConnection
        {
            get { return (DbConnection)_innerDbTransaction.Connection; }
        }


        public override IsolationLevel IsolationLevel
        {
            get { return (IsolationLevel)_innerDbTransaction.IsolationLevel; }
        }

        internal DbTransactionProxy(IDbTransaction tran)
        {
            if (tran == null)
                throw new ArgumentNullException("IDbTransaction");
            _innerDbTransaction = tran;
        }

        public override void Commit()
        {
            _innerDbTransaction.Commit();
        }

        public override void Rollback()
        {
            _innerDbTransaction.Rollback();
        }

        protected override void Dispose(bool disposing)
        {
            if (_innerDbTransaction.Connection != null)
                _innerDbTransaction.Connection.Dispose();

            if (_innerDbTransaction != null)
                _innerDbTransaction.Dispose();

            base.Dispose(disposing);
        }
    }
}
