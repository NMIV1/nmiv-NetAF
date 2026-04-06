using NetAF.Assets;
using NetAF.MyGame;
using NetAF.Logic;
using NetAF.Rendering.FrameBuilders;
using NetAF.Targets.Console;

var creator = MyGame.Create(new GameConfiguration(new ConsoleAdapter(), FrameBuilderCollections.Console, Size.Dynamic));
GameExecutor.Execute(creator, new ConsoleExecutionController());
