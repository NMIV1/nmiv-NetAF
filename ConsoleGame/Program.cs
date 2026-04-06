using NetAF.Assets;
using NetAF.MyGame;
using NetAF.Logic;
using NetAF.Rendering.FrameBuilders;
using NetAF.Targets.Console;
using NetAF.Targets.Console.Rendering.FrameBuilders;

// Wire up the day clock display in the top-right corner
ConsoleSceneFrameBuilder.ClockProvider = DayClock.TickAndGetDisplayText;

var creator = MyGame.Create(new GameConfiguration(new ConsoleAdapter(), FrameBuilderCollections.Console, Size.Dynamic));
GameExecutor.Execute(creator, new ConsoleExecutionController());
