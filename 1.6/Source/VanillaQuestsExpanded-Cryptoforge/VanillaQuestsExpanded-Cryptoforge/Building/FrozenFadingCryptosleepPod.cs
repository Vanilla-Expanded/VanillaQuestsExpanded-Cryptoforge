using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Grammar;
using static RimWorld.DefOf;

namespace VanillaQuestsExpandedCryptoforge
{
    public class FrozenFadingCryptosleepPod : Building_AncientCryptosleepCasket
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad)
            {
                innerContainer.ClearAndDestroyContents();
                Pawn hero = PawnGenerator.GeneratePawn(InternalDefOf.VQE_Hero);
                GrammarRequest firstNameReq = default;
                if (hero.gender == Gender.Male)
                {
                    firstNameReq.Includes.Add(InternalDefOf.VQE_HeroMaleNames);
                }
                else
                {
                    firstNameReq.Includes.Add(InternalDefOf.VQE_HeroFemaleNames);
                }
                var firstName = GrammarResolver.Resolve("r_first_name", firstNameReq);
                GrammarRequest lastNameReq = default;
                lastNameReq.Includes.Add(InternalDefOf.VQE_HeroLastNames);
                var lastName = GrammarResolver.Resolve("r_last_name", lastNameReq);
                var name = new NameTriple(firstName, "", lastName);
                hero.Name = name;


                hero.equipment.DestroyAllEquipment();
                hero.apparel.DestroyAll();
                hero.inventory.DestroyAll();
                
                var apparels = new List<Apparel>();
                apparels.Add(ThingMaker.MakeThing(InternalDefOf.VQE_Apparel_CryptoHeavyHelmet) as Apparel);
                apparels.Add(ThingMaker.MakeThing(InternalDefOf.VQE_CryptoHeavyArmor) as Apparel);
                foreach (var apparel in apparels)
                {
                    var comp = apparel.TryGetComp<CompQuality>();
                    if (comp != null)
                    {
                        comp.SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);
                    }
                    hero.apparel.Wear(apparel);
                }
                innerContainer.TryAdd(hero);
            }
        }

        public override void EjectContents()
        {
            var map = Map;
            var hero = innerContainer.FirstOrDefault() as Pawn;
            base.EjectContents();
            hero.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Sad, transitionSilently: true);
            hero.SetFaction(Faction.OfPlayer);
            hero.health.AddHediff(InternalDefOf.VQE_Guilt);
            Find.LetterStack.ReceiveLetter("VQE_HeroJoins".Translate(hero.Named("PAWN")), "VQE_HeroJoinsDesc".Translate(hero.Named("PAWN")), LetterDefOf.PositiveEvent, hero);
            Thing.allowDestroyNonDestroyable = true;
            this.Destroy();
            Thing.allowDestroyNonDestroyable = false;
            var pod = ThingMaker.MakeThing(InternalDefOf.VQE_FrozenEmptyCryptosleepPod);
            GenSpawn.Spawn(pod, Position, map, Rotation);
        }
    }
}