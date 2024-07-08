namespace CSharpGameServer.DB.SPObjects
{
    public abstract class PCOwnerSPObject : SPBase
    {
        protected PC.PC? owner = null;

        public PCOwnerSPObject(PC.PC inOwner) 
        {
            owner = inOwner;
        }

        public abstract void OnCommit();
        public abstract void OnRollback();
    }

    public class TestSPObject : PCOwnerSPObject
    {
        private int? id = null;
        private string? name = null;

        public TestSPObject(PC.PC inOwner) 
            : base(inOwner)
        {
            query = "SELECT * FROM tbl WHERE id = {0} AND name = \"{1}\"";
        }

        public override void OnCommit()
        {
            // checkt if owner is not null then
            // Callback owner TestSPObject on committed
        }

        public override void OnRollback()
        {
            // check if owner is not null then
            // Callback owner TestSPObject on rollbacked
        }
    }
}
