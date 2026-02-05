# 图像揭示动画预制体配置说明

## 概述
此预制体用于实现"显示图像一部分，然后通过动画还原整体图像"的效果。

## Unity中创建预制体步骤

### 1. 创建UI层级结构

在Unity编辑器中按照以下层级创建GameObject：

```
Canvas
└── ImageRevealContainer (空GameObject)
    ├── MaskPanel (Panel, 添加Mask组件)
    │   └── SuspectImage (Image, 使用嫌疑人照片.png)
    └── ImageRevealAnimation (空GameObject, 添加ImageRevealAnimation脚本)
```

### 2. 详细配置步骤

#### 步骤1: 创建Canvas
- 在Hierarchy中右键 → UI → Canvas
- 设置Canvas Scaler为 "Scale With Screen Size"
- Reference Resolution: 1920x1080

#### 步骤2: 创建ImageRevealContainer
- 在Canvas下右键 → Create Empty
- 重命名为 "ImageRevealContainer"
- 设置Rect Transform:
  - Anchors: (0.5, 0.5, 0.5, 0.5) - 居中
  - Pivot: (0.5, 0.5)
  - Position: (0, 0, 0)
  - Width: 800
  - Height: 600

#### 步骤3: 创建MaskPanel
- 在ImageRevealContainer下右键 → UI → Panel
- 重命名为 "MaskPanel"
- 添加Mask组件: Add Component → Mask
- 设置Image组件的Color为白色
- 设置Rect Transform:
  - Anchors: (0, 0, 1, 1) - 拉伸填充
  - Pivot: (0.5, 0.5)
  - Position: (0, 0, 0)
  - Width: 100 (初始显示宽度)
  - Height: 100 (初始显示高度)

#### 步骤4: 创建SuspectImage
- 在MaskPanel下右键 → UI → Image
- 重命名为 "SuspectImage"
- 设置Source Image: 拖入 `Assets/Arts/img/嫌疑人照片.png`
- 设置Image Type: Simple
- 设置Preserve Aspect: 勾选
- 点击Set Native Size按钮
- 设置Rect Transform:
  - Anchors: (0.5, 0.5, 0.5, 0.5) - 居中
  - Pivot: (0.5, 0.5)
  - Position: (0, 0, 0)

#### 步骤5: 创建ImageRevealAnimation脚本对象
- 在ImageRevealContainer下右键 → Create Empty
- 重命名为 "ImageRevealAnimation"
- 添加ImageRevealAnimation脚本: Add Component → Image Reveal Animation

#### 步骤6: 配置ImageRevealAnimation脚本
在Inspector中配置以下参数：

**组件引用:**
- Mask Rect: 拖入MaskPanel对象
- Target Image: 拖入SuspectImage对象

**动画设置:**
- Animation Duration: 1.0 (动画持续时间，单位秒)
- Easing Type: EaseOutCubic (缓动类型)
- Start Revealed: 不勾选 (开始时不显示完整图像)

**初始显示区域设置:**
- Initial Size: (100, 100) (初始显示的宽度和高度)
- Initial Position: (0, 0) (初始显示的中心位置)

**事件:**
- On Reveal Start: 可以添加动画开始时的事件
- On Reveal Complete: 可以添加动画完成时的事件

### 3. 保存为预制体

- 选中ImageRevealContainer
- 在Project窗口中，导航到Assets/Prefabs目录
- 将ImageRevealContainer拖拽到Prefabs目录中
- 预制体创建完成

## 使用方法

### 方法1: 通过代码触发
```csharp
// 获取ImageRevealAnimation组件
ImageRevealAnimation revealAnim = GetComponent<ImageRevealAnimation>();

// 触发揭示动画
revealAnim.TriggerRevealAnimation();

// 触发隐藏动画
revealAnim.TriggerHideAnimation();

// 重置到初始状态
revealAnim.ResetToInitialState();
```

### 方法2: 通过Unity事件触发
- 在按钮或其他触发器上添加OnClick事件
- 将ImageRevealAnimation对象拖入事件槽
- 选择函数: ImageRevealAnimation → TriggerRevealAnimation

### 方法3: 通过Inspector事件配置
- 在ImageRevealAnimation脚本的On Reveal Start/Complete事件中
- 添加需要触发的其他函数

## 自定义配置

### 调整初始显示区域
- 修改Initial Size: 改变初始显示区域的大小
- 修改Initial Position: 改变初始显示区域的位置

### 调整动画效果
- 修改Animation Duration: 改变动画速度
- 修改Easing Type: 选择不同的缓动效果
  - Linear: 线性匀速
  - EaseInQuad: 缓入
  - EaseOutQuad: 缓出
  - EaseInOutQuad: 缓入缓出
  - EaseInCubic: 三次缓入
  - EaseOutCubic: 三次缓出 (推荐)
  - EaseInOutCubic: 三次缓入缓出

### 代码动态调整
```csharp
// 设置动画持续时间
revealAnim.SetAnimationDuration(2f);

// 设置缓动类型
revealAnim.SetEasingType(ImageRevealAnimation.EasingType.EaseOutQuad);
```

## 常见问题

### Q: 图像没有正确裁剪？
A: 确保MaskPanel添加了Mask组件，并且SuspectImage是MaskPanel的子对象。

### Q: 动画播放不流畅？
A: 检查Animation Duration是否设置合理，建议设置为0.5-2秒之间。

### Q: 如何改变初始显示的位置？
A: 修改Initial Position参数，或者调整MaskPanel的初始Anchored Position。

### Q: 如何在动画完成后执行其他操作？
A: 在On Reveal Complete事件中添加需要执行的函数。

## 示例场景配置

### 场景1: 从左上角展开
- Initial Size: (100, 100)
- Initial Position: (-350, 250) (根据图像大小调整)

### 场景2: 从中心展开
- Initial Size: (100, 100)
- Initial Position: (0, 0)

### 场景3: 从右侧展开
- Initial Size: (100, 600)
- Initial Position: (350, 0) (根据图像大小调整)

## 文件位置

- 脚本: `Assets/Controller/ImageRevealAnimation.cs`
- 图像资源: `Assets/Arts/img/嫌疑人照片.png`
- 预制体: `Assets/Prefabs/ImageRevealContainer.prefab` (创建后)
