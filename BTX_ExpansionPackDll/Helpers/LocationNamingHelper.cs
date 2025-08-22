using BattleTech;
using System.Collections.Generic;

namespace BTX_ExpansionPack
{
    public static class LocationNamingHelper
    {
        public static readonly List<LocationNamingTemplateByTags> Templates =
        [
            new LocationNamingTemplateByTags
            {
                Tags = ["unit_quad"],
                Names =
                [
                    new LocationName { Location = ChassisLocations.Head, Name = "HEAD", ShortName = "H" },
                    new LocationName { Location = ChassisLocations.LeftTorso, Name = "LEFT TORSO", ShortName = "LT" },
                    new LocationName { Location = ChassisLocations.CenterTorso, Name = "CENTER TORSO", ShortName = "CT" },
                    new LocationName { Location = ChassisLocations.RightTorso, Name = "RIGHT TORSO", ShortName = "RT" },
                    new LocationName { Location = ChassisLocations.LeftArm, Name = "FRONT LEFT LEG", ShortName = "FLL" },
                    new LocationName { Location = ChassisLocations.RightArm, Name = "FRONT RIGHT LEG", ShortName = "FRL" },
                    new LocationName { Location = ChassisLocations.LeftLeg, Name = "REAR LEFT LEG", ShortName = "RLL" },
                    new LocationName { Location = ChassisLocations.RightLeg, Name = "REAR RIGHT LEG", ShortName = "RRL" },
                ]
            },
            new LocationNamingTemplateByTags
            {
                Tags = ["unit_mech"],
                Names =
                [
                    new LocationName { Location = ChassisLocations.Head, Name = "HEAD", ShortName = "H" },
                    new LocationName { Location = ChassisLocations.LeftArm, Name = "LEFT ARM", ShortName = "LA" },
                    new LocationName { Location = ChassisLocations.LeftTorso, Name = "LEFT TORSO", ShortName = "LT" },
                    new LocationName { Location = ChassisLocations.CenterTorso, Name = "CENTER TORSO", ShortName = "CT" },
                    new LocationName { Location = ChassisLocations.RightTorso, Name = "RIGHT TORSO", ShortName = "RT" },
                    new LocationName { Location = ChassisLocations.RightArm, Name = "RIGHT ARM", ShortName = "RA" },
                    new LocationName { Location = ChassisLocations.LeftLeg, Name = "LEFT LEG", ShortName = "LL" },
                    new LocationName { Location = ChassisLocations.RightLeg, Name = "RIGHT LEG", ShortName = "RL" },
                ]
            },
            new LocationNamingTemplateByTags
            {
                Tags = ["unit_vtol"],
                Names =
                [
                    new LocationName { Location = ChassisLocations.LeftArm, Name = "FRONT", ShortName = "FR" },
                    new LocationName { Location = ChassisLocations.RightArm, Name = "REAR", ShortName = "RR" },
                    new LocationName { Location = ChassisLocations.LeftLeg, Name = "LEFT SIDE", ShortName = "LS" },
                    new LocationName { Location = ChassisLocations.RightLeg, Name = "RIGHT SIDE", ShortName = "RS" },
                    new LocationName { Location = ChassisLocations.Head, Name = "ROTOR", ShortName = "RO" }
                ]
            },
            new LocationNamingTemplateByTags
            {
                Tags = ["unit_vehicle", "fake_vehicle_chassis"],
                Names =
                [
                    new LocationName { Location = ChassisLocations.LeftArm, Name = "FRONT", ShortName = "FR" },
                    new LocationName { Location = ChassisLocations.RightArm, Name = "REAR", ShortName = "RR" },
                    new LocationName { Location = ChassisLocations.LeftLeg, Name = "LEFT SIDE", ShortName = "LS" },
                    new LocationName { Location = ChassisLocations.RightLeg, Name = "RIGHT SIDE", ShortName = "RS" },
                    new LocationName { Location = ChassisLocations.Head, Name = "TURRET", ShortName = "TU" }
                ]
            },
            new LocationNamingTemplateByTags
            {
                Tags = ["unit_squad"],
                Names =
                [
                    new LocationName { Location = ChassisLocations.Head, Name = "SQUAD LEADER", ShortName = "U0" },
                    new LocationName { Location = ChassisLocations.CenterTorso, Name = "SQUAD MEMBER 1", ShortName = "U1" },
                    new LocationName { Location = ChassisLocations.LeftTorso, Name = "SQUAD MEMBER 2", ShortName = "U2" },
                    new LocationName { Location = ChassisLocations.RightTorso, Name = "SQUAD MEMBER 3", ShortName = "U3" },
                    new LocationName { Location = ChassisLocations.LeftArm, Name = "SQUAD MEMBER 4", ShortName = "U4" },
                    new LocationName { Location = ChassisLocations.RightArm, Name = "SQUAD MEMBER 5", ShortName = "U5" },
                    new LocationName { Location = ChassisLocations.LeftLeg, Name = "SQUAD MEMBER 6", ShortName = "U6" },
                    new LocationName { Location = ChassisLocations.RightLeg, Name = "SQUAD MEMBER 7", ShortName = "U7" },
                ]
            }
        ];
        public static string GetLocationName(IEnumerable<string> tags, ChassisLocations location, bool showFullName)
        {
            var template = GetTemplate(tags);
            if (template != null)
            {
                foreach (var locName in template.Names)
                {
                    if (locName.Location == location)
                        return showFullName ? locName.Name : locName.ShortName;
                }
            }

            return string.Empty;
        }

        public static LocationNamingTemplateByTags GetTemplate(IEnumerable<string> tags)
        {
            foreach (var template in Templates)
            {
                foreach (var tag in template.Tags)
                {
                    if (tags != null && System.Linq.Enumerable.Contains(tags, tag))
                        return template;
                }
            }

            return null;
        }

        public class LocationNamingTemplateByTags
        {
            public string[] Tags;
            public LocationName[] Names;
        }

        public class LocationName
        {
            public ChassisLocations Location;
            public string Name;
            public string ShortName;
        }
    }
}
