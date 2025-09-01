# SyawaseWorks

SyawaseWorks 官中代理工具套件

## 功能
---
* 过`Steam`检测
* 定制 `Artemis .pfs` 封包解密提取
* 定制 `Qile .pack` 封包解密提取 

## 保护
---
* Themida 3.x 加壳

## 使用
---

## HamidashiCreative
### 描述
* 游戏破解
* 定制 `Artemis .pfs` 封包解密提取
### 使用
* `HamidashiPatch`<br>
&emsp;启动<br>
&emsp;&emsp;将`HamidashiPatch.dll`与`SteamPatch`补丁放置于游戏目录<br>
&emsp;&emsp;使用注入器注入`HamidashiPatch.dll`启动游戏<br>
&emsp;输出<br>
&emsp;&emsp;`HamidashiCreative.log`<br>
&emsp;&emsp;&emsp;补丁日志<br>
&emsp;&emsp;`Dumper_Output`文件夹 (`EnableDumper`宏启用)<br>
&emsp;&emsp;&emsp;运行时资源Dump<br>
* `HamidashiCreativeStatic`<br>
&emsp;按照GUI界面指引<br>
### 游戏支持
&emsp;《ハミダシクリエイティブ》 国际中文版<br>
### 编译
* `HamidashiPatch`<br>
&emsp;依赖库<br>
&emsp;&emsp;[Detours](https://github.com/microsoft/Detours)<br>
&emsp;编译器<br>
&emsp;&emsp;MSVC2022 x86<br>
* `HamidashiCreativeStatic`<br>
&emsp;编译器<br>
&emsp;&emsp;.Net 6.x<br>

---

## HappyLiveShowUp
### 描述
* 游戏破解
* 定制 `Qile .pack` 封包解密提取 
### 使用
* `Patch`<br>
&emsp;启动<br>
&emsp;&emsp;将`Patch.dll`与`SteamPatch`补丁放置于游戏目录<br>
&emsp;&emsp;使用注入器注入`Patch.dll`启动游戏<br>
* `HappyLiveShowUpStatic`<br>
&emsp;按照GUI界面指引<br>
&emsp;解密完成`dec_.pack`包使用`GARBro`提取<br>
### 游戏支持
&emsp;《ハッピーライヴ ショウアップ》 国际中文版<br>
### 编译
* `Patch`<br>
&emsp;依赖库<br>
&emsp;&emsp;[Detours](https://github.com/microsoft/Detours)<br>
&emsp;编译器<br>
&emsp;&emsp;MSVC2022 x86<br>
* `HappyLiveShowUpStatic`<br>
&emsp;依赖库<br>
&emsp;&emsp;\[Nuget\] System.IO.Hashing<br>
&emsp;编译器<br>
&emsp;&emsp;.Net 6.x<br>

---
