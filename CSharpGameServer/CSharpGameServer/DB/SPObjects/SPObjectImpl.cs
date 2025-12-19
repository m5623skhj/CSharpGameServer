namespace CSharpGameServer.DB.SPObjects
{
    public abstract class PcOwnerSpObject(PC.Pc inOwner) : SpBase
    {
        protected PC.Pc? Owner = inOwner;
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
            Query = "SELECT * FROM tbl WHERE id = {0} AND name = \"{1}\"";
        }

        public override void OnCommit()
        {
            // check if owner is not null then
            // Callback owner TestSPObject on committed
        }

        public override void OnRollback()
        {
            // check if owner is not null then
            // Callback owner TestSPObject on rollback
        }
    }
}
