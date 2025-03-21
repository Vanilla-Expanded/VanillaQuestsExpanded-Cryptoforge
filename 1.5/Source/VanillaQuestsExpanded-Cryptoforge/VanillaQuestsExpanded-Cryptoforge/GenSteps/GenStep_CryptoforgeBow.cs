using RimWorld;
using Verse;
using KCSG;
using VFECore;
using System.Collections.Generic;
using System.Linq;
using Verse.AI.Group;

namespace VanillaQuestsExpandedCryptoforge
{
    [HotSwappable]
    public class GenStep_CryptoforgeBow : GenStep_CryptoforgeStructureBase
    {
        protected override string LeftDefNamePrefix => "VQE_Cryptoforge_Bow_LeftSide";
        protected override string CenterDefNamePrefix => "VQE_Cryptoforge_Bow_Center";
        protected override string RightDefNamePrefix => "VQE_Cryptoforge_Bow_RightSide";
    }
}