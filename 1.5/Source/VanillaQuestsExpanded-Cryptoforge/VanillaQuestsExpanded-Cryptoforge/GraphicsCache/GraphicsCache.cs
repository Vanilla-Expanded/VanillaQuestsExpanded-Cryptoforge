using System;
using UnityEngine;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    [StaticConstructorOnStartup]
    public static class GraphicsCache
    {
       
        public static readonly Material ScavengeOverlay = MaterialPool.MatFrom("UI/Scavenge_Overlay", ShaderDatabase.MetaOverlay);
        public static readonly Material StudyOverlay = MaterialPool.MatFrom("UI/Study_Overlay", ShaderDatabase.MetaOverlay);

    }
}
