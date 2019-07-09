using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface ITransactionRepository
    {
        Task AddTransactionAsync(TransactionDetail transactionDetail);
    }
}
