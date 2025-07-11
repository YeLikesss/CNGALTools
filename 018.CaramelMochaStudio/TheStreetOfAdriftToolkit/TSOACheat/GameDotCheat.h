#pragma once

#include "Il2Cpp.h"
namespace GameDotCheat
{
	class GameDotNode;
	class GameDotNodeLayer;

	/// <summary>
	/// 连线点类型
	/// </summary>
	enum GameDotType
	{
		/// <summary>
		/// 普通
		/// </summary>
		Default,
		/// <summary>
		/// 起点
		/// </summary>
		Start,
		/// <summary>
		/// 终点
		/// </summary>
		End,
	};

	/// <summary>
	/// 连线点
	/// </summary>
	class GameDotNode : public Il2CppObject
	{
	public:
		GameDotNode() = delete;
		GameDotNode(const GameDotNode&) = delete;
		GameDotNode(GameDotNode&&) = delete;
		GameDotNode& operator=(const GameDotNode&) = delete;
		GameDotNode& operator=(GameDotNode&&) = delete;
		~GameDotNode() = delete;
	private:
		void* MonoBehaviour;				//0x10
	public:
		/// <summary>
		/// 绑定的关卡
		/// </summary>
		GameDotNodeLayer* NodeLayer;		//0x18

		void* ConnectFill;					//0x20
		void* SelectFill;					//0x28
		void* TintFill;						//0x30
		void* QuestionText;					//0x38
		float TintColor[4];					//0x40
		float StartTintColor[4];			//0x50

		/// <summary>
		/// 点类型
		/// </summary>
		GameDotType Type;					//0x60
		/// <summary>
		/// 选项标签
		/// </summary>
		Il2CppString* GotoLabel;			//0x68
		/// <summary>
		/// 添加变量
		/// </summary>
		Il2CppString* AddFlag;				//0x70
		/// <summary>
		/// 解锁变量
		/// </summary>
		Il2CppString* UnlockFlag;			//0x78
		/// <summary>
		/// 开启变量
		/// </summary>
		Il2CppString* EnableFlag;			//0x80
		/// <summary>
		/// 关闭变量
		/// </summary>
		Il2CppString* DisableFlag;			//0x88
		/// <summary>
		/// 提示字符串
		/// </summary>
		Il2CppString* EmitText;				//0x90
		/// <summary>
		/// 当前点所连接的其他点
		/// </summary>
		Il2CppExtend::Il2CppArrayT<GameDotNode*>* Connectable;			//0x98
		/// <summary>
		/// 是否已连接
		/// </summary>
		bool Connected;						//0xA0

	public:
		/// <summary>
		/// 获取类
		/// </summary>
		static Il2CppClass* Class();
		/// <summary>
		/// 新建对象
		/// <para>[C#]new GameDotNode()</para>
		/// </summary>
		static Il2CppObject* New();
	};

	/// <summary>
	/// 连线关卡
	/// </summary>
	class GameDotNodeLayer : public Il2CppObject
	{
	public:
		GameDotNodeLayer() = delete;
		GameDotNodeLayer(const GameDotNodeLayer&) = delete;
		GameDotNodeLayer(GameDotNodeLayer&&) = delete;
		GameDotNodeLayer& operator=(const GameDotNodeLayer&) = delete;
		GameDotNodeLayer& operator=(GameDotNodeLayer&&) = delete;
		~GameDotNodeLayer() = delete;
	private:
		void* MonoBehaviour;				//0x10
	public:
		/// <summary>
		/// 点位数组
		/// </summary>
		Il2CppExtend::Il2CppArrayT<GameDotNode*>* Nodes;				//0x18

		void* NodePrefab;					//0x20

		/// <summary>
		/// 当前关卡
		/// </summary>
		Il2CppString* LevelName;			//0x28
		/// <summary>
		/// 默认选择
		/// </summary>
		Il2CppString* LevelDefaultGoto;		//0x30
		/// <summary>
		/// 限时时间(s)
		/// </summary>
		float LevelLimitDuration;			//0x38

		void* rectTransform;				//0x40
		void* DotConnect;					//0x48

		/// <summary>
		/// 当前选择点
		/// </summary>
		GameDotNode* CurrentSelect;			//0x50

		/// <summary>
		/// 是否拖动中
		/// </summary>
		bool onDrag;						//0x58

	public:
		/// <summary>
		/// 获取类结构
		/// </summary>
		static Il2CppClass* Class();
		/// <summary>
		/// 新建对象
		/// <para>[C#]new GameDotNodeLayer()</para>
		/// </summary>
		static Il2CppObject* New();
		/// <summary>
		/// 打印信息
		/// </summary>
		void Print();
		/// <summary>
		/// 补丁
		/// </summary>
		void Patch();
	};

	/// <summary>
	/// 连线UI
	/// </summary>
	class GameDotConnectUI : public Il2CppObject
	{
	public:
		GameDotConnectUI() = delete;
		GameDotConnectUI(const GameDotConnectUI&) = delete;
		GameDotConnectUI(GameDotConnectUI&&) = delete;
		GameDotConnectUI& operator=(const GameDotConnectUI&) = delete;
		GameDotConnectUI& operator=(GameDotConnectUI&&) = delete;
		~GameDotConnectUI() = delete;

	public:
		using tGameDotConnectUI_Awake = void(*)(GameDotConnectUI* obj, const MethodInfo* method);
	private:
		void* monoBehaviour;
		unsigned __int8 Naninovel_UI_CustomUI_Fields[0xA0];
	public:
		Il2CppObject* LevelRoot;
		/// <summary>
		/// 关卡数组
		/// </summary>
		Il2CppExtend::Il2CppArrayT<GameDotNodeLayer*>* Levels;
		void* LineLayer;
		void* PathLayer;
		void* LevelTitle;
		void* UIAnimator;
		/// <summary>
		/// 变量List
		/// </summary>
		void* Flags;
		void* TimerFill;
		float FillWidth;
		float FillHeight;
		float startTime;
		float duration;
		int32_t levelIndex;
		bool Running;
		void* ConnectedClip;
		void* PuzzleSolvedClip;
		void* Audio;
		void* PredictLine;
		void* TextManager;
		void* SkipButton;
		void* scriptPlayer;
		void* VariableManager;
		void* textManager;
		float LineWidth;
		float LineYOffset;
		bool nodeHold;
		GameDotNode* startNode;
		float lineDraggedPosition[3];

	private:
		Il2CppExtend::Il2CppArrayT<GameDotNodeLayer*>* GetLevels();

	public:
		/// <summary>
		/// 获取类结构
		/// </summary>
		static Il2CppClass* Class();
		/// <summary>
		/// 打印信息
		/// </summary>
		void Print();
		/// <summary>
		/// 补丁
		/// </summary>
		void Patch();
	};

}