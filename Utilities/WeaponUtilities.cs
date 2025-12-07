using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using ProjectM;
using RetroCamera.Configuration;
using RetroCamera.Utilities;

namespace RetroCamera.Utilities
{
    public enum WeaponCategory
    {
        Sword, Axes, Mace, Spear, Reaper, Greatsword, Whip,
        Slashers, Claws, Twinblade, Crossbow, Longbow, Daggers, Pistols
    }

    public static class WeaponHelper
    {
        private static readonly Dictionary<int, WeaponCategory> prefabToCategory = new()
        {
            // SWORD
            { -2085919458, WeaponCategory.Sword },
            { -796306296, WeaponCategory.Sword },
            { -2037272000, WeaponCategory.Sword },
            { -1219959051, WeaponCategory.Sword },
            { -903587404, WeaponCategory.Sword },
            { -435501075, WeaponCategory.Sword },
            { -1455388114, WeaponCategory.Sword },
            { -774462329, WeaponCategory.Sword },
            { -1215982687, WeaponCategory.Sword },
            { 1637216050, WeaponCategory.Sword },
            { 2106567892, WeaponCategory.Sword },
            { 195858450, WeaponCategory.Sword },

            // AXE
            { -1958888844, WeaponCategory.Axes },
            { -1391446205, WeaponCategory.Axes },
            { 1541522788, WeaponCategory.Axes },
            { 518802008, WeaponCategory.Axes },
            { -491969324, WeaponCategory.Axes },
            { -1579575933, WeaponCategory.Axes },
            { 198951695, WeaponCategory.Axes },
            { -1130238142, WeaponCategory.Axes },
            { -2044057823, WeaponCategory.Axes },
            { 2100090213, WeaponCategory.Axes },
            { 1259464735, WeaponCategory.Axes },
            { 1239564213, WeaponCategory.Axes },
            { -102830349, WeaponCategory.Axes },

            // MACE
            { 1588258447, WeaponCategory.Mace },
            { -1998017941, WeaponCategory.Mace },
            { -687294429, WeaponCategory.Mace },
            { -331345186, WeaponCategory.Mace },
            { 343324920, WeaponCategory.Mace },
            { -1714012261, WeaponCategory.Mace },
            { -276593802, WeaponCategory.Mace },
            { -184713893, WeaponCategory.Mace },
            { -126076280, WeaponCategory.Mace },
            { 160471982, WeaponCategory.Mace },
            { 675187526, WeaponCategory.Mace },
            { 1994084762, WeaponCategory.Mace },
            { 1177597629, WeaponCategory.Mace },

            // SPEAR
            { 2038011836, WeaponCategory.Spear },
            { 1244180446, WeaponCategory.Spear },
            { 1370755976, WeaponCategory.Spear },
            { 790210443, WeaponCategory.Spear },
            { 1853029976, WeaponCategory.Spear },
            { 1065194820, WeaponCategory.Spear },
            { -352704566, WeaponCategory.Spear },
            { -850142339, WeaponCategory.Spear },
            { 1307774440, WeaponCategory.Spear },
            { 2001389164, WeaponCategory.Spear },
            { -1674680373, WeaponCategory.Spear },
            { -1931117134, WeaponCategory.Spear },

            // REAPER
            { -152327780, WeaponCategory.Reaper },
            { 1402953369, WeaponCategory.Reaper },
            { 1522792650, WeaponCategory.Reaper },
            { 1048518929, WeaponCategory.Reaper },
            { -2081286944, WeaponCategory.Reaper },
            { 1778128946, WeaponCategory.Reaper },
            { 1887724512, WeaponCategory.Reaper },
            { 6711686, WeaponCategory.Reaper },
            { -859437190, WeaponCategory.Reaper },
            { -2053917766, WeaponCategory.Reaper },
            { -465491217, WeaponCategory.Reaper },
            { -922125625, WeaponCategory.Reaper },
            { -105026635, WeaponCategory.Reaper },

            // GREAT SWORD
            { -768054337, WeaponCategory.Greatsword },
            { 82781195, WeaponCategory.Greatsword },
            { 674704033, WeaponCategory.Greatsword },
            { 147836723, WeaponCategory.Greatsword },
            { 1322254792, WeaponCategory.Greatsword },
            { 869276797, WeaponCategory.Greatsword },
            { 820408138, WeaponCategory.Greatsword },
            { -1173681254, WeaponCategory.Greatsword },

            // WHIP
            { -847062445, WeaponCategory.Whip },
            { 1393113320, WeaponCategory.Whip },
            { -960205578, WeaponCategory.Whip },
            { -655095317, WeaponCategory.Whip },
            { 567413754, WeaponCategory.Whip },
            { 1705984031, WeaponCategory.Whip },
            { -671246832, WeaponCategory.Whip },
            { 429323760, WeaponCategory.Whip },

            // SLASHERS
            { -588909332, WeaponCategory.Slashers },
            { 926722036, WeaponCategory.Slashers },
            { 1499160417, WeaponCategory.Slashers },
            { -1042299347, WeaponCategory.Slashers },
            { -314614708, WeaponCategory.Slashers },
            { 866934844, WeaponCategory.Slashers },
            { 633666898, WeaponCategory.Slashers },
            { 1322545846, WeaponCategory.Slashers },
            { 506082542, WeaponCategory.Slashers },
            { 1930526079, WeaponCategory.Slashers },
            { 1570363331, WeaponCategory.Slashers },
            { -2068145306, WeaponCategory.Slashers },
            { 821410795, WeaponCategory.Slashers },

            // CLAWS
            { -1333849822, WeaponCategory.Claws },
            { 1748886117, WeaponCategory.Claws },
            { -996999913, WeaponCategory.Claws },
            { -1470260175, WeaponCategory.Claws },
            { -1777908217, WeaponCategory.Claws },
            { -2060572315, WeaponCategory.Claws },
            { -27238530, WeaponCategory.Claws },

            // TWINBLADES
            { -1122389049, WeaponCategory.Twinblade },
            { -1651990235, WeaponCategory.Twinblade },
            { -1595292245, WeaponCategory.Twinblade },
            { -297349982, WeaponCategory.Twinblade },
            { -699863795, WeaponCategory.Twinblade },
            { -1634108038, WeaponCategory.Twinblade },
            { 601169005, WeaponCategory.Twinblade },
            { 152014105, WeaponCategory.Twinblade },

            // CROSSBOW
            { -20041991, WeaponCategory.Crossbow },
            { 898159697, WeaponCategory.Crossbow },
            { -1277074895, WeaponCategory.Crossbow },
            { -1636801169, WeaponCategory.Crossbow },
            { 836066667, WeaponCategory.Crossbow },
            { 1221976097, WeaponCategory.Crossbow },
            { -814739263, WeaponCategory.Crossbow },
            { 1389040540, WeaponCategory.Crossbow },
            { 1957540013, WeaponCategory.Crossbow },
            { -1401104184, WeaponCategory.Crossbow },
            { -517906196, WeaponCategory.Crossbow },
            { 935392085, WeaponCategory.Crossbow },

            // LONGBOW
            { 532033005, WeaponCategory.Longbow },
            { 352247730, WeaponCategory.Longbow },
            { -1993708658, WeaponCategory.Longbow },
            { 1951565953, WeaponCategory.Longbow },
            { -1830162796, WeaponCategory.Longbow },
            { 1860352606, WeaponCategory.Longbow },
            { 1283345494, WeaponCategory.Longbow },
            { -557203874, WeaponCategory.Longbow },
            { -1003309553, WeaponCategory.Longbow },
            { 1177453385, WeaponCategory.Longbow },

            // DAGGERS
            { 1296724931, WeaponCategory.Daggers },
            { 703783407, WeaponCategory.Daggers },
            { -211034148, WeaponCategory.Daggers },
            { 1031107636, WeaponCategory.Daggers },
            { -1873605364, WeaponCategory.Daggers },
            { -1961050884, WeaponCategory.Daggers },
            { -1276458869, WeaponCategory.Daggers },
            { 140761255, WeaponCategory.Daggers },

            // PISTOLS
            { 769603740, WeaponCategory.Pistols },
            { 1850870666, WeaponCategory.Pistols },
            { 674407758, WeaponCategory.Pistols },
            { 1071656850, WeaponCategory.Pistols },
            { 1759077469, WeaponCategory.Pistols },
            { -1265586439, WeaponCategory.Pistols },
            { 14297698, WeaponCategory.Pistols },
            { -944318126, WeaponCategory.Pistols }
        };


        public static WeaponCategory? GetEquippedWeaponCategory(Entity localPlayer, EntityManager entityManager)
        {
            if (!entityManager.HasComponent<ProjectM.Equipment>(localPlayer)) return null;

            var equipment = entityManager.GetComponentData<ProjectM.Equipment>(localPlayer);
            int prefabGuid = equipment.WeaponSlot.SlotId._Value;

            if (prefabToCategory.TryGetValue(prefabGuid, out var category))
            {
                //Core.Log.LogInfo($"[WeaponCategory] Equipped weapon category: {category}");
                return category;
            }

            //Core.Log.LogInfo($"[WeaponCategory] Unknown weapon prefab GUID: {prefabGuid}");
            return null;
        }
    }
}
