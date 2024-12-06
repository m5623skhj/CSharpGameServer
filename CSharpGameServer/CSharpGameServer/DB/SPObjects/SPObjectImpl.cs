namespace CSharpGameServer.DB.SPObjects
{
    public abstract class PCOwnerSPObject : SpBase
    {
        protected PC.Pc? owner = null;

        public PCOwnerSPObject(PC.Pc inOwner) 
        {
            owner = inOwner;
        }
    }

    public class TestSpObject : PCOwnerSPObject
    {
        private int? id;
        private string? name;

        public TestSpObject(PC.Pc inOwner) 
            : base(inOwner)
        {
            id = null;
            name = null;
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
