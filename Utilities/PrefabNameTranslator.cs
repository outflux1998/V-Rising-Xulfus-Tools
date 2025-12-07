using System.Collections.Generic;

public static class PrefabNameTranslator
{
    public static readonly Dictionary<int, string> GuidToName = new Dictionary<int, string>
    {
        //{ 38526109, "Player Inventory" },
        { 240964190, "Wooden Chest" },
        { -1657744516, "Golden Chest" },
        { 257686919, "Iron Chest" },
        { -1576310588, "Gloomrot Chest" },
        { -1203166929, "Sludge Chest" },

        // Adicione mais GUIDs conhecidos aqui
    };

    public static string GetName(int guid)
    {
        return GuidToName.TryGetValue(guid, out string name) ? name : $"GUID: {guid}";
    }
}
