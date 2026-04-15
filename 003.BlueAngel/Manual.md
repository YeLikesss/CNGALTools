# BlueAngel

蓝天使制作组工具套件

## 功能
---
* 封包资源解密

## 使用
---

## BlueAngelExtract
### 描述
* XP3封包解密
### 保护
* VMProtect 3.x 加壳
### 使用
* 对应游戏解包Exe的GUI界面指引
### 游戏支持
&emsp;《亿万年的星光》<br>
### 编译
&emsp;&emsp;依赖库<br>
&emsp;&emsp;&emsp;\[Nuget\] K4os.Compression.LZ4<br>
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;.Net 6.x<br>

---

## TheCardinalMemoryNotch
### 描述
* Packman VFS提取
* 解包SPK-XP3封包资源
### 保护
* Packman 加壳
### 使用
* `ProtectorFileDumper`<br>
&emsp;使用注入器注入到游戏中<br>
&emsp;`Debug输出`<br>
&emsp;&emsp;使用`debugview`查看输出<br>
* `SPKExtractor`<br>
&emsp;解包SPK-XP3封包资源
### 游戏支持
&emsp;《绯色的记忆之痕》 V2.01<br>
### 编译
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;`ProtectorFileDumper`<br>
&emsp;&emsp;&emsp;&emsp;MSVC2022 x86<br>
&emsp;&emsp;&emsp;`SPKExtractor`<br>
&emsp;&emsp;&emsp;&emsp;.Net 6.x<br>

---

## TheCardinalMemoryNotchV2
### 描述
* 解包SPK-XP3v2封包资源
### 保护
* Safengine V2.3.7.0 加壳
### 使用
* `KrkrFileDumper`<br>
&emsp;`KrkrFileDumperLoader`必需使用`Release`档编译<br>
&emsp;`KrkrFileDumperLoader.exe`与`KrkrFileDumper.dll`放置于游戏目录下<br>
&emsp;游戏exe拖拽到`KrkrFileDumperLoader.exe`运行<br>
&emsp;`Debug输出`<br>
&emsp;&emsp;使用`debugview`查看输出<br>
&emsp;`资源文件输出`<br>
&emsp;&emsp;游戏目录/File_Dumper<br>
### 游戏支持
&emsp;《绯色的记忆之痕 Notch Series Episode 2》<br>
### 编译
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;`KrkrFileDumper`<br>
&emsp;&emsp;&emsp;&emsp;MSVC2022 x86<br>

---
