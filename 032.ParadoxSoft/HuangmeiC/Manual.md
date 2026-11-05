# HuangmeiC

悖论软件<br>
《黄莓C》工具套件<br>

## 功能
---
* 游戏破解

## 使用
---

## Patch
### 描述
* 游戏破解
### 使用
&emsp;&emsp;1. 使用`GoldbergSteamEmu`替换steam_api64.dll<br>
&emsp;&emsp;2. 配置`GoldbergSteamEmu`启动参数<br>
&emsp;&emsp;3. 使用`Steamless`移除主程序Exe的DRM<br>
&emsp;&emsp;4. 原Exe改名为`HuangmeiC.bak`<br>
&emsp;&emsp;5. `Steamless`输出Exe改名为`HuangmeiC.exe`<br>
&emsp;&emsp;6. 将编译好的`Patch.dll`放置于游戏目录<br>
&emsp;&emsp;7. 使用`CFF Explorer VIII`打开Exe, 添加`Patch.dll`导出函数到导入表, 保存覆盖<br>
&emsp;&emsp;8. 双击Exe启动游戏<br>
### 编译
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;MSVC 2022 x64<br>

---

## Windows 7 支持
### 说明
* 破解后使用`Vxkex`运行Exe启动游戏
