using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetAF.MyGame;

namespace NetAF.Game.Tests
{
    /// <summary>
    /// Tests for NodeMapState truth-table behaviour:
    /// derived flags must appear when prerequisites are satisfied
    /// and disappear when prerequisites are removed.
    /// </summary>
    [TestClass]
    public class NodeMapState_Tests
    {
        [TestInitialize]
        public void Setup()
        {
            // Reset to a clean state before every test
            NodeMapState.Reset();
        }

        // ---------------------------------------------------------------
        // Basic set / clear / has
        // ---------------------------------------------------------------

        [TestMethod]
        public void GivenCleanState_WhenSetFlag_ThenHasFlagIsTrue()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);

            Assert.IsTrue(NodeMapState.HasFlag(NodeMapState.ObjAFound));
        }

        [TestMethod]
        public void GivenFlagSet_WhenClearFlag_ThenHasFlagIsFalse()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);
            NodeMapState.ClearFlag(NodeMapState.ObjAFound);

            Assert.IsFalse(NodeMapState.HasFlag(NodeMapState.ObjAFound));
        }

        [TestMethod]
        public void GivenCleanState_WhenSetFlagBoolTrue_ThenHasFlagIsTrue()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound, true);

            Assert.IsTrue(NodeMapState.HasFlag(NodeMapState.ObjAFound));
        }

        [TestMethod]
        public void GivenFlagSet_WhenSetFlagBoolFalse_ThenHasFlagIsFalse()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);
            NodeMapState.SetFlag(NodeMapState.ObjAFound, false);

            Assert.IsFalse(NodeMapState.HasFlag(NodeMapState.ObjAFound));
        }

        [TestMethod]
        public void GivenNullFlag_WhenSetFlag_ThenNoException()
        {
            NodeMapState.SetFlag(null!);

            Assert.AreEqual(0, NodeMapState.FlagCount);
        }

        [TestMethod]
        public void GivenEmptyFlag_WhenSetFlag_ThenNoException()
        {
            NodeMapState.SetFlag(string.Empty);

            Assert.AreEqual(0, NodeMapState.FlagCount);
        }

        // ---------------------------------------------------------------
        // Derived flags appear when prerequisites satisfied
        // ---------------------------------------------------------------

        [TestMethod]
        public void GivenBothLexiconPrereqs_WhenSet_ThenConnLexiconDerived()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);
            NodeMapState.SetFlag(NodeMapState.ObjEFound);

            Assert.IsTrue(NodeMapState.HasConnection(NodeMapState.ConnLexicon));
        }

        [TestMethod]
        public void GivenBothHauntingPrereqs_WhenSet_ThenConnHauntingDerived()
        {
            NodeMapState.SetFlag(NodeMapState.ObjBFound);
            NodeMapState.SetFlag(NodeMapState.ObjEFound);

            Assert.IsTrue(NodeMapState.HasConnection(NodeMapState.ConnHaunting));
        }

        [TestMethod]
        public void GivenBothPhantomDatePrereqs_WhenSet_ThenConnPhantomDateDerived()
        {
            NodeMapState.SetFlag(NodeMapState.ObjCFound);
            NodeMapState.SetFlag(NodeMapState.ObjDFound);

            Assert.IsTrue(NodeMapState.HasConnection(NodeMapState.ConnPhantomDate));
        }

        // ---------------------------------------------------------------
        // HasFlag returns true for derived flags too
        // ---------------------------------------------------------------

        [TestMethod]
        public void GivenConnLexiconDerived_WhenHasFlag_ThenTrue()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);
            NodeMapState.SetFlag(NodeMapState.ObjEFound);

            Assert.IsTrue(NodeMapState.HasFlag(NodeMapState.ConnLexicon));
        }

        // ---------------------------------------------------------------
        // Derived flags disappear when prerequisites removed
        // ---------------------------------------------------------------

        [TestMethod]
        public void GivenConnLexiconActive_WhenClearObjA_ThenConnLexiconFalse()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);
            NodeMapState.SetFlag(NodeMapState.ObjEFound);

            Assert.IsTrue(NodeMapState.HasConnection(NodeMapState.ConnLexicon));

            NodeMapState.ClearFlag(NodeMapState.ObjAFound);

            Assert.IsFalse(NodeMapState.HasConnection(NodeMapState.ConnLexicon));
        }

        [TestMethod]
        public void GivenConnHauntingActive_WhenClearObjE_ThenConnHauntingFalse()
        {
            NodeMapState.SetFlag(NodeMapState.ObjBFound);
            NodeMapState.SetFlag(NodeMapState.ObjEFound);

            NodeMapState.ClearFlag(NodeMapState.ObjEFound);

            Assert.IsFalse(NodeMapState.HasConnection(NodeMapState.ConnHaunting));
        }

        [TestMethod]
        public void GivenConnPhantomDateActive_WhenClearObjD_ThenConnPhantomDateFalse()
        {
            NodeMapState.SetFlag(NodeMapState.ObjCFound);
            NodeMapState.SetFlag(NodeMapState.ObjDFound);

            NodeMapState.ClearFlag(NodeMapState.ObjDFound);

            Assert.IsFalse(NodeMapState.HasConnection(NodeMapState.ConnPhantomDate));
        }

        // ---------------------------------------------------------------
        // Partial prerequisites do NOT produce a derived flag
        // ---------------------------------------------------------------

        [TestMethod]
        public void GivenOnlyObjA_WhenSet_ThenConnLexiconFalse()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);

            Assert.IsFalse(NodeMapState.HasConnection(NodeMapState.ConnLexicon));
        }

        [TestMethod]
        public void GivenOnlyObjE_WhenSet_ThenConnLexiconFalse()
        {
            NodeMapState.SetFlag(NodeMapState.ObjEFound);

            Assert.IsFalse(NodeMapState.HasConnection(NodeMapState.ConnLexicon));
        }

        // ---------------------------------------------------------------
        // Determinism: setting the same flag twice is idempotent
        // ---------------------------------------------------------------

        [TestMethod]
        public void GivenFlag_WhenSetTwice_ThenIdempotent()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);
            NodeMapState.SetFlag(NodeMapState.ObjAFound);

            Assert.IsTrue(NodeMapState.HasFlag(NodeMapState.ObjAFound));
        }

        [TestMethod]
        public void GivenBothPrereqs_WhenSetAgain_ThenDerivedStillActive()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);
            NodeMapState.SetFlag(NodeMapState.ObjEFound);
            NodeMapState.SetFlag(NodeMapState.ObjAFound);  // duplicate

            Assert.IsTrue(NodeMapState.HasConnection(NodeMapState.ConnLexicon));
        }

        // ---------------------------------------------------------------
        // GetActiveDerived
        // ---------------------------------------------------------------

        [TestMethod]
        public void GivenNoPrereqs_WhenGetActiveDerived_ThenEmpty()
        {
            var derived = NodeMapState.GetActiveDerived();

            Assert.IsEmpty(derived);
        }

        [TestMethod]
        public void GivenAllThreeConnectionsUnlocked_WhenGetActiveDerived_ThenThreeEntries()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);
            NodeMapState.SetFlag(NodeMapState.ObjBFound);
            NodeMapState.SetFlag(NodeMapState.ObjCFound);
            NodeMapState.SetFlag(NodeMapState.ObjDFound);
            NodeMapState.SetFlag(NodeMapState.ObjEFound);

            var derived = NodeMapState.GetActiveDerived();

            Assert.HasCount(3, derived);
        }

        // ---------------------------------------------------------------
        // Reset
        // ---------------------------------------------------------------

        [TestMethod]
        public void GivenFlagsSet_WhenReset_ThenAllFlagsCleared()
        {
            NodeMapState.SetFlag(NodeMapState.ObjAFound);
            NodeMapState.SetFlag(NodeMapState.ObjEFound);

            NodeMapState.Reset();

            Assert.AreEqual(0, NodeMapState.FlagCount);
            Assert.IsFalse(NodeMapState.HasConnection(NodeMapState.ConnLexicon));
        }

        // ---------------------------------------------------------------
        // Evidence possession flags (HAS_EVIDENCE_*)
        // ---------------------------------------------------------------

        [TestMethod]
        public void GivenEvidenceFlag_WhenSet_ThenHasFlagIsTrue()
        {
            NodeMapState.SetFlag(NodeMapState.HasEvidenceHardDrive);

            Assert.IsTrue(NodeMapState.HasFlag(NodeMapState.HasEvidenceHardDrive));
        }

        [TestMethod]
        public void GivenEvidenceFlag_WhenCleared_ThenHasFlagIsFalse()
        {
            NodeMapState.SetFlag(NodeMapState.HasEvidenceHardDrive);
            NodeMapState.ClearFlag(NodeMapState.HasEvidenceHardDrive);

            Assert.IsFalse(NodeMapState.HasFlag(NodeMapState.HasEvidenceHardDrive));
        }

        // ---------------------------------------------------------------
        // GetRuleSchema
        // ---------------------------------------------------------------

        [TestMethod]
        public void WhenGetRuleSchema_ThenReturnsAllThreeRules()
        {
            var schema = NodeMapState.GetRuleSchema();

            Assert.HasCount(3, schema);
            Assert.IsTrue(schema.ContainsKey(NodeMapState.ConnLexicon));
            Assert.IsTrue(schema.ContainsKey(NodeMapState.ConnHaunting));
            Assert.IsTrue(schema.ContainsKey(NodeMapState.ConnPhantomDate));
        }

        [TestMethod]
        public void WhenGetRuleSchema_ThenLexiconRequiresObjAAndObjE()
        {
            var schema = NodeMapState.GetRuleSchema();
            var requires = schema[NodeMapState.ConnLexicon];

            CollectionAssert.Contains(requires, NodeMapState.ObjAFound);
            CollectionAssert.Contains(requires, NodeMapState.ObjEFound);
        }

        [TestMethod]
        public void WhenGetRuleSchema_ThenSchemaIsIndependentOfCurrentState()
        {
            // Schema should reflect all rules even with no flags set
            var schemaBefore = NodeMapState.GetRuleSchema();

            NodeMapState.SetFlag(NodeMapState.ObjAFound);
            NodeMapState.SetFlag(NodeMapState.ObjEFound);

            var schemaAfter = NodeMapState.GetRuleSchema();

            Assert.HasCount(schemaBefore.Count, schemaAfter);
        }
    }
}
