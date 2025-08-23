# NVLKRKR

NVLKRKR 工具套件

## 功能
---
* NVLKRKR2 静态解包
* NVLKRKR2 动态Dump资源
* NVLKRKR2 封包Hash文件名Dump
* NVLKRKR2 封包Hash文件名解码器

## 使用
---

## NVLKR2Extract
### 描述
* NVLKRKR2 静态解包
### 使用
* 按照GUI界面指引
### 游戏支持
&emsp;《橘子班短篇合集》<br>
&emsp;《高考恋爱100天》<br>
&emsp;《虹色旋律》<br>
&emsp;《祈风》<br>
&emsp;《雾之本境S》<br>
&emsp;《真恋寄语枫秋》<br>
&emsp;《余香》<br>
&emsp;《茸雪》<br>
&emsp;《回忆忘却之匣》<br>
&emsp;《花落冬阳》<br>
&emsp;《雪之本境S》<br>
&emsp;《丑小鸭的天鹅湖》<br>
### 编译
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;.Net 6.x<br>

---

## NVLKrkrDump
### 描述
* NVLKRKR2 动态Dump资源
* NVLKRKR2 封包Hash文件名Dump
### 使用
* 启动<br>
&emsp;将`NVLKrkrDumpLoader.exe`与`NVLKrkrDump.dll`置于游戏目录下<br>
&emsp;拖拽游戏Exe到`NVLKrkrDumpLoader.exe`上启动<br>
* 输出<br>
&emsp;`Extract`<br>
&emsp;&emsp;运行时Dump封包资源文件夹<br>
&emsp;`NVLKrkrDump.log`<br>
&emsp;&emsp;运行日志<br>
&emsp;`FullPath.lst`<br>
&emsp;&emsp;封包资源全路径列表<br>
&emsp;`RelativePath.lst`<br>
&emsp;&emsp;封包资源相对路径列表<br>
&emsp;`AutoPath.lst`<br>
&emsp;&emsp;封包自动路径列表<br>
### 编译
&emsp;&emsp;依赖库<br>
&emsp;&emsp;&emsp;[Detours](https://github.com/microsoft/Detours)<br>
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;MSVC2022 x86<br>

---

## NVLKR2 Hash Decoder
### 描述
* NVLKRKR2 封包Hash文件名解码器
### 使用
* 操作<br>
&emsp;1."选择目标文件夹" -> 资源路径<br>
&emsp;2."加载自动路径" -> `AutoPath.lst`<br>
&emsp;3."加载Dump的文件名还原" -> `RelativePath.lst`<br>
&emsp;4."使用本地路径还原" -> 遍历本地路径还原<br>
&emsp;5."使用本地路径还原(AutoPath)" -> 遍历本地路径还原(添加AutoPath路径)<br>
&emsp;6.字符串生成器 -> 自己找规律生成爆破<br>
&emsp;7.该Hash算法碰撞率极高(不要一次性加载太多AutoPath)<br>
### 编译
&emsp;&emsp;编译器<br>
&emsp;&emsp;&emsp;.Net 6.x<br>

---


