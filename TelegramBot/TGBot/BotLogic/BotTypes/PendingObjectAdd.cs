using DataAccessLayer.Entities;
using System;

namespace TGBot.BotLogic.BotTypes
{
    internal class PendingObjectAdd
    {
        public Guid guid;
        public long id;
        public object addObject;

        public Tag[] Tags;

        public Types type;

        public PendingObjectAdd()
        {
            Tags = Array.Empty<Tag>();
        }
    }
}