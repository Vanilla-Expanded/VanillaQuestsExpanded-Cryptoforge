﻿
using RimWorld;
using System;
using Verse;
using UnityEngine;

namespace VanillaQuestsExpandedCryptoforge
{
    public class ConstantSprayer
    {
        private Thing parent;

        private int ticksUntilSpray = 250;

        private int sprayTicksLeft;

        public Action startSprayCallback;

        public Action endSprayCallback;

        private const int MinTicksBetweenSprays = 50;

        private const int MaxTicksBetweenSprays = 50;

        private const int MinSprayDuration = 500;

        private const int MaxSprayDuration = 500;

        private const float SprayThickness = 0.8f;

        public ConstantSprayer(Thing parent)
        {
            this.parent = parent;
        }

        public void SteamSprayerTick()
        {
            if (sprayTicksLeft > 0)
            {
                sprayTicksLeft--;
                if (Rand.Value < SprayThickness)
                {
                    Utils.ThrowExtendedAirPuffUp(parent.TrueCenter() + new Vector3(0, 0, 1.5f), parent.Map, 2, 2);
                }

                if (sprayTicksLeft <= 0)
                {
                    if (endSprayCallback != null)
                    {
                        endSprayCallback();
                    }
                    ticksUntilSpray = Rand.RangeInclusive(MinTicksBetweenSprays, MaxTicksBetweenSprays);
                }
                return;
            }
            ticksUntilSpray--;
            if (ticksUntilSpray <= 0)
            {
                if (startSprayCallback != null)
                {
                    startSprayCallback();
                }
                sprayTicksLeft = Rand.RangeInclusive(MinSprayDuration, MaxSprayDuration);
            }
        }
    }
}