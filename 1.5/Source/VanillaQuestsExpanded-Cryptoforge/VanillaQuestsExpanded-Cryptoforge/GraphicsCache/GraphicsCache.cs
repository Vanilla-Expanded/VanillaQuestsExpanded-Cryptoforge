using System;
using System.Diagnostics;
using UnityEngine;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    [StaticConstructorOnStartup]
    public static class GraphicsCache
    {
       
        public static readonly Material ScavengeOverlay = MaterialPool.MatFrom("UI/Scavenge_Overlay", ShaderDatabase.MetaOverlay);
        public static readonly Material StudyOverlay = MaterialPool.MatFrom("UI/Study_Overlay", ShaderDatabase.MetaOverlay);
        public static readonly Material EnableOverlay = MaterialPool.MatFrom("UI/Enable_Overlay", ShaderDatabase.MetaOverlay);
        public static readonly Graphic_Single VisibleMine = (Graphic_Single)GraphicDatabase.Get<Graphic_Single>("Things/Security/AncientLandmine_Visible", ShaderDatabase.Cutout,
                     new Vector2(1,1), Color.white); 


        public static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);
        public static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.02f, 0.46f, 0f), false);



    }
}
