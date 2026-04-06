using Microsoft.VisualStudio.TestTools.UnitTesting;

// Disable parallel execution — NodeMapState is static and tests reset state via [TestInitialize]
[assembly: Parallelize(Workers = 1, Scope = ExecutionScope.MethodLevel)]
