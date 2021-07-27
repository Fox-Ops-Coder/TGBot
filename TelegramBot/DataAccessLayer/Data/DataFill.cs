using DataAccessLayer.Entities;
using DataAccessLayer.Model;
using System.Linq;

namespace DataAccessLayer.Data
{
    public sealed class DataFill
    {
        public static bool CreateDb(TGContext tGContext)
        {
            try
            {
                tGContext.Database.EnsureCreated();

                if (!tGContext.Professions.Any())
                {
                    Profession[] professions = new Profession[]
                    {
                        new Profession
                        {
                            ProfessionName = "FRONTEND-РАЗРАБОТЧИК"
                        },

                        new Profession
                        {
                            ProfessionName = "BACKEND-РАЗРАБОТЧИК"
                        }
                    };

                    tGContext.AddRange(professions);
                    tGContext.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}