using System.Collections.Generic;

[System.Serializable]
public class JsonPosition
{
    public string left;
    public string top;
}

[System.Serializable]
public class JsonPositionWrapper
{
    public string positionName;
    public JsonPosition position;
}

[System.Serializable]
public class JsonFormation
{
    public string formationName;
    public string images;
    public List<JsonPositionWrapper> positions;
}

[System.Serializable]
public class JsonFormations
{
    public List<JsonFormation> formations;
}