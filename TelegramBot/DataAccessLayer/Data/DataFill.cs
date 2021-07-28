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

                    Tag[] tags = new Tag[]
                    {
                        new Tag
                        {
                            TagName = "Node.js"
                        },

                        new Tag
                        {
                            TagName = "TypeScript"
                        },

                        new Tag
                        {
                            TagName = "HTML"
                        },

                        new Tag
                        {
                            TagName = "C#"
                        },

                        new Tag
                        {
                            TagName = "Python"
                        },

                        new Tag
                        {
                            TagName = "Entity Core"
                        },

                        new Tag
                        {
                            TagName = "Django"
                        }
                    };

                    tGContext.Professions.AddRange(professions);
                    tGContext.Tags.AddRange(tags);
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