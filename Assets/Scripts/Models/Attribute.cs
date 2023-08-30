using System.Collections.Generic;

[System.Serializable]
public class AttributeModel
{
    public string position;
    public List<string> altPosition;
    public int rating;
    public List<AttributeLabelValue> attribute;
}

[System.Serializable]
public class AttributeLabelValue
{
    public string label;
    public int value;
}