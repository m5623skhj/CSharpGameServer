namespace CSharpGameServer.DB.SPObjects
{
    public class TestSPObject : SPBase
    {
        private int? id = null;
        private string? name = null;

        public TestSPObject() 
        {
            query = "SELECT * FROM tbl WHERE id = {0} AND name = \"{1}\"";
        }
    }
}
