namespace CSharpGameServer.DB.SPObjects
{
    public abstract class PcOwnerSpObject(PC.Pc inOwner) : SpBase
    {
        protected PC.Pc? Owner = inOwner;
    }

    public class TestSpObject : PcOwnerSpObject
    {
        public TestSpObject(PC.Pc inOwner)
            : base(inOwner)
        {
            Query = "SELECT * FROM tbl WHERE id = @Id AND name = @Name";
        }

        public void SetTestParams(int inId, string inName)
        {
            ClearParameters();
            AddParameter("@Id", inId);
            AddParameter("@Name", inName);
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
