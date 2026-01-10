# DOTween 最全基础入门+简明语法+完整示例（Unity必备）
你需要的是 **DOTween 极简入门+核心基础语法+可直接运行的示例代码**，这份内容从基础到核心用法全覆盖，新手友好，所有示例都能直接复制使用，适合快速上手。
> 前置说明：DOTween 是 Unity 最主流的补间动画插件，功能远强于Unity自带的`iTween`，语法简洁、性能优秀，支持几乎所有Unity常用属性的平滑过渡动画。

---

## ✅ 一、DOTween 前期准备（必做，否则报错）
### 1. 导入DOTween
- 方式1（推荐）：Unity Package Manager 中搜索 `DOTween` 安装官方包
- 方式2：官网下载导入，导入后会弹出 `DOTween Setup` 窗口，点击`Setup DOTween`完成初始化即可

### 2. 必须引用命名空间
所有要写DOTween动画的脚本，**第一行必须加**，否则代码无法识别：
```csharp
using DG.Tweening;
```

---

## ✅ 二、DOTween 核心基础概念（2个核心，必懂）
DOTween的所有动画，都基于 **2种调用方式**，无其他例外，理解这2个概念，语法就懂了一半！
### ✔ 1. 快捷方式（推荐新手，最常用）
**语法格式**：`目标对象.DO+要执行的动画(参数1, 参数2, 动画时长);`
- 本质：Unity的`Transform/Vector3/Color`等常用类，DOTween都做了**扩展方法**
- 优点：**语法极简、可读性极强**，一行代码完成动画，90%的业务需求用这种方式就够了
- 示例：`transform.DOMove(Vector3.right*5, 2);` 物体2秒移动到右侧5单位的位置

### ✔ 2. 静态方式（进阶用法）
**语法格式**：`DOTween.To(取值委托, 赋值委托, 目标值, 动画时长);`
- 本质：DOTween的静态方法，通过委托自定义「要改变的属性」
- 优点：**万能通用**，可以给「任何自定义属性」做动画（比如血量值、音量、自定义脚本的参数）
- 缺点：语法稍复杂，适合快捷方式实现不了的自定义动画需求
- 示例：`DOTween.To(()=>hp, val=>hp=val, 0, 3);` 3秒内让变量`hp`从当前值平滑降到0

---

## ✅ 三、DOTween 核心基础语法 & 常用动画示例（全可直接运行）
### 🌟 基础规则
1. 所有动画的**时间单位默认是秒**
2. 未特殊指定时，动画**立即执行**
3. 所有动画方法都有返回值 `Tween`，可以接收这个对象做「链式配置」（下文重点讲）

---

### ✔ 1. 位置动画（最常用）
```csharp
// 1. DOMove 绝对位置移动：2秒内，移动到世界坐标 (5,0,0) 的位置
transform.DOMove(new Vector3(5, 0, 0), 2f);

// 2. DOMoveLocal 本地位置移动：基于父物体的本地坐标移动，适配UI/嵌套物体
transform.DOMoveLocal(new Vector3(0, 3, 0), 1.5f);

// 3. DOJump 跳跃动画：目标位置(5,0,0)，跳跃高度2，跳1次，总时长3秒
transform.DOJump(new Vector3(5, 0, 0), 2f, 1, 3f);
```

### ✔ 2. 旋转动画
```csharp
// 1. DORotate 绝对旋转：2秒内，旋转到欧拉角 (0,180,0)（Y轴旋转180度）
transform.DORotate(new Vector3(0, 180, 0), 2f);

// 2. DORotateQuaternion 四元数旋转：适合需要平滑无万向锁的旋转（推荐3D物体）
transform.DORotateQuaternion(Quaternion.Euler(0, 90, 0), 1.5f);

// 3. DOLocalRotate 本地旋转：基于父物体的本地坐标系旋转
transform.DOLocalRotate(new Vector3(90, 0, 0), 1f);

// 4. DORotateAround 绕轴旋转：绕世界坐标Y轴，旋转360度，时长2秒
transform.DORotateAround(Vector3.up, Mathf.PI * 2, 2f);
```

### ✔ 3. 缩放动画
```csharp
// DOScale 缩放：2秒内，从当前大小缩放到2倍大小
transform.DOScale(Vector3.one * 2, 2f);

// 局部缩放：UI常用，基于父物体的缩放比例
transform.DOScaleLocal(new Vector3(0.5f, 0.5f, 0.5f), 1f);
```

### ✔ 4. 透明/颜色动画（UGUI/精灵/材质通用）
#### ✅ UGUI 文本/图片 透明/颜色渐变
```csharp
using UnityEngine.UI;
public Image img; // 挂载图片组件
public Text text; // 挂载文本组件

void Start()
{
    // 1. DOFade 透明度动画：1秒内，图片从当前透明度变为完全透明(0)
    img.DOFade(0, 1f);
    // 文本渐显：1秒内从透明变为不透明(1)
    text.DOFade(1, 1f);

    // 2. DOColor 颜色渐变：1.5秒内，图片变为红色
    img.DOColor(Color.red, 1.5f);
}
```

#### ✅ 3D物体材质颜色渐变
```csharp
public Material mat; // 挂载物体的材质

void Start()
{
    // 材质颜色渐变：2秒内变为蓝色
    mat.DOColor(Color.blue, 2f);
    // 材质透明度渐变（材质必须是Fade模式）
    mat.DOFade(0.5f, 1f);
}
```

### ✔ 5. 静态方式 To 动画（万能自定义动画，重点）
所有 **快捷方式实现不了的动画**，都用这个方法！核心是「告诉DOTween：要读取哪个值、要赋值给哪个值、目标值是多少、动画多久」
#### 示例1：自定义数值动画（血量条、音量条、进度条）
```csharp
public float hp = 100; // 当前血量
public Slider hpSlider; // 血条组件

void Start()
{
    // 需求：3秒内，让hp从100平滑降到0，同时同步血条进度
    DOTween.To(
        () => hp,          // 取值委托：告诉DOTween，要读取的「当前值」是hp
        val => hp = val,   // 赋值委托：DOTween每帧计算出的新值，赋值给hp
        0,                 // 目标值：最终要到达的值
        3f                 // 动画时长
    ).OnUpdate(()=>{
        hpSlider.value = hp / 100; // 每帧同步血条
    });
}
```

#### 示例2：自定义Vector2动画（UI锚点、RectTransform位置）
```csharp
public RectTransform rect; // UI的RectTransform组件

void Start()
{
    // 2秒内，让UI的锚点从当前值变为(0.5,0.5)
    DOTween.To(()=>rect.anchorMin, val=>rect.anchorMin=val, new Vector2(0.5f,0.5f), 2f);
}
```

---

## ✅ 四、DOTween 核心精髓 - 链式调用（必学，重中之重）
### ✅ 核心说明
DOTween 所有动画方法的返回值都是 `Tween` 类型，这个对象可以通过 **`.` 连续调用配置方法**，一行代码完成「动画+所有配置」，这就是链式调用，**DOTween的灵魂用法**，没有之一！
> 特点：无先后顺序、可无限链式拼接、代码极简、逻辑清晰

### ✅ 常用链式配置方法（高频必备，全部要记）
| 方法名 | 作用 | 示例 |
| ---- | ---- | ---- |
| `.SetDelay(时间)` | 延迟执行动画 | `.SetDelay(1f)` 延迟1秒再播放 |
| `.SetLoops(次数, 循环类型)` | 设置循环播放 | `.SetLoops(-1)` 无限循环；`.SetLoops(3, LoopType.Yoyo)` 循环3次，来回动画 |
| `.SetEase(曲线类型)` | 设置动画缓动曲线（重中之重） | `.SetEase(Ease.OutBounce)` 回弹缓动，落地有弹跳效果 |
| `.OnComplete(回调函数)` | 动画播放完成后执行的方法 | `.OnComplete(PlayFinish)` 播放完执行PlayFinish方法 |
| `.OnStart(回调函数)` | 动画开始播放时执行的方法 | `.OnStart(PlayStart)` 开始播放时执行 |
| `.OnUpdate(回调函数)` | 动画播放中，**每帧执行**的方法 | `.OnUpdate(()=>{Debug.Log("动画播放中");})` |
| `.SetSpeedBased()` | 改为「速度模式」，时长参数变为「移动速度」 | `.SetSpeedBased()` 物体每秒移动5单位，而非2秒移动5单位 |
| `.SetRelative()` | 改为「相对值动画」，基于当前状态偏移 | `.DOMove(Vector3.right*5,2).SetRelative()` 向右移动5单位，而非移动到5的位置 |
| `.Pause()` | 暂停动画 | `.Pause()` 动画创建后先暂停，不立即播放 |
| `.Kill()` | 销毁动画，释放内存 | `.Kill()` 结束并销毁当前动画 |

### ✅ 链式调用 经典综合示例（必看，可直接复制）
```csharp
void Start()
{
    // 一行代码实现：物体向右移动5单位（相对值）+ 延迟1秒 + 无限循环来回动 + 回弹缓动 + 完成一次动画打印日志
    transform.DOMove(Vector3.right * 5, 2f)
        .SetRelative()       // 相对移动，不是绝对位置
        .SetDelay(1f)        // 延迟1秒执行
        .SetLoops(-1, LoopType.Yoyo) // 无限循环，Yoyo=来回动画（去+回）
        .SetEase(Ease.OutBounce) // 缓动曲线：移动到终点有弹跳效果
        .OnComplete(()=>{ Debug.Log("单次动画播放完成"); })
        .OnStart(()=>{ Debug.Log("动画开始播放了"); });
}
```

---

## ✅ 五、DOTween 常用缓动曲线（Ease）说明
动画的「生硬/顺滑/真实感」全靠缓动曲线，`SetEase()` 是必配项，以下是 **高频实用的曲线类型**，直接记：
1. **匀速运动**：`Ease.Linear` - 无加速减速，适合机械移动、子弹飞行
2. **先快后慢**：`Ease.OutQuad` / `Ease.OutCubic` - 最常用，平滑减速，适合绝大多数位移/缩放动画，视觉舒适
3. **回弹效果**：`Ease.OutBounce` - 到达目标点后会弹跳几下，适合跳跃、落地、弹性缩放
4. **弹性效果**：`Ease.OutElastic` - 超过目标点再回弹，适合弹簧、弹性旋转
5. **先慢后快**：`Ease.InQuad` - 适合加速冲刺、下落动画
6. **先慢后快再慢**：`Ease.InOutQuad` - 适合往返动画、平滑过渡

---

## ✅ 六、DOTween 全局控制（快速控制所有动画）
直接通过DOTween静态类控制场景中**所有**的动画，适合全局暂停/继续/重启等需求：
```csharp
// 暂停所有动画
DOTween.PauseAll();

// 继续播放所有暂停的动画
DOTween.PlayAll();

// 暂停当前脚本中的所有动画
DOTween.Pause(this);

// 重启所有动画
DOTween.RestartAll();

// 销毁所有动画，释放内存（场景切换必写，防止内存泄漏）
DOTween.KillAll();
```

---

## ✅ 七、DOTween 常用拓展示例（UI专项+实用功能）
### 1. UGUI 滑动框/面板移动（RectTransform专用）
```csharp
public RectTransform panel; // UI面板的RectTransform

void OpenPanel()
{
    // 面板从左侧滑入：0.5秒内移动到目标位置，缓动顺滑
    panel.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutQuad);
}

void ClosePanel()
{
    // 面板滑出到左侧
    panel.DOAnchorPos(new Vector2(-800, 0), 0.5f).SetEase(Ease.InQuad);
}
```

### 2. 文本打字机效果（UGUI Text/TMP通用）
```csharp
using TMPro;
public Text text; // 普通Text
public TextMeshProUGUI tmpText; // TMPro文本

void Start()
{
    // DOText 打字机效果：3秒内，逐字显示文本内容
    text.DOText("DOTween打字机效果演示", 3f);
    tmpText.DOText("TMP文本也支持同样的方法", 2.5f);
}
```

### 3. 颜色渐变+透明渐变组合动画
```csharp
public Image img;
void Start()
{
    // 同时执行：1.5秒变红色 + 2秒变透明，链式配置无限循环
    img.DOColor(Color.red, 1.5f)
        .SetLoops(-1, LoopType.Yoyo);
    img.DOFade(0.3f, 2f)
        .SetLoops(-1, LoopType.Yoyo);
}
```

---

## ✅ 八、DOTween 避坑小技巧（新手必看）
1. **内存泄漏**：场景切换时，一定要调用 `DOTween.KillAll()` 销毁所有动画，否则残留的动画会占用内存
2. **重复创建动画**：如果同一个物体频繁调用动画（比如按钮点击），建议先调用 `transform.DOKill();` 销毁旧动画，再创建新动画，防止动画叠加
3. **UI动画用Local**：UI物体都是嵌套的，位置/旋转/缩放优先用 `DOxxxLocal` 方法，避免坐标错乱
4. **材质透明无效**：材质必须设置为 `Rendering Mode = Fade` 模式，`DOFade` 才会生效

---

## ✅ 总结（核心知识点速记）
1. 必写：`using DG.Tweening;` 命名空间，否则所有语法失效
2. 2种核心调用：**快捷方式**（`对象.DOxxx()`，90%场景用）+ **静态To方式**（`DOTween.To()`，自定义属性万能）
3. 灵魂用法：**链式调用**，一行代码拼接所有动画配置，无先后顺序
4. 高频配置：`SetDelay`(延迟)、`SetLoops`(循环)、`SetEase`(缓动)、`OnComplete`(完成回调) 是四大必配项
5. 缓动曲线：`Ease.OutQuad`(通用顺滑)、`Ease.OutBounce`(回弹)、`Ease.Linear`(匀速) 够用99%的需求

所有示例代码均可直接复制运行，建议先跑通基础示例，再逐步尝试链式调用和自定义动画，DOTween上手极快，掌握这些内容足以应对Unity开发中95%的动画需求！🚀