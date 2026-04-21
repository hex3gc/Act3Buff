using BaseLib.Config;

namespace Act3Buff.Config
{
    [HoverTipsByDefault]
    internal class Act3BuffConfig : SimpleModConfig
    {
        // BOSSES
        [ConfigSection("DoormakerSection")]
            public static bool DoormakerEnabled { get; set; } = true;
            [SliderRange(10, 1000, 10)]
            [SliderLabelFormat("{0:0} hp")]
            public static double DoormakerMaxHPEasy { get; set; } = 600;
            [SliderRange(10, 1000, 10)]
            [SliderLabelFormat("{0:0} hp")]
            public static double DoormakerMaxHPHard { get; set; } = 650;
            [SliderRange(5, 60, 1)]
            /*
            public static double DoormakerHungerDamageEasy { get; set; } = 12;
            [SliderRange(5, 60, 1)]
            public static double DoormakerHungerDamageHard { get; set; } = 13;
            public static bool DoormakerScrutinyIsMulti { get; set; } = true;
            [SliderRange(5, 60, 1)]
            public static double DoormakerScrutinyDamageEasy { get; set; } = 12;
            [SliderRange(5, 60, 1)]
            public static double DoormakerScrutinyDamageHard { get; set; } = 13;
            [SliderRange(5, 60, 1)]
            public static double DoormakerGraspDamageEasy { get; set; } = 22;
            [SliderRange(5, 60, 1)]
            public static double DoormakerGraspDamageHard { get; set; } = 25;
            */

        [ConfigSection("QueenSection")]
            public static bool QueenEnabled { get; set; } = true;
            [SliderRange(1, 10, 1)]
            public static double QueenCardsAfflicted { get; set; } = 3;
            public static bool QueenBurnsAdd { get; set; } = true;
            [SliderRange(1, 3, 1)]
            public static double QueenBurnsAddCount { get; set; } = 1;
            public static bool QueenMoreBeams { get; set; } = true;

        [ConfigSection("TestSubjectSection")]
            public static bool TestSubjectEnabled { get; set; } = true;
            [SliderRange(10, 1000, 10)]
            [SliderLabelFormat("{0:0} hp")]
            public static double TestSubject1MaxHPEasy { get; set; } = 90;
            [SliderRange(10, 1000, 10)]
            [SliderLabelFormat("{0:0} hp")]
            public static double TestSubject1MaxHPHard { get; set; } = 100;
            [SliderRange(10, 1000, 10)]
            [SliderLabelFormat("{0:0} hp")]
            public static double TestSubject2MaxHPEasy { get; set; } = 180;
            [SliderRange(10, 1000, 10)]
            [SliderLabelFormat("{0:0} hp")]
            public static double TestSubject2MaxHPHard { get; set; } = 200;
            [SliderRange(10, 1000, 10)]
            [SliderLabelFormat("{0:0} hp")]
            public static double TestSubject3MaxHPEasy { get; set; } = 270;
            [SliderRange(10, 1000, 10)]
            [SliderLabelFormat("{0:0} hp")]
            public static double TestSubject3MaxHPHard { get; set; } = 300;
            public static bool TestSubjectBurningOpener { get; set; } = true;
            public static bool TestSubjectIntangibleOpener { get; set; } = true;
            [SliderRange(1, 10, 1)]
            public static double TestSubjectBurningAmountEasy { get; set; } = 2;
            [SliderRange(1, 10, 1)]
            public static double TestSubjectBurningAmountHard { get; set; } = 3;
            /*
            public static bool TestSubjectIntangiblePhases { get; set; } = true;
            public static bool TestSubjectAttack1Constrict { get; set; } = true;
            public static bool TestSubjectAttack2Constrict { get; set; } = true;
            [SliderRange(1, 20, 1)]
            public static double TestSubjectConstrictAmountEasy { get; set; } = 4;
            [SliderRange(1, 20, 1)]
            public static double TestSubjectConstrictAmountHard { get; set; } = 5;
            */
            [SliderRange(5, 60, 1)]
            public static double TestSubjectBiteDamageEasy { get; set; } = 18;
            [SliderRange(5, 60, 1)]
            public static double TestSubjectBiteDamageHard { get; set; } = 19;
            [SliderRange(5, 60, 1)]
            public static double TestSubjectBashDamageEasy { get; set; } = 12;
            [SliderRange(5, 60, 1)]
            public static double TestSubjectBashDamageHard { get; set; } = 13;
            [SliderRange(5, 60, 1)]
            public static double TestSubjectMultiDamageEasy { get; set; } = 8;
            [SliderRange(5, 60, 1)]
            public static double TestSubjectMultiDamageHard { get; set; } = 8;

        // ELITES
        [ConfigSection("SoulNexusSection")]
            public static bool SoulNexusEnabled { get; set; } = true;
            [SliderRange(1, 8, 1)]
            public static double SoulNexusHits { get; set; } = 5;

        [ConfigSection("KnightsSection")]
            public static bool KnightsEnabled { get; set; } = true;
            [SliderRange(1, 6, 1)]
            public static double KnightsGhostIntangible { get; set; } = 2;
            [SliderRange(10, 140, 1)]
            public static double KnightsGhostHPEasy { get; set; } = 88;
            [SliderRange(10, 140, 1)]
            public static double KnightsGhostHPHard { get; set; } = 92;
            public static bool KnightsMagiBlock { get; set; } = true;
            [SliderRange(2, 40, 1)]
            public static double KnightsMagiBlockEasy { get; set; } = 6;
            [SliderRange(2, 40, 1)]
            public static double KnightsMagiBlockHard { get; set; } = 8;
            [SliderRange(2, 40, 1)]
            public static double KnightsMagiPrepEasy { get; set; } = 16;
            [SliderRange(2, 40, 1)]
            public static double KnightsMagiPrepHard { get; set; } = 20;
            [SliderRange(10, 140, 1)]
            public static double KnightsMagiHPEasy { get; set; } = 72;
            [SliderRange(10, 140, 1)]
            public static double KnightsMagiHPHard { get; set; } = 76;

        // ENEMIES
        [ConfigSection("ScrollSection")]
            public static bool ScrollEnabled { get; set; } = true;
            [SliderRange(1, 5, 1)]
            [SliderLabelFormat("{0:0} hp")]
            public static double ScrollMaxHpReduction { get; set; } = 3;
            [SliderRange(10, 60, 1)]
            [SliderLabelFormat("{0:0} hp")]
            public static double ScrollMinMaxHpEasy { get; set; } = 38;
            [SliderRange(10, 60, 1)]
            [SliderLabelFormat("{0:0} hp")]
            public static double ScrollMaxMaxHpEasy { get; set; } = 44;
            [SliderRange(10, 60, 1)]
            [SliderLabelFormat("{0:0} hp")]
            public static double ScrollMinMaxHpHard { get; set; } = 40;
            [SliderRange(10, 60, 1)]
            [SliderLabelFormat("{0:0} hp")]
            public static double ScrollMaxMaxHpHard { get; set; } = 46;

        [ConfigSection("AxebotSection")]
            public static bool AxebotEnabled { get; set; } = true;
            [SliderRange(0, 3, 1)]
            public static double AxebotWeakAdd { get; set; } = 1;
            [SliderRange(0, 3, 1)]
            public static double AxebotVulnAdd { get; set; } = 1;

        [ConfigSection("GlobeheadSection")]
            public static bool GlobeheadEnabled { get; set; } = true;
            [SliderRange(1, 24, 1)]
            public static double GlobeheadPowerDamage { get; set; } = 12;

        [ConfigSection("LostForgottenSection")]
            public static bool LostForgottenEnabled { get; set; } = true;
    }
}