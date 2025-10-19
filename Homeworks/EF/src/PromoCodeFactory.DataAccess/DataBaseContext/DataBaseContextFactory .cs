using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SQLitePCL;

namespace PromoCodeFactory.DataAccess.DataBaseContext
{
    public class DataBaseContextFactory : IDesignTimeDbContextFactory<DataBaseContextSqlLite>
    {
        public DataBaseContextSqlLite CreateDbContext(string[] args)
        {
            Batteries.Init();
            // Создаём билдер для DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<DataBaseContextSqlLite>();
            optionsBuilder.UseSqlite("Data Source=../PromoCodeFactory.DataAccess/DataBase/Otus_HomeWorк");

            return new DataBaseContextSqlLite(optionsBuilder.Options);
        }
    }
}
