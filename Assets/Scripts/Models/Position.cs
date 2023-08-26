using UnityEngine;

[System.Serializable]
public class Position
{
    public string positionName;
    public Vector2 position;

    public void ConvertPercentageToPosition(string leftPercentage, string topPercentage, RectTransform footballField)
    {
        float width = footballField.rect.width;
        float height = footballField.rect.height;

        float left = (float.Parse(leftPercentage.TrimEnd('%')) / 100) * width;
        float top = (float.Parse(topPercentage.TrimEnd('%')) / 100) * height;

        position = new Vector2(left, top);
    }
}
