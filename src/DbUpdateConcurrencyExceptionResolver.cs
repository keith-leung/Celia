using SharpCC.UtilityFramework.Loggings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCC.UtilityFramework
{
    public class DbUpdateConcurrencyExceptionResolver
    {
        public static void SaveAndResolveExceptionServerWin(DbContext context)
        {
            bool saveFailed;
            int counter = 0;
            do
            {
                if (counter > 50)
                    break;

                saveFailed = false;
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    LogHelper.Error(ex.Message, ex);
                    if (counter % 2 == 0)
                    {
                        // Update original values from the database 
                        var entry = ex.Entries.Single();
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                    else
                        ex.Entries.Single().Reload();
                    counter++;
                }
            } while (saveFailed);
        }
    }
}
