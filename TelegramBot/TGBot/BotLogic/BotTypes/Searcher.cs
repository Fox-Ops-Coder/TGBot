using DataAccessLayer.Entities;
using System.Collections.Generic;

namespace TGBot.BotLogic.BotTypes
{
    /// <summary>
    /// Класс хранить выбранные пользователем теги
    /// </summary>
    internal sealed class Searcher
    {
        public readonly long userId;

        private readonly List<int> tagsIds;

        public Searcher(long userId)
        {
            this.userId = userId;
            tagsIds = new List<int>();
        }

        public void AddNewTagId(int tagId) => tagsIds.Add(tagId);

        /// <summary>
        /// Находи и убирает теги, выбранные пользователем
        /// </summary>
        /// <param name="tags">Список тегов</param>
        /// <returns>Список тегов без уже выбранных</returns>
        public List<Tag> FindDifference(List<Tag> tags)
        {
            if (tagsIds.Count != 0)
            {
                List<Tag> ReturnList = new();

                foreach (Tag tag in tags)
                {
                    if (!tagsIds.Contains(tag.TagId)) ReturnList.Add(tag);
                }

                return ReturnList;
            }
            else return tags;
        }

        public List<int> GetTagsIds() => tagsIds;

        public override bool Equals(object obj)
        {
            Searcher searcher = (Searcher)obj;
            if (searcher != null) return searcher.userId == userId;
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}