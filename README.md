# CNGALTools

国产/官中代理Galgame工具套件

Chinese Galgame Reverse Engineering Project

---

## 附录
* [游戏分析笔记](https://github.com/YeLikesss/CNGALReverseNote)
* 第三方工具与源码<br>
&emsp;[AssetStudio (Unity解包)](https://github.com/Perfare/AssetStudio)<br>
&emsp;[CNStudio (Unity中国版解包)](https://github.com/RazTools/Studio)<br>
&emsp;[GARbro (Galgame解包)](https://github.com/crskycode/GARbro)<br>
&emsp;[Detect It Easy (查壳工具)](https://github.com/horsicq/Detect-It-Easy)<br>
&emsp;[demoleition (molebox脱壳提取工具)](https://lifeinhex.com/category/tools/)<br>
&emsp;[Renpy (Galgame引擎)](https://github.com/renpy/renpy)<br>
&emsp;[dnSpy (.Net反编译器)](https://github.com/dnSpyEx/dnSpy)<br>
&emsp;[uncompyle6 (Python反编译器)](https://github.com/rocky/python-uncompyle6)<br>
&emsp;[debugview (Windows调试信息)](https://learn.microsoft.com/zh-cn/sysinternals/downloads/debugview)<br>
&emsp;[texturepacker](https://www.codeandweb.com/texturepacker)<br>

---

## 工具支持
### 001. NVL (Navila Software Japan)
* [BKEngine](./001.NVL/BKEngine/Manual.md)
* [NVLKRKR](./001.NVL/NVLKrkr2/Manual.md)
* [NVLUnity](./001.NVL/NVLUnity/Manual.md)
* [NVLWeb](./001.NVL/NVLWeb/Manual.md)
### 002.Strrationalism (弦语蝶梦)
* [Snowing](./002.Strrationalism/Snowing/Manual.md)
### 003.BlueAngel (蓝天使)
* [蓝天使制作组](./003.BlueAngel/Manual.md)
### 004.Fontainebleau (枫丹白露)
* [枫丹白露制作组](./004.Fontainebleau/Manual.md)
### 005.ZixSolution
* [Renpy引擎加密定制](./005.ZixSolution/Manual.md)
### 006.iFAction
* [iFAction引擎](./006.iFAction/iFActionTool/Manual.md)
### 007.AsicxArt (芯片社)
* [芯片社制作组](./007.AsicxArt/Manual.md)
### 008.XinYvanGames (心愿游戏)
* [心愿游戏制作组](./008.XinYvanGames/Manual.md)
### 009.SoraPlayer

* 修改了Sign的Krkr2封包  (未加密)

    &emsp;游戏测试

    &emsp;&emsp;《夏花的轨迹 That Summer Of Eternal Eden》

    &emsp;&emsp;《锈翅 逃离我的家乡》

    &emsp;&emsp;《夏雪花染》

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 010.UniversalXP3DecFilter

* 通用XP3加密接口 XP3ArchiveExtractionFilter

    &emsp;游戏测试

    &emsp;&emsp;部分游戏使用molebox打包 请先使用[demoleition](https://lifeinhex.com/category/tools/)解包

    &emsp;&emsp;《彼岸花葬》

    &emsp;&emsp;《翡翠月》

    &emsp;&emsp;《雪之本境》 光盘版(2010)

    &emsp;&emsp;《雪之本境解境篇》 光盘版

    &emsp;&emsp;《雪之本境EX》 光盘版

    &emsp;&emsp;《雾之本境》 光盘版

    &emsp;&emsp;《雨港基隆》 Steam

    &emsp;&emsp;《鸑鷟 镜花水月》

    &emsp;&emsp;《鸑鷟 橘子传》

    &emsp;&emsp;《叶之离别:若叶归尘》

    &emsp;&emsp;《雨夜》

    &emsp;&emsp;《吉祥铃》

    &emsp;&emsp;《水滴里的夏天》

    &emsp;&emsp;《宛若朝阳》

    &emsp;&emsp;《遗忘花园》 光盘版

    &emsp;&emsp;《紫罗兰-里: 水中倒影》

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 011.Irregulars

* Irregulars Engine

    &emsp;游戏测试

    &emsp;&emsp;《MOBIUS BAND*》

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 012.Visual Novel Maker

* XorFilter

    &emsp;游戏测试

    &emsp;&emsp;《执谕者：坠月之兆》

    &emsp;&emsp;《爱与命的彼端》

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

    &emsp;注意

    &emsp;&emsp;1. 不是所有资源都进行了加密, 资源是否加密需要自行判断

### 013.Game Creator

* V1版本加密 (图像/文本/音频)

    &emsp;游戏测试

    &emsp;&emsp;《令和罕见物语》

    &emsp;&emsp;《我亲爱的妹妹》

    &emsp;&emsp;《鼓手余命十日谭》

    &emsp;&emsp;《叛军组织的我爱上了贵族大小姐》

    &emsp;&emsp;《风之歌》

    &emsp;&emsp;《在时间的尽头等你》

    &emsp;&emsp;《残神觉醒》

    &emsp;&emsp;《暮雨流花+》

    &emsp;依赖库

    &emsp;&emsp;NuGet

    &emsp;&emsp;&emsp;ICSharpCode.SharpZipLib

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 014.橙汁工作室

*  Lover (情人节:不见不散)

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 015.SeparateHearts

*   SeparateHearts Engine(古月引擎)(Hac解包/Tex解码/Hgp解码/HTP+HTL解码)

    &emsp;游戏测试

    &emsp;&emsp;《夏梦夜话》

    &emsp;&emsp;《红楼梦》

    &emsp;&emsp;《红楼梦: 林黛玉与北静王》

    &emsp;&emsp;《楼兰: 轮回之轨迹》

    &emsp;&emsp;《楼兰: 轮回之轨迹 缘》

    &emsp;依赖库

    &emsp;&emsp;Nuget

    &emsp;&emsp;&emsp;ICSharpCode.SharpZipLib

    &emsp;&emsp;&emsp;LZMA-SDK

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 016.NekoNovel

*   NekoNovel Package

    &emsp;游戏测试

    &emsp;&emsp;《Lucy -The Eternity She Wished For-》

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 017.Ourshow Games

*   AGP封包

    &emsp;游戏测试

    &emsp;&emsp;《李雷和韩梅梅:与你同在》

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 018.焦糖摩卡组/CaramelMochaStudio

*   《仿徨之街》辅助(TheStreetOfAdriftToolkit)

    &emsp;功能

    &emsp;&emsp;连线关卡选项变得非常简单

    &emsp;使用方法

    &emsp;&emsp;将`TSOALoader.exe`与`TSOACheat.dll`置于游戏目录下
    
    &emsp;&emsp;双击启动`TSOALoader.exe`运行即可

    &emsp;依赖库

    &emsp;&emsp;[Detours](https://github.com/microsoft/Detours)

    &emsp;编译环境

    &emsp;&emsp;MSVC2022 x64

### 019.PygmaGame

* 修改版Renpy8 RPA封包(V1)
    
    &emsp;&emsp;PygmaGame
    
    &emsp;&emsp;零点世界工作室

    &emsp;游戏测试

    &emsp;&emsp;《苍空的彼端》(PygmaGame)

    &emsp;&emsp;《愚者之梦: 零时将至》(零点世界工作室)

    &emsp;依赖库
    
    &emsp;&emsp;NuGet
    
    &emsp;&emsp;&emsp;Razorvine.Pickle

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 020.Xso

*  Xso制作组 修改版Renpy8 RPA封包(V1)

    &emsp;游戏测试

    &emsp;&emsp;《不恋爱就完蛋了》

    &emsp;依赖库
    
    &emsp;&emsp;NuGet
    
    &emsp;&emsp;&emsp;Razorvine.Pickle

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 021.UniversalRPAExtractor

*  Renpy官方封包提取 (较多国Gal使用该引擎)
    
    &emsp;[官方源码](https://github.com/renpy/renpy/blob/master/renpy/loader.py)

    &emsp;依赖库
    
    &emsp;&emsp;NuGet
    
    &emsp;&emsp;&emsp;Razorvine.Pickle

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 022.箱社

*  IceCreamStick AVGEngine(V1)(Unity)

    &emsp;游戏测试

    &emsp;&emsp;《大科学家》

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 995.Chatte Noire

* Nie no Hakoniwa(贄の匣庭)

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 996.LightVN

* LightVN Engine

    &emsp;游戏测试

    &emsp;&emsp;《U-ena 空焰火少女》

    &emsp;&emsp;《プトリカ 1st.cut》

    &emsp;编译环境

    &emsp;&emsp;.Net 7.x

### 997.SyawaseWorks (官中发行商)

* HamidashiCreative (定制加密+Themida 3.x) (Cracker/Dumper/Extractor)

    &emsp;游戏测试

    &emsp;&emsp;《ハミダシクリエイティブ》 Chs Release

    &emsp;依赖库

    &emsp;&emsp;[Detours](https://github.com/microsoft/Detours)

    &emsp;编译环境 
    
    &emsp;&emsp;MSVC2022 x86 (破解/Dump)

    &emsp;&emsp;.Net 7.x (静态解包)

* HappyLiveShowUp (定制加密+Themida 3.x) (Cracker/Decryptor)

    &emsp;游戏测试

    &emsp;&emsp;《ハッピーライヴ ショウアップ！》 Chs Release

    &emsp;依赖库

    &emsp;&emsp;[Detours](https://github.com/microsoft/Detours)

    &emsp;&emsp;Nuget

    &emsp;&emsp;&emsp;System.IO.Hashing

    &emsp;编译环境
    
    &emsp;&emsp;MSVC2022 x86 (破解)

    &emsp;&emsp;.Net6.x (静态解密)

    &emsp;注意

    &emsp;&emsp;使用封包解密工具后, 保持目录不变使用[GARbro](https://github.com/crskycode/GARbro)打开dec_xxx.pack封包

### 998.HikariField (官中发行商)

* HFUnityV1 (HFUnityV1) (Extractor)

    &emsp;游戏测试

    &emsp;&emsp;《アオナツライン》 Chs-Cht Release

    &emsp;&emsp;《Making Lovers FHD》 Chs-ChtRelease

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

* FutureRadio (CatSystem2 Unity) (Extractor)

    &emsp;游戏测试

    &emsp;&emsp;《未来ラジオと人工鳩》 Chs-Cht-Jpn-Eng Release

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

* NekoNyan (NekoNyan Unity) (Extractor)

    &emsp;游戏测试

    &emsp;&emsp;《蒼の彼方のフォーリズム PE/EXTRA1/EXTRA2》 Chs-Cht-Jpn-Eng Release

    &emsp;&emsp;《金色ラブリッチェ》 Chs-Cht-Jpn-Eng Release

    &emsp;&emsp;《Clover Day's Plus》 Chs-Cht-Jpn-Eng Release

    &emsp;&emsp;《恋と選挙とチョコレート》 Chs-Cht-Jpn-Eng Release

    &emsp;编译环境

    &emsp;&emsp;.Net 6.x

### 999.Others

