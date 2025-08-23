# BKEngine

BKEngine 工具套件

## 功能
---
* BKEngine 引擎解包
* BKEngine 封包Hash文件名Dump提取

## 使用
---

## BKEngineExtract
### 描述
* BKEngine 引擎解包
### 使用
* 按照GUI界面指引
### 游戏支持
&emsp;&emsp;V2.0<br>
&emsp;&emsp;&emsp;官网封包版本<br>
&emsp;&emsp;V2.1(V2.0 HashVer)<br>
&emsp;&emsp;&emsp;《十二色季节》<br>
&emsp;&emsp;&emsp;《灭魂·误佳期》<br>
&emsp;&emsp;V4.0<br>
&emsp;&emsp;&emsp;《遥望彼方》<br>
&emsp;&emsp;&emsp;《五等分的抢婚 三玖篇》<br>
&emsp;&emsp;&emsp;《某一种青春》<br>
&emsp;&emsp;&emsp;《他人世界末》<br>
&emsp;&emsp;&emsp;《局外人 - L'Etranger》<br>
### 编译
&emsp;&emsp;依赖库<br>
&emsp;&emsp;&emsp;\[Nuget\] ICSharpCode.SharpZipLib<br>
&emsp;&emsp;&emsp;\[Nuget\] Zstd.Net<br>
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;.Net 6.x<br>

---

## BKEFileNameDumper
### 描述
* 封包Hash文件名Dump提取
### 使用
* 启动<br>
&emsp;将`Loader.exe`与`BKEFileNameDumper.dll`置于游戏目录<br>
&emsp;拖拽游戏Exe到`Loader.exe`上启动<br>
* 输出<br>
&emsp;`游戏目录\FileName.lst`<br>
&emsp;&emsp;游戏封包文件列表<br>
### 游戏支持
&emsp;&emsp;V2.1(V2.0 HashVer)<br>
&emsp;&emsp;&emsp;《十二色季节》<br>
### 编译
&emsp;&emsp;依赖库<br>
&emsp;&emsp;&emsp;[Detours](https://github.com/microsoft/Detours)<br>
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;MSVC2022 x86<br>

---











