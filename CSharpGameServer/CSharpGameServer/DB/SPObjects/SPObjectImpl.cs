namespace CSharpGameServer.DB.SPObjects
{
    public abstract class PcOwnerSpObject : SpBase
    {
        protected PC.Pc? owner;

        public PcOwnerSpObject(PC.Pc inOwner) 
        {
            owner = inOwner;
        }
    }

    public class TestSpObject : PcOwnerSpObject
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
