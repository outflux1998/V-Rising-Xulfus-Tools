using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetroCamera.Utilities
{
    internal class BloodType
    {
        public static string GetBloodTypeName(int bloodTypeHash)
        {
            return bloodTypeHash switch
            {
                -1382693416 => "Corrupted",
                524822543 => "Creature",
                804798592 => "Brute",
                1328126535 => "Draculin",
                1821108694 => "Mutant",
                -1620185637 => "Rogue",
                1476452791 => "Scholar",
                -516976528 => "Warrior",
                -1776904174 => "Worker",
                _ => "Unknown"
            };
        }

    }
}
