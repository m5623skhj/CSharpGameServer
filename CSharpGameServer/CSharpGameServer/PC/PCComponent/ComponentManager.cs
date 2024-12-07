using CSharpGameServer.etc;

namespace CSharpGameServer.PC.PCComponent
{
    public enum ComponentType
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
            return components!.GetValueOrNull(inComponentType);
        }

        public void UpdateComponents()
        {
            foreach (var component in components.Values)
            {
                component.PreUpdateComponents();
            }

            foreach (var component in components.Values)
            {
                component.UpdateComponents();
            }

            foreach (var component in components.Values)
            {
                component.PostUpdateComponents();
            }
        }
    }

    public abstract class ComponentBase
    {
        protected Pc? owner;

        public ComponentBase(Pc? inOwner)
        {
            owner = inOwner;
        }

        abstract public void Initialize();
        abstract public void PreUpdateComponents();
        abstract public void UpdateComponents();
        abstract public void PostUpdateComponents();
    }
}
