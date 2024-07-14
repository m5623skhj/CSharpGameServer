namespace CSharpGameServer.PC.PCComponent
{
    public enum ComponentType : int
    {
        InvalidComponentType = 0,
    }

    public class ComponentManager
    {
        private Dictionary<ComponentType, ComponentBase> components = new Dictionary<ComponentType, ComponentBase>();

        private void AddComponentListForInitialize()
        {
            components.Clear();

            // AddComponent
        }

        public void Initialize()
        {
            AddComponentListForInitialize();

            foreach (var component in components.Values)
            {
                component.Initialize();
            }
        }

        public ComponentBase? GetComponent(ComponentType inComponentType)
        {
            return components.GetValueOrNull(inComponentType);
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
