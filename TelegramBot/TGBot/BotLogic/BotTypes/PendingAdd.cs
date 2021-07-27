namespace TGBot.BotLogic.BotTypes
{
    internal class PendingAdd
    {
        public long id;
        public int professionId;
        public Types type;

        public override bool Equals(object obj)
        {
            PendingAdd pendingAdd = (PendingAdd)obj;
            return pendingAdd != null && pendingAdd.id == id;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}