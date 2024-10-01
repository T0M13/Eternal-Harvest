using System.Collections.Generic;

public class TileProperty
{
    private Dictionary<TilePropertyType, bool> properties;

    public TileProperty()
    {
        properties = new Dictionary<TilePropertyType, bool>();
    }

    public void AddProperty(TilePropertyType propertyType, bool value)
    {
        if (!properties.ContainsKey(propertyType))
        {
            properties[propertyType] = value;
        }
        else
        {
            properties[propertyType] = value; 
        }
    }

    public void RemoveProperty(TilePropertyType propertyType)
    {
        if (properties.ContainsKey(propertyType))
        {
            properties.Remove(propertyType);
        }
    }

    public bool HasProperty(TilePropertyType propertyType)
    {
        return properties.ContainsKey(propertyType);
    }

    public bool GetProperty(TilePropertyType propertyType)
    {
        if (properties.ContainsKey(propertyType))
        {
            return properties[propertyType];
        }
        return false;  
    }

    public void SetProperty(TilePropertyType propertyType, bool value)
    {
        if (properties.ContainsKey(propertyType))
        {
            properties[propertyType] = value;
        }
    }
    public Dictionary<TilePropertyType, bool> GetAllProperties()
    {
        return properties;
    }

    public void ClearAllProperties()
    {
        properties.Clear();
    }
}
