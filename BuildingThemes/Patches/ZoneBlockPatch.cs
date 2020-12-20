using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Math;
using HarmonyLib;
using UnityEngine;

namespace BuildingThemes.Patches
{

    [HarmonyPatch(typeof(ZoneBlock))]
    static class ZoneBlockPatch
    {
        
        private static int _zoneGridResolution = ZoneManager.ZONEGRID_RESOLUTION;
        private static int _zoneGridHalfResolution = ZoneManager.ZONEGRID_RESOLUTION / 2;
        private static readonly int EIGHTY_ONE_ZONEGRID_RESOLUTION = 270;
        private static readonly int EIGHTY_ONE_HALF_ZONEGRID_RESOLUTION = EIGHTY_ONE_ZONEGRID_RESOLUTION / 2;
        
        private static int debugCount = 0;
        

        [HarmonyPatch("SimulationStep")]
        [HarmonyPatch(new [] {typeof(ushort)},
            new [] {ArgumentType.Normal})]
        static bool Prefix(ref ZoneBlock __instance, ushort blockID)
        {
            // check if 81 mod is active - and in that case, set up different grid
            if (Util.IsModActive(BuildingThemesMod.EIGHTY_ONE_MOD))
            {
                _zoneGridResolution = EIGHTY_ONE_ZONEGRID_RESOLUTION;
                _zoneGridHalfResolution = EIGHTY_ONE_HALF_ZONEGRID_RESOLUTION;
            }

            if (Debugger.Enabled && debugCount < 10)
            {
                debugCount++;
                Debugger.LogFormat("Building Themes: Detoured ZoneBlock.SimulationStep was called. blockID: {0}, position: {1}.", blockID, __instance.m_position);
            }

            ZoneManager zoneManager = Singleton<ZoneManager>.instance;
            int rowCount = __instance.RowCount;
            float m_angle = __instance.m_angle;

            Vector2 xDirection = new Vector2(Mathf.Cos(m_angle), Mathf.Sin(m_angle)) * 8f;
            Vector2 zDirection = new Vector2(xDirection.y, -xDirection.x);
            ulong num = __instance.m_valid & ~(__instance.m_occupied1 | __instance.m_occupied2);
            int spawnpointRow = 0;
            ItemClass.Zone zone = ItemClass.Zone.Unzoned;
            
            int num3 = 0;
            while (num3 < 4 && zone == ItemClass.Zone.Unzoned)
            {
                spawnpointRow = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)rowCount);
                if ((num & 1uL << (spawnpointRow << 3)) != 0uL)
                {
                    zone = __instance.GetZone(0, spawnpointRow);
                }
                num3++;
            }

            DistrictManager instance2 = Singleton<DistrictManager>.instance;
            Vector3 m_position = __instance.m_position;
            byte district = instance2.GetDistrict(m_position);

            int actualWorkplaceDemand;
            switch (zone)
            {
                case ItemClass.Zone.ResidentialLow:
                    actualWorkplaceDemand = zoneManager.m_actualResidentialDemand;
                    actualWorkplaceDemand += instance2.m_districts.m_buffer[(int)district].CalculateResidentialLowDemandOffset();
                    break;
                case ItemClass.Zone.ResidentialHigh:
                    actualWorkplaceDemand = zoneManager.m_actualResidentialDemand;
                    actualWorkplaceDemand += instance2.m_districts.m_buffer[(int)district].CalculateResidentialHighDemandOffset();
                    break;
                case ItemClass.Zone.CommercialLow:
                    actualWorkplaceDemand = zoneManager.m_actualCommercialDemand;
                    actualWorkplaceDemand += instance2.m_districts.m_buffer[(int)district].CalculateCommercialLowDemandOffset();
                    break;
                case ItemClass.Zone.CommercialHigh:
                    actualWorkplaceDemand = zoneManager.m_actualCommercialDemand;
                    actualWorkplaceDemand += instance2.m_districts.m_buffer[(int)district].CalculateCommercialHighDemandOffset();
                    break;
                case ItemClass.Zone.Industrial:
                    actualWorkplaceDemand = zoneManager.m_actualWorkplaceDemand;
                    actualWorkplaceDemand += instance2.m_districts.m_buffer[(int)district].CalculateIndustrialDemandOffset();
                    break;
                case ItemClass.Zone.Office:
                    actualWorkplaceDemand = zoneManager.m_actualWorkplaceDemand;
                    actualWorkplaceDemand += instance2.m_districts.m_buffer[(int)district].CalculateOfficeDemandOffset();
                    break;
                default:
                    return false; // TODO: check if it is right - skip vanilla method
            }

            Vector2 a = VectorUtils.XZ(m_position);
            Vector2 vector3 = a - 3.5f * xDirection + ((float)spawnpointRow - 3.5f) * zDirection;

            int[] tmpXBuffer = zoneManager.m_tmpXBuffer;
            for (int i = 0; i < 13; i++)
            {
                tmpXBuffer[i] = 0;
            }

            Quad2 quad = default(Quad2);
            quad.a = a - 4f * xDirection + ((float)spawnpointRow - 10f) * zDirection;
            quad.b = a + 3f * xDirection + ((float)spawnpointRow - 10f) * zDirection;
            quad.c = a + 3f * xDirection + ((float)spawnpointRow + 2f) * zDirection;
            quad.d = a - 4f * xDirection + ((float)spawnpointRow + 2f) * zDirection;
            Vector2 vector4 = quad.Min();
            Vector2 vector5 = quad.Max();

            //begin mod 81 tiles compatibility fix
            int Xnum3 = Mathf.Max((int)((vector4.x - 46f) / 64f + _zoneGridHalfResolution), 0);
            int Xnum4 = Mathf.Max((int)((vector4.y - 46f) / 64f + _zoneGridHalfResolution), 0);
            int Xnum5 = Mathf.Min((int)((vector5.x + 46f) / 64f + _zoneGridHalfResolution), _zoneGridResolution - 1);
            int Xnum6 = Mathf.Min((int)((vector5.y + 46f) / 64f + _zoneGridHalfResolution), _zoneGridResolution - 1);
            //end mod

            for (int j = Xnum4; j <= Xnum6; j++)
            {
                for (int k = Xnum3; k <= Xnum5; k++)
                {
                    //begin mod 81 tiles compatibity fix
                    ushort Xnum7 = zoneManager.m_zoneGrid[j * _zoneGridResolution + k];
                    //end mod
                    int Xnum8 = 0;
                    while (Xnum7 != 0)
                    {
                        Vector3 positionVar = zoneManager.m_blocks.m_buffer[(int)Xnum7].m_position;
                        float Xnum9 = Mathf.Max(Mathf.Max(vector4.x - 46f - positionVar.x, vector4.y - 46f - positionVar.z),
                            Mathf.Max(positionVar.x - vector5.x - 46f, positionVar.z - vector5.y - 46f));

                        if (Xnum9 < 0f)
                        {
                            CheckBlock(ref __instance, ref zoneManager.m_blocks.m_buffer[(int)Xnum7], tmpXBuffer, zone, vector3, xDirection, zDirection, quad);
                        }
                        Xnum7 = zoneManager.m_blocks.m_buffer[(int)Xnum7].m_nextGridBlock;
                        if (++Xnum8 >= 49152)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }

            for (int l = 0; l < 13; l++)
            {
                uint Xnum10 = (uint)tmpXBuffer[l];
                int Xnum11 = 0;
                bool flag = (Xnum10 & 196608u) == 196608u;
                bool flag2 = false;
                while ((Xnum10 & 1u) != 0u)
                {
                    Xnum11++;
                    flag2 = ((Xnum10 & 65536u) != 0u);
                    Xnum10 >>= 1;
                }
                if (Xnum11 == 5 || Xnum11 == 6)
                {
                    if (flag2)
                    {
                        Xnum11 -= Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) + 2;
                    }
                    else
                    {
                        Xnum11 = 4;
                    }
                    Xnum11 |= 131072;
                }
                else if (Xnum11 == 7)
                {
                    Xnum11 = 4;
                    Xnum11 |= 131072;
                }
                if (flag)
                {
                    Xnum11 |= 65536;
                }
                tmpXBuffer[l] = Xnum11;
            }
            int Xnum12 = tmpXBuffer[6] & 65535;
            if (Xnum12 == 0)
            {
                return false; // TODO: check if it is right - skip vanilla method
            }

            bool flag3 = IsGoodPlace(ref __instance, vector3);
            
            if (Singleton<SimulationManager>.instance.m_randomizer.Int32(100u) >= actualWorkplaceDemand)
            {
                if (flag3)
                {
                    zoneManager.m_goodAreaFound[(int)zone] = 1024;
                }
                return false; // TODO: check if it is right - skip vanilla method
            }
           
            if (!flag3 && zoneManager.m_goodAreaFound[(int)zone] > -1024)
            {
                if (zoneManager.m_goodAreaFound[(int)zone] == 0)
                {
                    zoneManager.m_goodAreaFound[(int)zone] = -1;
                }
                return false; // TODO: check if it is right - skip vanilla method
            }

            int Xnum13 = 6;
            int Xn = 6;
            bool flag4 = true;
            
            while (true)
            {
                if (flag4)
                {
                    while (Xnum13 != 0)
                    {
                        if ((tmpXBuffer[Xnum13 - 1] & 65535) != Xnum12)
                        {
                            break;
                        }
                        Xnum13--;
                    }
                    while (Xn != 12)
                    {
                        if ((tmpXBuffer[Xn + 1] & 65535) != Xnum12)
                        {
                            break;
                        }
                        Xn++;
                    }
                }
                else
                {
                    while (Xnum13 != 0)
                    {
                        if ((tmpXBuffer[Xnum13 - 1] & 65535) < Xnum12)
                        {
                            break;
                        }
                        Xnum13--;
                    }
                    while (Xn != 12)
                    {
                        if ((tmpXBuffer[Xn + 1] & 65535) < Xnum12)
                        {
                            break;
                        }
                        Xn++;
                    }
                }
                int Xnum14 = Xnum13;
                int Xnum15 = Xn;
                while (Xnum14 != 0)
                {
                    if ((tmpXBuffer[Xnum14 - 1] & 65535) < 2)
                    {
                        break;
                    }
                    Xnum14--;
                }
                while (Xnum15 != 12)
                {
                    if ((tmpXBuffer[Xnum15 + 1] & 65535) < 2)
                    {
                        break;
                    }
                    Xnum15++;
                }
                bool flag5 = Xnum14 != 0 && Xnum14 == Xnum13 - 1;
                bool flag6 = Xnum15 != 12 && Xnum15 == Xn + 1;
                if (flag5 && flag6)
                {
                    if (Xn - Xnum13 > 2)
                    {
                        break;
                    }
                    if (Xnum12 <= 2)
                    {
                        if (!flag4)
                        {
                            goto Block_34;
                        }
                    }
                    else
                    {
                        Xnum12--;
                    }
                }
                else if (flag5)
                {
                    if (Xn - Xnum13 > 1)
                    {
                        goto Block_36;
                    }
                    if (Xnum12 <= 2)
                    {
                        if (!flag4)
                        {
                            goto Block_38;
                        }
                    }
                    else
                    {
                        Xnum12--;
                    }
                }
                else if (flag6)
                {
                    if (Xn - Xnum13 > 1)
                    {
                        goto Block_40;
                    }
                    if (Xnum12 <= 2)
                    {
                        if (!flag4)
                        {
                            goto Block_42;
                        }
                    }
                    else
                    {
                        Xnum12--;
                    }
                }
                else
                {
                    if (Xnum13 != Xn)
                    {
                        goto IL_884;
                    }
                    if (Xnum12 <= 2)
                    {
                        if (!flag4)
                        {
                            goto Block_45;
                        }
                    }
                    else
                    {
                        Xnum12--;
                    }
                }
                flag4 = false;
            }
            Xnum13++;
            Xn--;
            Block_34:
            goto IL_891;
            Block_36:
            Xnum13++;
            Block_38:
            goto IL_891;
            Block_40:
            Xn--;
            Block_42:
            Block_45:
            IL_884:
            IL_891:
            int Xnum16;
            int Xnum17;
            if (Xnum12 == 1 && Xn - Xnum13 >= 1)
            {
                Xnum13 += Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)(Xn - Xnum13));
                Xn = Xnum13 + 1;
                Xnum16 = Xnum13 + Singleton<SimulationManager>.instance.m_randomizer.Int32(2u);
                Xnum17 = Xnum16;
            }
            else
            {
                do
                {
                    Xnum16 = Xnum13;
                    Xnum17 = Xn;
                    if (Xn - Xnum13 == 2)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            Xnum17--;
                        }
                        else
                        {
                            Xnum16++;
                        }
                    }
                    else if (Xn - Xnum13 == 3)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            Xnum17 -= 2;
                        }
                        else
                        {
                            Xnum16 += 2;
                        }
                    }
                    else if (Xn - Xnum13 == 4)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            Xn -= 2;
                            Xnum17 -= 3;
                        }
                        else
                        {
                            Xnum13 += 2;
                            Xnum16 += 3;
                        }
                    }
                    else if (Xn - Xnum13 == 5)
                    {
                        if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            Xn -= 3;
                            Xnum17 -= 2;
                        }
                        else
                        {
                            Xnum13 += 3;
                            Xnum16 += 2;
                        }
                    }
                    else if (Xn - Xnum13 >= 6)
                    {
                        if (Xnum13 == 0 || Xn == 12)
                        {
                            if (Xnum13 == 0)
                            {
                                Xnum13 = 3;
                                Xnum16 = 2;
                            }
                            if (Xn == 12)
                            {
                                Xn = 9;
                                Xnum17 = 10;
                            }
                        }
                        else if (Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0)
                        {
                            Xn = Xnum13 + 3;
                            Xnum17 = Xnum16 + 2;
                        }
                        else
                        {
                            Xnum13 = Xn - 3;
                            Xnum16 = Xnum17 - 2;
                        }
                    }
                }
                while (Xn - Xnum13 > 3 || Xnum17 - Xnum16 > 3);
            }

            int depth_A = 4;
            int width_A = Xn - Xnum13 + 1;
            BuildingInfo.ZoningMode zoningMode = BuildingInfo.ZoningMode.Straight;
            bool flag7 = true;

            for (int Xnum20 = Xnum13; Xnum20 <= Xn; Xnum20++)
            {
                depth_A = Mathf.Min(depth_A, tmpXBuffer[Xnum20] & 65535);
                if ((tmpXBuffer[Xnum20] & 131072) == 0)
                {
                    flag7 = false;
                }
            }
            if (Xn > Xnum13)
            {
                if ((tmpXBuffer[Xnum13] & 65536) != 0)
                {
                    zoningMode = BuildingInfo.ZoningMode.CornerLeft;
                    Xnum17 = Xnum13 + Xnum17 - Xnum16;
                    Xnum16 = Xnum13;
                }
                if ((tmpXBuffer[Xn] & 65536) != 0 && (zoningMode != BuildingInfo.ZoningMode.CornerLeft || Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0))
                {
                    zoningMode = BuildingInfo.ZoningMode.CornerRight;
                    Xnum16 = Xn + Xnum16 - Xnum17;
                    Xnum17 = Xn;
                }
            }

            int depth_B = 4;
            int width_B = Xnum17 - Xnum16 + 1;
            BuildingInfo.ZoningMode zoningMode2 = BuildingInfo.ZoningMode.Straight;
            bool flag8 = true;

            for (int Xnum23 = Xnum16; Xnum23 <= Xnum17; Xnum23++)
            {
                depth_B = Mathf.Min(depth_B, tmpXBuffer[Xnum23] & 65535);
                if ((tmpXBuffer[Xnum23] & 131072) == 0)
                {
                    flag8 = false;
                }
            }
            if (Xnum17 > Xnum16)
            {
                if ((tmpXBuffer[Xnum16] & 65536) != 0)
                {
                    zoningMode2 = BuildingInfo.ZoningMode.CornerLeft;
                }
                if ((tmpXBuffer[Xnum17] & 65536) != 0 && (zoningMode2 != BuildingInfo.ZoningMode.CornerLeft || Singleton<SimulationManager>.instance.m_randomizer.Int32(2u) == 0))
                {
                    zoningMode2 = BuildingInfo.ZoningMode.CornerRight;
                }
            }

            ItemClass.SubService subService = ItemClass.SubService.None;
            ItemClass.Level level = ItemClass.Level.Level1;
            ItemClass.Service service;
            switch (zone)
            {
                case ItemClass.Zone.ResidentialLow:
                    service = ItemClass.Service.Residential;
                    subService = ItemClass.SubService.ResidentialLow;
                    break;
                case ItemClass.Zone.ResidentialHigh:
                    service = ItemClass.Service.Residential;
                    subService = ItemClass.SubService.ResidentialHigh;
                    break;
                case ItemClass.Zone.CommercialLow:
                    service = ItemClass.Service.Commercial;
                    subService = ItemClass.SubService.CommercialLow;
                    break;
                case ItemClass.Zone.CommercialHigh:
                    service = ItemClass.Service.Commercial;
                    subService = ItemClass.SubService.CommercialHigh;
                    break;
                case ItemClass.Zone.Industrial:
                    service = ItemClass.Service.Industrial;
                    break;
                case ItemClass.Zone.Office:
                    service = ItemClass.Service.Office;
                    subService = ItemClass.SubService.None;
                    break;
                default:
                    return false; // TODO: check if it is right - skip vanilla method
            }

            BuildingInfo buildingInfo = null;
            Vector3 vector6 = Vector3.zero;
            int row = 0;
            int length = 0;
            int width = 0;
            BuildingInfo.ZoningMode zoningMode3 = BuildingInfo.ZoningMode.Straight;

            // return to vanilla

            for (int Xnum27 = 0; Xnum27 < 6; Xnum27++)
            { 
                switch (Xnum27)
                {
                    case 0:
                        if (zoningMode == BuildingInfo.ZoningMode.Straight)
                        {
                            continue;
                        }
                        row = Xnum13 + Xn + 1;
                        length = depth_A;
                        width = width_A;
                        zoningMode3 = zoningMode;
                        goto default;

                    case 1:
                        if (zoningMode2 == BuildingInfo.ZoningMode.Straight)
                        {
                            continue;
                        }
                        row = Xnum16 + Xnum17 + 1;
                        length = depth_B;
                        width = width_B;
                        zoningMode3 = zoningMode2;
                        goto default;

                    case 2:
                        if (zoningMode == BuildingInfo.ZoningMode.Straight || depth_A < 4)
                        {
                            continue;
                        }
                        row = Xnum13 + Xn + 1;
                        length = ((!flag7) ? 2 : 3);
                        width = width_A;
                        zoningMode3 = zoningMode;
                        goto default;

                    case 3:
                        if (zoningMode2 == BuildingInfo.ZoningMode.Straight || depth_B < 4)
                        {
                            continue;
                        }
                        row = Xnum16 + Xnum17 + 1;
                        length = ((!flag8) ? 2 : 3);
                        width = width_B;
                        zoningMode3 = zoningMode2;
                        goto default;

                    case 4:
                        row = Xnum13 + Xn + 1;
                        length = depth_A;
                        width = width_A;
                        zoningMode3 = BuildingInfo.ZoningMode.Straight;
                        goto default;

                    case 5:
                        row = Xnum16 + Xnum17 + 1;
                        length = depth_B;
                        width = width_B;
                        zoningMode3 = BuildingInfo.ZoningMode.Straight;
                        goto default;

                    default:
                        {
                            vector6 = m_position + VectorUtils.X_Y(((float)length * 0.5f - 4f) * xDirection + ((float)row * 0.5f + (float)spawnpointRow - 10f) * zDirection);
                            switch (zone)
                            {
                                case ItemClass.Zone.Industrial:
                                    ZoneBlock.GetIndustryType(vector6, out subService, out level);
                                    break;
                                case ItemClass.Zone.CommercialLow:
                                case ItemClass.Zone.CommercialHigh:
                                    ZoneBlock.GetCommercialType(vector6, zone, width, length, out subService, out level);
                                    break;
                                case ItemClass.Zone.ResidentialLow:
                                case ItemClass.Zone.ResidentialHigh:
                                    ZoneBlock.GetResidentialType(vector6, zone, width, length, out subService, out level);
                                    break;
                                case ItemClass.Zone.Office:
                                    ZoneBlock.GetOfficeType(vector6, zone, width, length, out subService, out level);
                                    break;
                            }
                            byte district2 = instance2.GetDistrict(vector6);
                            ushort style = instance2.m_districts.m_buffer[district2].m_Style;
                            if (Singleton<BuildingManager>.instance.m_BuildingWrapper != null)
                            {
                                Singleton<BuildingManager>.instance.m_BuildingWrapper.OnCalculateSpawn(vector6, ref service, ref subService, ref level, ref style);
                            }
                            // Call modded building manager
                            buildingInfo = RandomBuildings.GetRandomBuildingInfo_Spawn(vector6, ref Singleton<SimulationManager>.instance.m_randomizer, service, subService, level, width, length, zoningMode3, style);


                            if ((object)buildingInfo == null)
                            {
                                continue;
                            }
                            if (Debugger.Enabled)
                            {
                                Debugger.LogFormat("Prefab Found: {5} - {0}, {1}, {2}, {3} x {4}", service, subService, level, width, length, buildingInfo.name);
                            }
                            break;
                        }
                }
                break;
            }

            if (buildingInfo == null)
            {
                // No prefab found
                if (Debugger.Enabled)
                {
                    Debugger.LogFormat("No Suitable Prefab Found: {0}, {1}, {2}, {3} x {4}", service, subService, level, width, length);
                }
                return false; // TODO: check if it is right - skip vanilla method
            }

            // end of return to the vanilla

            float Xnum28 = Singleton<TerrainManager>.instance.WaterLevel(VectorUtils.XZ(vector6));
            if (Xnum28 > vector6.y || Singleton<DisasterManager>.instance.IsEvacuating(vector6))
            {
                return false; // TODO: check if it is right - skip vanilla method
            }
            float Xnum29 = m_angle + (float)Math.PI / 2f;
            if (zoningMode3 == BuildingInfo.ZoningMode.CornerLeft && buildingInfo.m_zoningMode == BuildingInfo.ZoningMode.CornerRight)
            {
                Xnum29 -= (float)Math.PI / 2f;
                length = width;
            }
            else if (zoningMode3 == BuildingInfo.ZoningMode.CornerRight && buildingInfo.m_zoningMode == BuildingInfo.ZoningMode.CornerLeft)
            {
                Xnum29 += (float)Math.PI / 2f;
                length = width;
            }

            if (Singleton<BuildingManager>.instance.CreateBuilding(out var building, ref Singleton<SimulationManager>.instance.m_randomizer, buildingInfo, vector6, Xnum29, length, Singleton<SimulationManager>.instance.m_currentBuildIndex))
            {
                Singleton<SimulationManager>.instance.m_currentBuildIndex++;
                switch (service)
                {
                    case ItemClass.Service.Residential:
                        zoneManager.m_actualResidentialDemand = Mathf.Max(0, zoneManager.m_actualResidentialDemand - 5);
                        break;
                    case ItemClass.Service.Commercial:
                        zoneManager.m_actualCommercialDemand = Mathf.Max(0, zoneManager.m_actualCommercialDemand - 5);
                        break;
                    case ItemClass.Service.Industrial:
                        zoneManager.m_actualWorkplaceDemand = Mathf.Max(0, zoneManager.m_actualWorkplaceDemand - 5);
                        break;
                    case ItemClass.Service.Office:
                        zoneManager.m_actualWorkplaceDemand = Mathf.Max(0, zoneManager.m_actualWorkplaceDemand - 5);
                        break;
                }
                if (zone == ItemClass.Zone.ResidentialHigh || zone == ItemClass.Zone.CommercialHigh)
                {
                    Singleton<BuildingManager>.instance.m_buildings.m_buffer[building].m_flags |= Building.Flags.HighDensity;
                }
            }
            zoneManager.m_goodAreaFound[(int)zone] = 1024;
            
            // skip vanilla method
            return false;
        }

        [HarmonyReversePatch]
        [HarmonyPatch("IsGoodPlace")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static bool IsGoodPlace(ref ZoneBlock zoneBlock, Vector2 vector3)
        {
            Debugger.LogError("ZoneBlock.IsGoodPlace reverse Harmony patch was not applied");
            throw new NotImplementedException("ZoneBlock.IsGoodPlace reverse Harmony patch was not applied");
        }

        [HarmonyReversePatch]
        [HarmonyPatch("CheckBlock")]
        [MethodImpl(MethodImplOptions.NoInlining)]
        static void CheckBlock(ref ZoneBlock zoneBlock, ref ZoneBlock zoneBlock1, int[] tmpXBuffer, ItemClass.Zone zone, Vector2 vector3, Vector2 xDirection, Vector2 zDirection, Quad2 quad)
        {
            Debugger.LogError("ZoneBlock.CheckBlock reverse Harmony patch was not applied");
            throw new NotImplementedException("ZoneBlock.CheckBlock reverse Harmony patch was not applied");
        }
    }
    
}
