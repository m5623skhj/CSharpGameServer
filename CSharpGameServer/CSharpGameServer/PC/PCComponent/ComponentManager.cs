namespace CSharpGameServer.PC.PCComponent
{
    public class ComponentManager
    {
        public void Initialize()
        {

        }
    }

    public abstract class ComponentBase
    {
        protected PC? owner = null;

        public ComponentBase(PC? inOwner)
        {
            owner = inOwner;
        }

        abstract public void Initialize();
    }
}
