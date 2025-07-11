#include "GameDotCheat.h"
#include "UnityEngine.h"
#include "System.h"
#include "util.h"
#include <vector>

namespace GameDotCheat
{
	//*****************GameDotNode*********************//
	Il2CppClass* GameDotNode::Class()
	{
		const Il2CppImage* image = Il2CppUtils::GetImageByName("Assembly-CSharp");
		return il2cpp_class_from_name(image, "", "GameDotNode");
	}
	Il2CppObject* GameDotNode::New()
	{
		return il2cpp_object_new(GameDotNode::Class());
	}
	//*===============================================*//

	//*****************GameDotNodeLayer*********************//
	Il2CppClass* GameDotNodeLayer::Class()
	{
		const Il2CppImage* image = Il2CppUtils::GetImageByName("Assembly-CSharp");
		return il2cpp_class_from_name(image, "", "GameDotNodeLayer");
	}
	Il2CppObject* GameDotNodeLayer::New()
	{
		return il2cpp_object_new(GameDotNodeLayer::Class());
	}
	void GameDotNodeLayer::Print()
	{
		Il2CppObject* gameObject = UnityEngine::Component::GetGameObject(this);
		std::wstring name = std::wstring();
		{
			Il2CppString* s = UnityEngine::Object::GetName(gameObject);
			name.append((const wchar_t*)(s->chars), s->length);
		}
		std::wstring title = std::wstring((const wchar_t*)(this->LevelName->chars), this->LevelName->length);
		std::wstring defaultLabel = std::wstring((const wchar_t*)(this->LevelDefaultGoto->chars), this->LevelDefaultGoto->length);

		Util::WriteDebugMessage(L"[%s] 标题: %s 默认分支: %s 限时: %.2f(s)", name.c_str(), title.c_str(), defaultLabel.c_str(), this->LevelLimitDuration);

		//获取点对象
		//GameDotNode[] dots = gameObject.GetComponentsInChildren(typeof(GameDotNode), true)
		Il2CppExtend::Il2CppArrayT<GameDotNode*>* dots = (Il2CppExtend::Il2CppArrayT<GameDotNode*>*)UnityEngine::GameObject::GetComponentsInChildren
														(gameObject, System::Object::GetType(GameDotNode::New()), true);
		for (size_t i = 0u; i < dots->GetCount(); ++i)
		{
			GameDotNode* dot = dots->GetItemsPointer()[i];

			std::wstring dotType = std::wstring();
			{
				switch (dot->Type)
				{
					case GameDotType::Default:
					{
						dotType = L"Default";
						break;
					}
					case GameDotType::Start:
					{
						dotType = L"Start";
						break;
					}
					case GameDotType::End:
					{
						dotType = L"End";
						break;
					}
				}
			}
			std::wstring gotoLabel = std::wstring((const wchar_t*)(dot->GotoLabel->chars), dot->GotoLabel->length);
			std::wstring addFlag = std::wstring((const wchar_t*)(dot->AddFlag->chars), dot->AddFlag->length);
			std::wstring unlockFlag = std::wstring((const wchar_t*)(dot->UnlockFlag->chars), dot->UnlockFlag->length);
			std::wstring enableFlag = std::wstring((const wchar_t*)(dot->EnableFlag->chars), dot->EnableFlag->length);
			std::wstring disableFlag = std::wstring((const wchar_t*)(dot->DisableFlag->chars), dot->DisableFlag->length);
			std::wstring emitText = std::wstring((const wchar_t*)(dot->EmitText->chars), dot->EmitText->length);

			Util::WriteDebugMessage(L"[%03d] 类型: %s 分支: %s 添加变量: %s 解锁变量: %s 启用变量: %s 停用变量: %s 提示信息: %s",
									i, dotType.c_str(), gotoLabel.c_str(), 
									addFlag.c_str(), unlockFlag.c_str(), enableFlag.c_str(), disableFlag.c_str(), 
									emitText.c_str());
		}
	}
	void GameDotNodeLayer::Patch()
	{
		//获取点对象
		//GameDotNode[] dots = gameObject.GetComponentsInChildren(typeof(GameDotNode), true)
		Il2CppObject* gameObject = UnityEngine::Component::GetGameObject(this);
		Il2CppExtend::Il2CppArrayT<GameDotNode*>* dots = (Il2CppExtend::Il2CppArrayT<GameDotNode*>*)UnityEngine::GameObject::GetComponentsInChildren
														(gameObject, System::Object::GetType(GameDotNode::New()), true);
		//修改限时(s)
		this->LevelLimitDuration = 1000000.0f;

		//过滤点位
		GameDotNode* startNode = nullptr;
		std::vector<GameDotNode*> endNode = std::vector<GameDotNode*>();
		std::vector<GameDotNode*> validNode = std::vector<GameDotNode*>();
		std::vector<GameDotNode*> junkNode = std::vector<GameDotNode*>();

		for (size_t i = 0u; i < dots->GetCount(); ++i)
		{
			GameDotNode* dot = dots->GetItemsPointer()[i];

			switch (dot->Type)
			{
				case GameDotType::Default:
				{
					if (System::String::IsEmpty(dot->GotoLabel) &&
						System::String::IsEmpty(dot->AddFlag) &&
						System::String::IsEmpty(dot->UnlockFlag) &&
						System::String::IsEmpty(dot->EnableFlag) &&
						System::String::IsEmpty(dot->DisableFlag) &&
						System::String::IsEmpty(dot->EmitText))
					{
						//所有变量字段为空  无效点
						junkNode.push_back(dot);
					}
					else
					{
						bool unrepeat = true;
						for(const GameDotNode* vn : validNode)
						{
							if (System::String::Equals(vn->GotoLabel, dot->GotoLabel) &&
								System::String::Equals(vn->AddFlag, dot->AddFlag) &&
								System::String::Equals(vn->UnlockFlag, dot->UnlockFlag) &&
								System::String::Equals(vn->EnableFlag, dot->EnableFlag) &&
								System::String::Equals(vn->DisableFlag, dot->DisableFlag) &&
								System::String::Equals(vn->EmitText, dot->EmitText))
							{
								//检查到重复点
								unrepeat = false;
								break;
							}
						}

						if (unrepeat)
						{
							//存在变量字段保留(不重复)
							validNode.push_back(dot);
						}
						else
						{
							//重复点无效
							junkNode.push_back(dot);
						}
					}
					break;
				}
				case GameDotType::Start:
				{
					//起点
					startNode = dot;
					break;
				}
				case GameDotType::End:
				{
					//终点
					endNode.push_back(dot);
					break;
				}
			}
		}

		if (startNode)
		{
			Il2CppClass* cls = GameDotNode::Class();

			FieldInfo* connectableField = il2cpp_class_get_field_from_name(cls, "Connectable");

			//终点个数
			size_t destCount = endNode.size() + validNode.size();

			//创建新的连接点位
			Il2CppExtend::Il2CppArrayT<GameDotNode*>* emptyConnectable = (Il2CppExtend::Il2CppArrayT<GameDotNode*>*)il2cpp_array_new(cls, 0u);
			Il2CppExtend::Il2CppArrayT<GameDotNode*>* startConnectable = (Il2CppExtend::Il2CppArrayT<GameDotNode*>*)il2cpp_array_new(cls, destCount);

			//无效点脱离
			for (GameDotNode* jn : junkNode)
			{
				//无效点清空链接
				il2cpp_field_set_value_object(jn, connectableField, (Il2CppObject*)emptyConnectable);

				Il2CppObject* jnTrans = UnityEngine::Component::GetTransform(jn);
				UnityEngine::Transform::SetParent(jnTrans, nullptr);
			}

			//起始点链接到终点
			il2cpp_field_set_value_object(startNode, connectableField, (Il2CppObject*)startConnectable);

			GameDotNode** items = startConnectable->GetItemsPointer();
			for (GameDotNode* vn : validNode)
			{
				//终点清空链接
				il2cpp_field_set_value_object(vn, connectableField, (Il2CppObject*)emptyConnectable);

				*items++ = vn;
			}
			for (GameDotNode* en : endNode)
			{
				//终点清空链接
				il2cpp_field_set_value_object(en, connectableField, (Il2CppObject*)emptyConnectable);

				*items++ = en;
			}
		}
	}
	//*====================================================*//


	//*****************GameDotConnectUI*********************//
	Il2CppClass* GameDotConnectUI::Class()
	{
		const Il2CppImage* image = Il2CppUtils::GetImageByName("Assembly-CSharp");
		return il2cpp_class_from_name(image, "", "GameDotConnectUI");
	}
	Il2CppExtend::Il2CppArrayT<GameDotNodeLayer*>* GameDotConnectUI::GetLevels()
	{
		if (this->Levels && this->Levels->GetCount())
		{
			return this->Levels;
		}
		Il2CppObject* trans = UnityEngine::Component::GetTransform(this->LevelRoot);
		Il2CppObject* gameObject = UnityEngine::Component::GetGameObject(trans);

		return (Il2CppExtend::Il2CppArrayT<GameDotNodeLayer*>*)UnityEngine::GameObject::GetComponentsInChildren
																(gameObject, System::Object::GetType(GameDotNodeLayer::New()), true);
	}
	void GameDotConnectUI::Print()
	{
		Il2CppExtend::Il2CppArrayT<GameDotNodeLayer*>* levels = this->GetLevels();

		if (size_t count = levels->GetCount())
		{
			for (size_t i = 0u; i < count; ++i)
			{
				GameDotNodeLayer* item = levels->GetItemsPointer()[i];

				std::wstring name = std::wstring();
				{
					Il2CppString* s = UnityEngine::Object::GetName(UnityEngine::Component::GetGameObject(item));
					name.append((const wchar_t*)(s->chars), s->length);
				}
				Util::WriteDebugMessage(L"关卡%03d: %s\r\n", i, name.c_str());

				item->Print();
			}
		}
	}
	void GameDotConnectUI::Patch()
	{
		Il2CppExtend::Il2CppArrayT<GameDotNodeLayer*>* levels = this->GetLevels();
		if (size_t count = levels->GetCount())
		{
			GameDotNodeLayer** items = levels->GetItemsPointer();
			for (size_t i = 0u; i < count; ++i)
			{
				items[i]->Patch();
			}
		}
	}
	//*====================================================*//
}

