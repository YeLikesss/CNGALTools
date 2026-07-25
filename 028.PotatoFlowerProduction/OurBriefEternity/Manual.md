# OurBriefEternity

土豆花制作组<br>
《永恒与星辰与日常》工具套件<br>

## 功能
---
* 游戏破解
* 游戏解包

## 说明
* 游戏厂商于2026-7-22移除`VMP`与`SteamStub`
* 本项目仅适配VMP加壳版

## 使用
---

## Patch
### 描述
* 游戏破解
### 保护
* VMProtect 3.x 加壳
### 使用
&emsp;&emsp;1. 使用`GoldbergSteamEmu`替换steam_api64.dll<br>
&emsp;&emsp;2. 配置`GoldbergSteamEmu`启动参数<br>
&emsp;&emsp;3. 使用`Steamless`移除主程序Exe的DRM<br>
&emsp;&emsp;4. 原Exe改名为`OurBriefEternity.bak`<br>
&emsp;&emsp;5. `Steamless`输出Exe改名为`OurBriefEternity.exe`<br>
&emsp;&emsp;6. 将编译好的`Patch.dll`放置于游戏目录<br>
&emsp;&emsp;7. 使用`CFF Explorer VIII`打开Exe, 添加`Patch.dll`导出函数到导入表, 保存覆盖<br>
&emsp;&emsp;8. 双击Exe启动游戏<br>
### 编译
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;MSVC 2022 x64<br>

## Extractor
### 描述
* 游戏解包
### 使用
* 按照GUI界面指引
### 编译
&emsp;&emsp;依赖库<br>
&emsp;&emsp;&emsp;\[Nuget\] Razorvine.Pickle<br>
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;.Net 6.x<br>

---

## Windows 7 支持
### 说明
* 原游戏加壳VMP, 无法通过补丁在老旧系统运行
### 简要操作
&emsp;&emsp;1. 解包所有游戏资源<br>
&emsp;&emsp;2. 下载原版`Renpy 8.6.0`, 自行按照Renpy教程放置游戏资源<br>
&emsp;&emsp;3. 使用`Vxkex`运行Exe启用游戏<br>