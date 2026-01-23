[![](https://dcbadge.limes.pink/api/server/AEfHJGwggC)](https://discord.gg/AEfHJGwggC)

[English](README.md) | [简体中文]

> 本模组不隶属于 Among Us 或 Innersloth LLC，其包含的内容也未得到 Innersloth LLC 的认可或以其他方式赞助。
此处包含的部分材料是Innersloth LLC的财产。
© Innersloth LLC.

> 此文档的简体中文翻译来自[@TAIKongguo](https://github.com/TAIKgroup)

# Mira API

一个全面并且简单的AmongUs开发辅助API和程序集，包括：
- 职业
- 选项
- 附加职业（副职业）
- 按钮
- 自定义颜色
- 事件
- 投票
- 资源
- 兼容性
- ~~游戏模式~~  (即将完成)


Mira API力求全面而简单，同时尽可能使用游戏基本元素。
是一个为了让模组开发更简单的API，涵盖了多个方面。

**加入我们的 [Discord](https://discord.gg/FYYqJU2bvp) 获取支持并了解最新版本**

# 用法

在使用 Mira API 之前,您需要知道:
1. 通过 [DLL](https://github.com/All-Of-Us-Mods/MiraAPI/releases)，项目引用或 [NuGet包](https://www.nuget.org/packages/AllOfUs.MiraAPI)添加对Mira API的引用。
2. 在plugin class上添加一个BepInRubstance，如下所示： `[BepInDependency(MiraApiPlugin.Id)]`
3. 在plugin class中实现IMiraPlugin接口。

Mira API 也依赖 [Reactor](https://github.com/NuclearPowered/Reactor) 进行工作!
不要忘记引用 它和 `BepInDependency`!

有关完整实例，请参考 [这个文件](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/ExamplePlugin.cs).

## 推荐项目结构
强烈建议在使用Mira API时遵循此项目结构，以保持代码的整洁和有序。
您还可以查看此存储库中的示例Mod以获取一些指导。
```
MyMiraMod/
├── Buttons/
│   └── MyCoolButton.cs
├── Options/
│   ├── Roles/
│   │   └── CoolCustomRoleOptions.cs
│   └── MainOptionGroup.cs
├── Patches/
│   ├── Roles/
│   │   └── CoolCustomRole/
│   │       ├── PlayerControlPatches.cs
│   │       └── ExileControllerPatches.cs
│   └── General/
│       └── HudManagerPatches.cs
├── Resources/
│   ├── CoolButton.png
│   └── myAssets-win-x86.bundle
├── Roles/
│   └── CoolCustomRole.cs
├── MyMiraModPlugin.cs
└── MyModAssets.cs
```

## 职业
在Mira API中，创建自定义职业只需要需要做这两件事:
1. 创建一个 AmongUs 原版的职业类 (如 `CrewmateRole`, `ImpostorRole`, 等等。) 
2. 通过 Mira API 实现 `ICustomRole`。

**免责声明: 请确保您的插件类具有以下属性 `[ReactorModFlags(ModFlags.RequireOnAllClients)]` 否则您将无法正常创建一个职业。**

注意: 对于步骤 1, 如果您想创建一个中立职业，您可以选择,
 `CrewmateRole` 或 `ImpostorRole`；
 
如果您可以从头独立完成一个职业，您也可以选择使用 `RoleBehaviour` 。

Mira API 帮助您处理其他的一切事务，
包括在设置菜单中添加正确的选项，在游戏开始时正确分配每个职业。
您可以尽情发挥您的才艺，肆意创造您心目中的职业.


有关完整实例，请参考 [这个文件](https://github.com/All-Of-Us-Mods/MiraAPI/blob/teams/MiraAPI.Example/Roles/CustomRole.cs)。
（原文档链接搜索不到网页，此处进行了修改）

## 附加职业（副职业）
Mira API 中的附加职业（副职业）与其他模组并不完全相同。
例如，在 Town Of Us 中，附加职业（副职业）是基于主职业的额外能力。
但是，在Mira API 中，附加职业（副职业）是非常灵活的，
它们是玩家额外的能力或可互动的其他东西。

Mira API 提供了3个类来处理附加职业（副职业）:
- `BaseModifier`: 每个附加职业（副职业）的基础。您必须手动在玩家中添加和删除此修改器！
- `TimedModifier`: 有事件限制的附加职业（副职业）。此附加职业（副职业）必须手动添加，但会在到达时间限制后自动删除。
- `GameModifier`: 这类似典型的 Town Of Us 的附加职业（副职业），在游戏开始时自动添加，在游戏结束后删除。

附加职业（副职业）为自定义模组提供了各种可覆写的函数和属性。
它们还可以用来“标记”玩家
您可以通过 `PlayerControl` 对象上的扩展方法 `HasModifier` 来检查玩家是否拥有附加职业（副职业）/

若想在游戏开始时使用附加职业（副职业），请选择上文中的一个类，并创建一个继承自它的类。
实现您想要的效果，然后将 `[RegisterModifier]` 属性添加到这个类中。

有关普通附加职业（副职业）的实例，请参考 [这个文件](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/Modifiers/GameModifierExample.cs).

有关限时附加职业（副职业）的实例，请参考 [这个文件](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/Modifiers/ModifierTimerExample.cs).

（此文档链接搜索不到网页）

## Options
在 Mira API 中，选项也十分简单。
Mira API 在后台处理所有繁重的工作，
您只需要遵循几个简单的步骤就可以创建自定义选项。
选项分为组和选项，每个选项都必须属于一个组。

要创建组，您需要一个继承自 `AbstractOptionGroup` （抽象类）的类。
组包含一些供开发人员控制的属性，
包含 `GroupName`， `GroupColor`， `GroupVisible`, and `OptionableType`.
属性的完整列表可以在 `AbstractOptionGroup`类中找到。
只需要 `GroupName`就可以创建一个组。

这是一个组的代码实例:
```csharp
public class MyOptionsGroup : AbstractOptionGroup
{
    public override string GroupName => "My Options"; // this is required
    
    [ModdedNumberOption("My Number Option", min: 0, max: 10)]
    public float MyNumberOption { get; set; } = 5f;
}
```

您可以使用 `OptionGroupSingleton` 类访问任何组，如下所示:
```csharp
// MyOptionsGroup是一个继承自AbstractOptionGroup的类
var myGroup = OptionGroupSingleton<MyOptionsGroup>.Instance; // gets the instance of the group
Logger<MyPlugin>.Info(myGroup.MyNumberOption); // prints the value of the option to the console
```

一旦你有了一个组，有两种方法可以制作选项：
- 使用 Option Attribute with a property.
- 创建一个 ModdedOption property.

### Option Attributes

这是使用Option Attribute with a property的示例：
```csharp
// 第一个参数始终是选项的名称。其余的取决于选项的类型。
[ModdedNumberOption("Sussy level", min: 0, max: 10)]
public float SussyLevel { get; set; } = 4f; // You can set a default value here.
```

这是使用available Option Attributes的例示：
```csharp
ModdedEnumOption(string name, Type enumType, string[]? values = null, Type? roleType = null)
    
ModdedNumberOption(
    string name,
    float min,
    float max,
    float increment=1
    NumberSuffixes suffixType = NumberSuffixes.None,
    bool zeroInfinity = false,
    Type? roleType = null)

ModdedToggleOption(string name, Type? roleType = null)
```

### ModdedOption Properties

这是使用ModdedOption的示例：
```csharp
public ModdedToggleOption YeezusAbility { get; } = new ModdedToggleOption("Yeezus Ability", false);
```

目前，您可以创建三种类型的ModdedOptions：
- `ModdedEnumOption`
- `ModdedNumberOption`
- `ModdedToggleOption`

有关完整实例，请参考[这个文件](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/Options/ExampleOptions.cs).

### 职业选项

您还可以为选项或组指定为一个职业的选项。

要为整个组设置职业，请按如下代码设置该组的 `OptionableType` 属性：
```csharp
public class MyOptionsGroup : AbstractOptionGroup
{
    public override string GroupName => "My Options";
    public override Type? OptionableType => typeof(MyRole); // this is the role that will have these options
    
    [ModdedNumberOption("Ability Uses", min: 0, max: 10)]
    public float AbilityUses { get; set; } = 5f;
}
```

您还可以指定 `OptionableType` 为附加职业（副职业）的选项。
这将把该组与指定附加职业（副职业）相关联，
并将其显示在附加职业（副职业）选项菜单中。

有关完整实例，请参考[这个文件](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/Options/Roles/CustomRoleSettings.cs)。

（原文档链接搜索不到网页，此处进行了修改）

## 自定义击杀
Mira API 提供了自定义的击杀方式。
我们允许开发者添加更多的击杀类别，并有助于绕过服务器检查。
您可以使用 `PlayerControl.RpcCustomMurder` 或 `PlayerControl.CustomMurder` 
进行自定义击杀，例如：

```cs
PlayerControl.LocalPlayer.RpcCustomMurder(Target, createDeadBody: false, teleportMurderer: false, playKillSound: false, resetKillTimer: false, showKillAnim: false);
```
这将杀死一名玩家，而不会造成尸体，也不会让击杀者瞬移。

## 按钮

Mira API 提供了一个简单的界面，用于向游戏添加功能按钮。

您只需要添加一个继承自 `CustomActionButton` 的类，并实现属性和方法。

Mira API 处理将按钮添加到游戏中所需的所有其他任务和逻辑。

如果你需要从另一个类访问 `CustomActionButton` 你可以使用，
 `CustomButtonSingleton` ，如下所示:
 
```csharp
var myButton = CustomButtonSingleton<MyCoolButton>.Instance;
```

按钮十分简单，但提供了巨大的灵活性。
您可以使用多种方法来自定义按钮。
有关按钮的方法和完整列表，请参考 [这个文件](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI/Hud/CustomActionButton.cs) 

[这里](https://github.com/All-Of-Us-Mods/MiraAPI/blob/teams/MiraAPI.Example/Buttons/ExampleButton.cs)
是一个按钮的实例代码。

（原文档链接搜索不到网页，此处进行了修改）

## 自定义颜色

Mira API提供了简单的自定义颜色，允许您将自定义玩家颜色添加到游戏中。

创建自定义颜色并不困难，但Mira API对创建您的颜色有一些要求。

1. 创建***STATIC***类来容纳所有 `CustomColor` 对象. 
2. 在这个类中，为要添加的每种颜色创建 `CustomColor` 属性。
3. 将 `[RegisterCustomColors]`属性 添加到这个类中。

以下是一个自定义颜色的示例：
```csharp
[RegisterCustomColors]
public static class MyCustomColors
{
    public static CustomColor Cerulean { get; } = new CustomColor("Cerulean", new Color(0.0f, 0.48f, 0.65f)); 

    public static CustomColor Rose { get; } = new CustomColor("Rose", new Color(0.98f, 0.26f, 0.62f));
    
    public static CustomColor Gold { get; } = new CustomColor("Gold", new Color(1.0f, 0.84f, 0.0f));
}
```

## 资源

Mira API 提供了一个简单且可扩展的资源系统。
该系统的核心是 `LoadableAsset<T>` 类。
这是一个通用的抽象类，它提供了一种加载资源的模式。

Mira API 附带了一些资源加载器：
1. `LoadableBundleAsset<T>`: 这适用于从 AssetBundles 加载资源。
2. `LoadableAddressableAsset<T>`: 这适用于从 Addressables 加载资源。
3. `LoadableResourceAsset`: 这适用于从模组加载嵌入式资源。
4. `LoadableAudioResourceAsset`: 这适用于从模组加载嵌入式音频资源。

下面的代码展示了如何使用资源加载器：
```csharp
// Load a sprite from an AssetBundle
AssetBundle bundle = AssetBundleManager.Load("MyBundle"); // AssetBundleManager is a utility provided by Reactor
LoadableAsset<Sprite> mySpriteAsset = new LoadableBundleAsset<Sprite>("MySprite", bundle);
Sprite sprite = mySpriteAsset.LoadAsset();

// 加载嵌入式资源
// 确保将映像的“构建操作”设置为“嵌入式资源”！
LoadableAsset<Sprite> buttonAsset = new LoadableResourceAsset("ExampleMod.Resources.MyButton.png");
Sprite button = buttonSpriteAsset.LoadAsset();
```

您可以查看[例示文件](https://github.com/All-Of-Us-Mods/MiraAPI/blob/master/MiraAPI.Example/ExampleAssets.cs).

您可以通过继承 `LoadableAsset<T>` 类并实现 `LoadAsset`方法来创建自己的资产加载器。
