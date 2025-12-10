using CSharpGameServer.etc;

namespace CSharpGameServer.PC.PCComponent
{
    public enum ComponentType
    {
        InvalidComponentType = 0,
    }

    public class ComponentManager
    {
        private Dictionary<ComponentType, ComponentBase> components = new();

        private void AddComponentListForInitialize()
        {
            components.Clear();

            // AddComponents
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

    public abstract class ComponentBase(Pc? inOwner)
    {
        protected Pc? Owner = inOwner;

        public abstract void Initialize();
        public abstract void PreUpdateComponents();
        public abstract void UpdateComponents();
        public abstract void PostUpdateComponents();
    }
}
