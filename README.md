# Unity Node Graph

基于Unity的可视化节点图系统，用于创建和执行逻辑流。

## ✨ 特性

- 🎨 可视化编辑器 - 直观的GraphView节点编辑器
- 🔗 端口连接系统 - 灵活的输入/输出端口连接
- 💾 多种存储方式 - ScriptableObject和MonoBehaviour支持
- ⚡ 高效执行引擎 - 基于Tick的图运行系统
- 🏗️ 高度可扩展 - 通过继承轻松创建自定义节点
- 🔄 并行执行支持 - ParallelNode实现并行任务

## 📑 目录

- [快速开始](#快速开始)
- [安装](#安装)
- [核心概念](#核心概念)
- [使用教程](#使用教程)
- [编辑器指南](#编辑器指南)
- [API参考](#api参考)
- [样式定制](#样式定制)
- [常见问题](#常见问题)
- [开发规范](#开发规范)

---

## 🚀 快速开始

### 运行示例（5分钟）

1. 打开场景
   ```
   Assets/Examples/01_logic_flow/Example01.unity
   ```

2. 查看示例图
   - 选择 `Assets/Examples/01_logic_flow/Resources/Graph/logic_graph_01.asset`
   - 点击Inspector的 "open graph" 按钮

3. 运行
   - 按Play按钮
   - 观察Console输出

### 创建第一个节点

```csharp
[NodeMenu("Demo/HelloNode", "你好节点")]
public class HelloNode : BaseNode
{
    protected override void OnInit()
    {
        Debug.Log("节点初始化");
    }

    protected override bool OnTick(float dt, FlowContext ctx)
    {
        Debug.Log("Hello Node!");
        ImpulseOutPort("out", ctx);
        return true;
    }

    public override void OnSerialize() { }
    public override void Deserialize() { }
}
```

```csharp
[NodeTargetEditor(typeof(HelloNode))]
public class HelloNodeEditor : BaseNodeView
{
    protected override void OnInit()
    {
        DrawButton("点击我", () => Debug.Log("按钮被点击"));
    }
}
```

---

## 📦 安装

### 集成到项目

1. 复制 `Assets/NodeGraph/` 到你的项目

2. 验证安装
   - 查找菜单 `Graph/01 DefaultGraph`
   - 查找菜单 `Assets/Create/Scriptable/LogicFlowGraph`

### 创建新图

**ScriptableObject方式**:
```
Assets > Create > Scriptable/LogicFlowGraph
```

**MonoBehaviour方式**:
```csharp
public class MyGraphComponent : GraphComponent
{
    // 可添加到GameObject
}
```

---

## 🧠 核心概念

### 节点生命周期

```
创建 → OnInit() → ImpulseInPort() → OnTick() → MarkDone()
```

| 阶段 | 方法 | 说明 |
|-------|------|------|
| 初始化 | `OnInit()` | 重置节点状态 |
| 激活 | `ImpulseInPort()` | 接收输入信号 |
| 执行 | `OnTick()` | 每帧处理逻辑 |
| 完成 | `MarkDone()` | 标记节点完成 |

### 端口系统

**端口类型**:
- **输入端口** (Input) - 接收信号
- **输出端口** (Output) - 发送信号

**连接模式**:
- **单连接** - 只连接一个端口
- **多连接** - 可连接多个端口

```csharp
// 端口配置示例
public override List<PortData> inputPortIds => new()
{
    new PortData("trigger"),     // 单连接
    new PortData("data", true)   // 多连接
};
```

### 图执行流程

```
EnterNode → [Node A] → [Node B] → 完成
           └→ [Node C] ─┘   // 并行分支
```

1. EnterNode作为起始点
2. 通过ImpulseOutPort触发下一节点
3. 节点在OnTick中处理逻辑
4. 返回true表示节点完成
5. 所有节点完成后图执行结束

### 节点类型

| 类型 | 用途 |
|------|------|
| EnterNode | 图的入口，无输入端口 |
| BaseNode | 标准节点，需实现OnTick |
| ParallelNode | 并行节点，同时执行多个分支 |

---

## 📚 使用教程

### 教程1: 延时节点

```csharp
[NodeMenu("Demo/DelayNode", "延时节点")]
[Serializable]
public class DelayNode : BaseNode
{
    [SerializeField] private float delayTime = 1.0f;
    private float m_timer = 0;

    public override List<PortData> outputPortIds
        => new() { new PortData("out", true) }; // 多输出

    protected override bool OnTick(float dt, FlowContext ctx)
    {
        if (!isActive) return false;

        m_timer += dt;
        if (m_timer >= delayTime)
        {
            ImpulseOutPort("out", ctx);
            return true;
        }
        return false;
    }

    protected override void OnInit()
    {
        m_timer = 0;
    }

    public override void OnSerialize() { }
    public override void Deserialize() { }

#if UNITY_EDITOR
    public void SetDelay(float value) => delayTime = value;
#endif
}
```

### 教程2: 条件分支

```csharp
[NodeMenu("Demo/BranchNode", "条件分支")]
public class BranchNode : BaseNode
{
    [SerializeField] private bool condition = true;

    public override List<PortData> outputPortIds => new()
    {
        new PortData("true"),
        new PortData("false")
    };

    protected override bool OnTick(float dt, FlowContext ctx)
    {
        if (!isActive) return false;

        string port = condition ? "true" : "false";
        ImpulseOutPort(port, ctx);
        return true;
    }

    protected override void OnInit() { }
    public override void OnSerialize() { }
    public override void Deserialize() { }
}
```

### 教程3: 运行时执行

```csharp
public class GraphRunner : MonoBehaviour
{
    public GraphScriptable graph;
    private BaseFlow m_flow;

    void Start()
    {
        if (graph?.graph.enterNode != null)
        {
            m_flow = new BaseFlow();
            m_flow.Init(graph.graph.enterNode);
            m_flow.Start();
        }
    }

    void Update()
    {
        m_flow?.Tick(Time.deltaTime);
    }

    public void RestartGraph()
    {
        m_flow?.Restart();
    }
}
```

---

## 🎨 编辑器指南

### 基本操作

| 操作 | 方法 |
|------|------|
| 添加节点 | 右键空白处 → 选择节点 |
| 移动节点 | 拖拽节点标题栏 |
| 删除元素 | 选中后按Delete |
| 连接端口 | 拖拽端口到另一个端口 |
| 断开连接 | 选中连接线后按Delete |

### 快捷键

| 按键 | 功能 |
|------|------|
| `Ctrl+S` | 保存图 |
| `Ctrl+Shift+S` | 另存为 |
| `Delete` | 删除选中 |
| 右键 | 打开节点菜单 |

### 工具栏

- **Save** - 保存当前图
- **Save As** - 另存为新文件
- **Refresh** - 刷新视图
- **编辑模式** - 切换编辑/运行模式

### 节点状态颜色

| 颜色 | 状态 |
|------|------|
| 灰色 | 未激活 |
| 绿色 | 正在执行 |
| 红色 | 已完成 |

---

## 📖 API参考

### BaseNode 核心API

```csharp
// 抽象方法（必须实现）
protected abstract void OnInit();
public abstract void OnSerialize();
public abstract void Deserialize();

// 虚拟方法（可选重写）
protected virtual bool OnTick(float dt, FlowContext ctx);
protected virtual void OnImpulseInPort(string portName, FlowContext ctx);
protected virtual void OnFinished();

// 公共属性
public int id { get; }
public Rect position { get; set; }
public bool isDone { get; }
public bool isActive { get; }
public virtual string name { get; }

// 端口定义
public virtual List<PortData> inputPortIds { get; }
public virtual List<PortData> outputPortIds { get; }

// 操作方法
public void Reset();
public void ImpulseInPort(string portName, FlowContext ctx);
protected void ImpulseOutPort(string portName, FlowContext ctx);
public NodePort GetPort(NodePortType portType, string portName);
```

### BaseNodeView UI控件

```csharp
// 添加UI控件
protected Toggle DrawToggle(string text, bool value, EventCallback<ChangeEvent<bool>> callback);
protected FloatField DrawFloatField(string text, float value, EventCallback<ChangeEvent<float>> callback);
protected IntegerField DrawIntegerField(string text, int value, EventCallback<ChangeEvent<int>> callback);
protected Button DrawButton(string text, Action action);

// 端口控制
protected void SetPortsEnabled(bool enabled);

// 节点视图引用
public BaseNode target { get; }
public BaseGraphView graphView { get; }
```

### BaseFlow 执行控制

```csharp
// 构造
public BaseFlow();

// 流程控制
public void Init(EnterNode enterNode);
public void Start();
public void Stop();
public void Restart();
public void Tick(float dt);
```

### PortData 端口配置

```csharp
public struct PortData
{
    public string name;           // 端口名称
    public bool multiLink;        // 是否多连接
    public bool vertical;         // 是否垂直布局

    public PortData(string name, bool multiLink = false);
}
```

---

## 🎨 样式定制

### USS样式文件

```
Assets/NodeGraph/Editor/Src/Styles/
├── BaseNodeView.uss      # 节点样式
├── BaseGraphView.uss     # 图视图样式
└── GraphBackGround.uss    # 背景样式
```

### 自定义节点颜色

修改 `BaseNodeView.cs` 中的颜色常量：

```csharp
protected static readonly Color node_color_normal = Color.gray;      // 未激活
protected static readonly Color node_color_active = Color.green;     // 执行中
protected static readonly Color node_color_finished = Color.red;      // 已完成
protected static readonly Color port_color_in = Color.cyan * 0.5f;   // 输入端口
protected static readonly Color port_color_out = Color.yellow * 0.5f; // 输出端口
```

### 自定义样式步骤

1. 编辑 `BaseNodeView.uss`
2. 修改CSS样式规则
3. 调整节点布局
4. 自定义控件外观

---

## ❓ 常见问题

### 节点不执行？

**检查清单**:
- [ ] 节点是否正确连接（输入→输出）
- [ ] EnterNode是否存在并已连接
- [ ] OnTick是否返回true
- [ ] Flow是否在Update中调用Tick
- [ ] 编辑器模式是否切换到运行模式

### 序列化失败？

**解决方案**:
1. 添加 `[Serializable]` 属性到节点类
2. 确保实现了 `OnSerialize()` 和 `Deserialize()`
3. 检查复杂字段是否支持Unity序列化

### 如何调试节点？

```csharp
protected override bool OnTick(float dt, FlowContext ctx)
{
    // 添加调试信息
    Debug.Log($"[{name}] Tick: active={isActive}, done={isDone}");

    // 使用try-catch捕获错误
    try
    {
        // 节点逻辑
    }
    catch (System.Exception e)
    {
        Debug.LogError($"节点执行错误: {e}");
    }

    return true;
}
```

### 如何创建循环节点？

```csharp
[NodeMenu("Demo/LoopNode", "循环节点")]
public class LoopNode : BaseNode
{
    [SerializeField] private int count = 3;
    private int m_current = 0;

    protected override bool OnTick(float dt, FlowContext ctx)
    {
        if (!isActive)
        {
            m_current = 0;
            return false;
        }

        m_current++;
        if (m_current < count)
        {
            // 继续循环
            ImpulseOutPort("loop", ctx);
        }
        else
        {
            // 循环完成
            ImpulseOutPort("done", ctx);
            return true;
        }

        return false;
    }

    public override List<PortData> outputPortIds => new()
    {
        new PortData("loop"),
        new PortData("done")
    };

    protected override void OnInit() { }
    public override void OnSerialize() { }
    public override void Deserialize() { }
}
```

### 图文件太大？

**优化建议**:
- 将大型图拆分为多个子图
- 使用注释节点标记区域
- 删除未使用的节点
- 考虑使用Prefab复用节点结构

---

## 📏 开发规范

### 命名约定

| 类型 | 约定 | 示例 |
|------|--------|------|
| 类名 | PascalCase | `MyCustomNode` |
| 方法 | PascalCase | `OnInit`, `OnTick` |
| 私有字段 | m_camelCase | `m_flow`, `m_timer` |
| 公有字段/属性 | camelCase | `graph`, `delayTime` |
| 常量 | UPPER_CASE | `INPUT_PORT`, `MAX_COUNT` |

### 代码风格

- 缩进：4个空格
- 大括号：Allman风格（换行）
- 优先使用 `var` 关键字
- Editor代码使用 `#if UNITY_EDITOR` 条件编译
- 异常处理：使用try-catch包裹OnTick

### 文件组织

```
Assets/
├── NodeGraph/
│   ├── Runtime/
│   │   ├── Graph/          # 图数据相关
│   │   └── Flow/           # 流程执行相关
│   └── Editor/
│       ├── Views/          # 视图组件
│       └── Utils/          # 工具类
└── Examples/
    └── 01_logic_flow/
        ├── RunTime/        # 运行时示例
        └── Editor/         # 编辑器示例
```

### 最佳实践

1. **单一职责** - 每个节点只做一件事
2. **状态管理** - 在OnInit中重置所有状态
3. **性能优化** - 避免在OnTick中频繁分配
4. **错误处理** - 使用try-catch保护OnTick
5. **可重用性** - 设计通用节点而非特定场景
6. **版本控制** - 图资源纳入Git管理

---

## 🔗 依赖

- Unity 2021.3+
- Unity UIElements
- Unity GraphView

## 📄 许可证

请查看项目根目录的LICENSE文件。

## 🤝 贡献

欢迎提交Issue和Pull Request！

---

> 💡 **提示**: 这是一个学习项目，展示了Unity节点图编辑器的实现原理。
